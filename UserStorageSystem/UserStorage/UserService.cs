using IDGenerator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace UserStorage
{
    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    public class UserService : MarshalByRefObject, IUserService, IXmlSerializable
    {
        #region Fields
        private readonly IGenerator<int> idGenerator;
        private readonly IUserValidator userValidator;
        private readonly IUserRepository userRepository;
        private IPEndPoint address;
        private List<IPEndPoint> services;
        private int lastUserId;
        private bool isMaster;

        private static readonly BooleanSwitch boolSwitch = new BooleanSwitch("logSwitch", string.Empty);
        #endregion

        #region Constructors
        /// <summary>
        /// Instanciates MasterUserService.
        /// </summary>
        public UserService()
        {
            services = new List<IPEndPoint>();
        }

        /// <summary>
        /// Instanciates UserService with specified parameters.
        /// </summary>
        /// <param name="idGenerator"> IGenerator` instance.</param>
        /// <param name="userValidator"> UserValidator instance.</param>
        /// <param name="userRepository"> UserRepository instance.</param>
        public UserService(IGenerator<int> idGenerator, IUserValidator userValidator, IUserRepository userRepository, IPEndPoint address) : this()
        {
            this.idGenerator = idGenerator;
            this.userValidator = userValidator;
            this.userRepository = userRepository;
            this.address = address;
            StartListener();
        }

        /// <summary>
        /// Instanciates UserService with specified parameters.
        /// </summary>
        /// <param name="idGenerator"> IGenerator` instance.</param>
        /// <param name="userValidator"> UserValidator instance.</param>
        /// <param name="userRepository"> UserRepository instance.</param>
        public UserService(IGenerator<int> idGenerator, IUserValidator userValidator, IUserRepository userRepository, IPEndPoint address, List<IPEndPoint> services) 
            : this(idGenerator, userValidator, userRepository, address)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            this.services = services;
            isMaster = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Service address.
        /// </summary>
        public IPEndPoint Address => address;

        #endregion

        #region IUserService Methods

        /// <summary>
        /// Gets weather this service is master.
        /// </summary>
        public bool IsMaster() => isMaster;
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> A new user.</returns>
        public User CreateUser(User user)
        {
            if (!IsMaster()) throw new NotSupportedException();
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Create user. {user.ToString()}");
            }

            if (!userValidator.IsValid(user)) throw new ArgumentException(nameof(user));
            lastUserId = idGenerator.GenerateId(lastUserId);
            user.Id = lastUserId;
            userRepository.Create(user);

            var message = new StorageChangedMessage() { IsAdded = true, User = user };
            NotifySlaves(message);
            return user;
        }

        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> user instance.</param>
        public void DeleteUser(User user)
        {
            if (!IsMaster()) throw new NotSupportedException();
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation("Delete user. {user.ToString()}");
            }
            userRepository.Delete(user);

            var message = new StorageChangedMessage() { IsRemoved = true, User = user };
            NotifySlaves(message);
        }

        /// <summary>
        /// Performs a search for user by specified user name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByName(string name)
        {
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by name = {name}.");
            }
            return userRepository.Find(u => u.Name == name).ToList();
        }

        /// <summary>
        /// Performs a search for user by specified user name and last name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <param name="lastName"> User last name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByNameAndLastName(string name, string lastName)
        {
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by name and last name, name = {name}, last name = {lastName}.");
            }
            return userRepository.Find(u => u.Name == name && u.LastName == lastName).ToList();
        }

        /// <summary>
        /// Performs a search for user by specified personal id.
        /// </summary>
        /// <param name="personalId"></param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByPersonalId(string personalId)
        {
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by personalId, personalId = {personalId}.");
            }
            return userRepository.Find(u => u.PersonalId == personalId).ToList();
        }
        #endregion

        #region Xml Methods

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="xmlReader"> XmlReader instance.</param>
        public void ReadXml(XmlReader xmlReader)
        {
            if (!IsMaster()) throw new NotSupportedException();
            var doc = new XDocument();
            if (!doc.TryLoad(xmlReader, out doc)) return;
            int.TryParse(doc.Descendants("LastId").SingleOrDefault().Value, out lastUserId);
            var users = new List<User>();
            try
            {
                users = doc.Descendants("User").Select(u => new User
                {
                    Id = Convert.ToInt32(u.Element("Id").Value),
                    Name = u.Element("Name").Value,
                    LastName = u.Element("LastName").Value,
                    DateOfBirth = Convert.ToDateTime(u.Element("DateOfBirth").Value),
                    PersonalId = u.Element("PersonalId").Value,
                    Gender = (Gender)Enum.Parse(typeof(Gender), u.Element("Gender").Value),
                    VisaRecords = u.Descendants("VisaRecord").Select(vr => new VisaRecord
                    {
                        Country = vr.Element("Country").Value,
                        Start = Convert.ToDateTime(vr.Element("Start").Value),
                        End = Convert.ToDateTime(vr.Element("Start").Value),
                    }).ToList()
                }).ToList();

            }
            catch (InvalidOperationException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            InitUsers(users);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="filePath"> The path to the xml file.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (!IsMaster()) throw new NotSupportedException();
            var doc = new XDocument(
                new XElement(
                    "ServiceState",
                     new XElement("LastId", lastUserId),
                     new XElement(
                         "Users", 
                         userRepository.GetAll().Select(
                             u => new XElement(
                                 "User",
                                  new XElement("Id", u.Id),
                                  new XElement("Name", u.Name),
                                  new XElement("LastName", u.LastName),
                                  new XElement("DateOfBirth", u.DateOfBirth),
                                  new XElement("PersonalId", u.PersonalId),
                                  new XElement("Gender", u.Gender),
                                  new XElement(
                                      "VisaRecords", 
                                      u.VisaRecords.Select(
                                          vr => new XElement(
                                                "VisaRecord",
                                                new XElement("Country", vr.Country),
                                                new XElement("Start", vr.Start),
                                                new XElement("End", vr.End)))))))));
            try
            {
                doc.Save(xmlWriter);
            }
            catch (InvalidOperationException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
        }

        #endregion

        #region Private Methods
        private void InitUsers(List<User> users)
        {
            foreach (var u in users)
            {
                userRepository.Create(u);
                var message = new StorageChangedMessage() { IsAdded = true, User = u };
                NotifySlaves(message);
            }
        }

        XmlSchema IXmlSerializable.GetSchema() => null;

        private void StartListener()
        {
            Task.Run(async () =>
            {
                var tcpListener = TcpListener.Create(address.Port);
                tcpListener.Start();
                while (true)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync();

                    using (var networkStream = tcpClient.GetStream())
                    {
                        var buffer = new byte[1024];
                        var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        var serializer = new XmlSerializer(typeof(StorageChangedMessage));
                        var message = serializer.Deserialize(new MemoryStream(buffer)) as StorageChangedMessage;
                        UpdateData(message);
                    }

                }
            });
        }

        private void NotifySlaves(StorageChangedMessage message)
        {
            Task.Run(async () =>
            {
                var stream = new MemoryStream();
                using (var textWriter = XmlWriter.Create(stream))
                {
                    var serializer = new XmlSerializer(typeof(StorageChangedMessage));
                    serializer.Serialize(textWriter, message);
                }

                foreach (var addr in services)
                {
                    using (var tcpClient = new TcpClient())
                    {
                        await tcpClient.ConnectAsync(addr.Address, addr.Port);
                        using (var networkStream = tcpClient.GetStream())
                        {
                            await networkStream.WriteAsync(stream.GetBuffer(), 0, stream.GetBuffer().Length);
                        }
                    }
                }
            });
        }

        private void UpdateData(StorageChangedMessage message)
        {
            if (message.IsAdded) userRepository.Create(message.User);
            if (message.IsRemoved) userRepository.Delete(message.User);
        }
        #endregion
    }
}
