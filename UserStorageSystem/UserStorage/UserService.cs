//-----------------------------------------------------------------------
// <copyright file="UserService.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace UserStorage
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using IDGenerator;

    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    public class UserService : MarshalByRefObject, IUserService, IXmlSerializable
    {
        #region Fields

        private static readonly BooleanSwitch BoolSwitch = new BooleanSwitch("logSwitch", string.Empty);
        private readonly bool isMaster;
        private readonly IGenerator<int> idGenerator;
        private readonly IUserValidator userValidator;
        private readonly IUserRepository userRepository;
        private readonly IPEndPoint address;
        private readonly int internalCommunicationPort;
        private readonly List<IPEndPoint> services;
        private int lastUserId;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        public UserService()
        {
            this.services = new List<IPEndPoint>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="idGenerator"> IGenerator` instance.</param>
        /// <param name="userValidator"> UserValidator instance.</param>
        /// <param name="userRepository"> UserRepository instance.</param>
        /// <param name="address"> Service address.</param>
        /// <param name="internalPort"> Port for receiving messages from the master.</param>
        public UserService(IGenerator<int> idGenerator, IUserValidator userValidator, IUserRepository userRepository, IPEndPoint address, int internalPort) : this()
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (userValidator == null)
            {
                throw new ArgumentNullException(nameof(userValidator));
            }

            if (userRepository == null)
            {
                throw new ArgumentNullException(nameof(userRepository));
            }

            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.idGenerator = idGenerator;
            this.userValidator = userValidator;
            this.userRepository = userRepository;
            this.address = address;
            this.internalCommunicationPort = internalPort;
            this.StartListener();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="idGenerator"> IGenerator` instance.</param>
        /// <param name="userValidator"> UserValidator instance.</param>
        /// <param name="userRepository"> UserRepository instance.</param>
        /// <param name="address"> Service address.</param>
        /// <param name="services"> Collection of slave services.</param>
        public UserService(IGenerator<int> idGenerator, IUserValidator userValidator, IUserRepository userRepository, IPEndPoint address, List<IPEndPoint> services) 
            : this(idGenerator, userValidator, userRepository, address, 0)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            this.services = services;
            this.isMaster = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Service address.
        /// </summary>
        public IPEndPoint Address => this.address;

        #endregion

        #region IUserService Methods

        /// <summary>
        /// Gets weather this service is master.
        /// </summary>
        /// <returns> True if service is master; otherwise - false;</returns>
        public bool IsMaster() => this.isMaster;

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> A new user.</returns>
        public User CreateUser(User user)
        {
            if (!this.IsMaster())
            {
                throw new FaultException("Action is not supported.");
            }

            if (BoolSwitch.Enabled)
            {
                Trace.TraceInformation($"Create user. {user}");
            }

            if (!this.userValidator.IsValid(user))
            {
                throw new FaultException("User validation failed.");
            }

            this.lastUserId = this.idGenerator.GenerateId(this.lastUserId);
            user.Id = this.lastUserId;
            try
            {
                this.userRepository.Create(user);
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw new FaultException(ex.Message);
            }

            var message = new StorageChangedMessage { IsAdded = true, User = user };
            this.NotifySlaves(message);
            return user;
        }

        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> user instance.</param>
        public void DeleteUser(User user)
        {
            if (!this.IsMaster())
            {
                throw new FaultException("Action is not supported.");
            }

            if (BoolSwitch.Enabled)
            {
                Trace.TraceInformation("Delete user. {user.ToString()}");
            }

            try
            {
                this.userRepository.Delete(user);
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw new FaultException(ex.Message);
            }

            var message = new StorageChangedMessage { IsRemoved = true, User = user };
            this.NotifySlaves(message);
        }

        /// <summary>
        /// Performs a search for user by specified user name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByName(string name)
        {
            if (BoolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by name = {name}.");
            }

            List<User> users;
            try
            {
                users = this.userRepository.Find(u => u.Name == name).ToList();
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceInformation($"Users found = {users.Count}.");
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw new FaultException(ex.Message);
            }

            return users;
        }

        /// <summary>
        /// Performs a search for user by specified user name and last name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <param name="lastName"> User last name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByNameAndLastName(string name, string lastName)
        {
            if (BoolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by name and last name, name = {name}, last name = {lastName}.");
            }

            List<User> users;
            try
            {
                users = this.userRepository.Find(u => u.Name == name && u.LastName == lastName).ToList();
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceInformation($"Users found = {users.Count}.");
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw new FaultException(ex.Message);
            }

            return users;
        }

        /// <summary>
        /// Performs a search for user by specified personal id.
        /// </summary>
        /// <param name="personalId"> User personal id.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByPersonalId(string personalId)
        {
            if (BoolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by personalId, personalId = {personalId}.");
            }

            List<User> users;
            try
            {
                users = this.userRepository.Find(u => u.PersonalId == personalId).ToList();
                if (BoolSwitch.Enabled)
                {
                    Trace.TraceInformation($"Users found = {users.Count}.");
                }
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw new FaultException(ex.Message);
            }

            return users;
        }
        #endregion

        #region Xml Methods

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="xmlReader"> XmlReader instance.</param>
        public void ReadXml(XmlReader xmlReader)
        {
            if (!this.IsMaster())
            {
                throw new NotSupportedException();
            }

            var doc = new XDocument();
            if (!doc.TryLoad(xmlReader, out doc))
            {
                return;
            }

            int.TryParse(doc.Descendants("LastId").SingleOrDefault().Value, out this.lastUserId);
            List<User> users;
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
                this.LogException(ex);
                throw;
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }

            this.InitUsers(users);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="xmlWriter"> XmlWriter instance.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (!this.IsMaster())
            {
                throw new NotSupportedException();
            }

            if (xmlWriter == null)
            {
                throw new ArgumentNullException(nameof(xmlWriter));
            }

            IList<User> users;
            try
            {
                users = this.userRepository.GetAll();
            }
            catch (Exception ex)
            {
                this.LogException(ex);
                throw;
            }

            var doc = new XDocument(
                new XElement(
                    "ServiceState",
                     new XElement("LastId", this.lastUserId),
                     new XElement(
                         "Users",
                         users.Select(
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
            doc.Save(xmlWriter);
        }

        XmlSchema IXmlSerializable.GetSchema() => null;

        #endregion

        #region Private Methods
        private void InitUsers(List<User> users)
        {
            foreach (var u in users)
            {
                this.userRepository.Create(u);
                var message = new StorageChangedMessage { IsAdded = true, User = u };
                this.NotifySlaves(message);
            }
        }
        
        private async void StartListener()
        {
            var tcpListener = TcpListener.Create(this.internalCommunicationPort);
            tcpListener.Start();
            while (true)
            {
                using (var tcpClient = await tcpListener.AcceptTcpClientAsync())
                using (var networkStream = tcpClient.GetStream())
                {
                    // read message size from the stream
                    var messageSize = new byte[4];
                    await networkStream.ReadAsync(messageSize, 0, 4);
                    var size = BitConverter.ToInt32(messageSize, 0);
                    var buffer = new byte[size];

                    // read the actual message
                    await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    var serializer = new XmlSerializer(typeof(StorageChangedMessage));
                    var message = serializer.Deserialize(new MemoryStream(buffer)) as StorageChangedMessage;
                    this.UpdateData(message);

                    // send an empty message indicating that data has been updated
                    await networkStream.WriteAsync(new byte[0], 0, 0);
                }
            }
        }

        private async void NotifySlaves(StorageChangedMessage message)
        {
            var stream = new MemoryStream();
            using (var textWriter = XmlWriter.Create(stream))
            {
                var serializer = new XmlSerializer(typeof(StorageChangedMessage));
                serializer.Serialize(textWriter, message);
            }

            foreach (var addr in this.services)
            {
                using (var tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(addr.Address, addr.Port);
                    using (var networkStream = tcpClient.GetStream())
                    {
                        // send message size
                        var messageSize = BitConverter.GetBytes(stream.GetBuffer().Length);
                        await networkStream.WriteAsync(messageSize, 0, messageSize.Length);

                        // send the actual message
                        await networkStream.WriteAsync(stream.GetBuffer(), 0, stream.GetBuffer().Length);

                        // wait for confirmation from the slave that data has been updated
                        var buffer = new byte[0];
                        await networkStream.ReadAsync(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        private void UpdateData(StorageChangedMessage message)
        {
            if (message.IsAdded)
            {
                try
                {
                    this.userRepository.Create(message.User);
                }
                catch (Exception ex)
                {
                    this.LogException(ex);
                    throw new FaultException(ex.Message);
                }
            }

            if (message.IsRemoved)
            {
                try
                {
                    this.userRepository.Delete(message.User);
                }
                catch (Exception ex)
                {
                    this.LogException(ex);
                    throw new FaultException(ex.Message);
                }
            }
        }

        private void LogException(Exception ex)
        {
            if (BoolSwitch.Enabled)
            {
                Trace.TraceError($"{DateTime.Now} Exception {ex}");
            }
        }
        #endregion
    }
}
