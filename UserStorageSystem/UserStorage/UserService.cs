using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDGenerator;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UserStorage
{
    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    public class UserService : MarshalByRefObject, IUserService
    {
        #region Fields
        private readonly IGenerator<int> idGenerator;
        private readonly IUserValidator userValidator;
        private readonly IUserRepository userRepository;
        private int lastUserId;

        //private IEnumerable<IUserService> slaveServices;

        private static readonly BooleanSwitch boolSwitch = new BooleanSwitch("logSwitch", string.Empty);

        // Will become absolete.
        IUserService service;

        //IPEndPoint address;

        #endregion

        // Will become Absolete.
        #region Events
        /// <summary>
        /// Event raised after user storage was changed.
        /// </summary>
        public event EventHandler<StorageChangedEventArgs> StorageChanged = delegate { };
        #endregion

        #region Constructors
        /// <summary>
        /// Instanciates MasterUserService.
        /// </summary>
        public UserService()
        {
            Slaves = new List<IPEndPoint>();
        }

        /// <summary>
        /// Instanciates MasterUserService with specified parameter.
        /// </summary>
        /// <param name="idGenerator"> IGenerator` instance.</param>
        /// <param name="userValidator"> UserValidator instance.</param>
        /// <param name="userRepository"> UserRepository instance.</param>
        public UserService(IGenerator<int> idGenerator, IUserValidator userValidator, IUserRepository userRepository,  bool isMaster = false) : this()
        {
            this.idGenerator = idGenerator;
            this.userValidator = userValidator;
            this.userRepository = userRepository;
            IsMaster = isMaster;
            //if (!IsMaster) StartListener();
        }

        // SLAVE .CTOR.
        //public UserService(IUserService masterService, IUserRepository userRepository)
        //{
        //    service = masterService;
        //    this.userRepository = userRepository;

        //    (service as UserService).StorageChanged += OnStorageChanged;
        //}
        #endregion

        #region Properties

        /// <summary>
        /// Service address.
        /// </summary>
        public IPEndPoint Address { get; set; }

        /// <summary>
        /// Gets or sets collection of slave services.
        /// </summary>
        public List<IPEndPoint> Slaves { get; set; }

        /// <summary>
        /// Gets or sets weather this service is master.
        /// </summary>
        public bool IsMaster { get; set; }
        #endregion

        #region IUserService Methods
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> Id generated for a new user.</returns>
        public int CreateUser(User user)
        {
            if (!IsMaster) throw new NotSupportedException();
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

            //OnStorageChanged(new StorageChangedEventArgs(user, added:true));

            return lastUserId;
        }

        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> user instance.</param>
        public void DeleteUser(User user)
        {
            if (!IsMaster) throw new NotSupportedException();
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation("Delete user. {user.ToString()}");
            }
            userRepository.Delete(user);

            var message = new StorageChangedMessage() { IsRemoved = true, User = user };
            NotifySlaves(message);

            //OnStorageChanged(new StorageChangedEventArgs(user, removed:true));

        }

        /// <summary>
        /// Performs a search for user by specified user name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<int> FindByName(string name)
        {
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by name = {name}.");
            }
            return userRepository.Find(u => u.Name == name).Select(u => u.Id).ToList();
        }

        /// <summary>
        /// Performs a search for user by specified user name and last name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <param name="lastName"> User last name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<int> FindByNameAndLastName(string name, string lastName)
        {
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by name and last name, name = {name}, last name = {lastName}.");
            }
            return userRepository.Find(u => u.Name == name && u.LastName == lastName).Select(u => u.Id).ToList();
        }

        /// <summary>
        /// Performs a search for user by specified personal id.
        /// </summary>
        /// <param name="personalId"></param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<int> FindByPersonalId(string personalId)
        {
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by personalId, personalId = {personalId}.");
            }
            return userRepository.Find(u => u.PersonalId == personalId).Select(u => u.Id).ToList();
        }
        #endregion

        #region Xml Methods

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="filePath"> The path to the xml file.</param>
        public void ReadXml(XmlReader xmlReader)
        {
            if (!IsMaster) throw new NotSupportedException();
            try
            {
                xmlReader.MoveToContent();
                xmlReader.ReadStartElement();
                xmlReader.ReadStartElement();
                int.TryParse(xmlReader.ReadElementString("Value"), out lastUserId);
                xmlReader.ReadEndElement();
                var users = new XmlSerializer(userRepository.GetAll().GetType()).Deserialize(xmlReader) as List<User>;
                InitUsers(users);
            }
            catch (InvalidOperationException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (XmlException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="filePath"> The path to the xml file.</param>
        public void WriteXml(XmlWriter xmlWriter)
        {
            if (!IsMaster) throw new NotSupportedException();
            try
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("ServiceState");
                xmlWriter.WriteStartElement("LastId");
                xmlWriter.WriteElementString("Value", lastUserId.ToString());
                xmlWriter.WriteEndElement();
                new XmlSerializer(userRepository.GetAll().GetType()).Serialize(xmlWriter, userRepository.GetAll());
                xmlWriter.WriteEndElement();
            }
            catch (InvalidOperationException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
            catch (EncoderFallbackException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError("{0:HH:mm:ss.fff} Exception {1}", DateTime.Now, ex);
                }
                throw;
            }
        }

        #endregion

        // Will become absolete
        #region Protected Methods
        /// <summary>
        /// Raises the StorageChanged event.
        /// </summary>
        /// <param name="e"> Additional event arguments.</param>
        protected virtual void OnStorageChanged(StorageChangedEventArgs e)
        {
            StorageChanged.Invoke(this, e);
        }

        private void OnStorageChanged(object sender, StorageChangedEventArgs e)
        {
            var user = e.User;
            if (e.IsAdded) userRepository.GetAll().Add(e.User);
            if (e.IsRemoved) userRepository.GetAll().Remove(e.User);
        }

        #endregion

        #region Private Methods
        private void InitUsers(List<User> users)
        {
            foreach (var u in users)
            {
                CreateUser(u);
            }
        }

        XmlSchema IXmlSerializable.GetSchema() => null;

        public async void StartListener()
        {
            var tcpListener = TcpListener.Create(Address.Port);
            tcpListener.Start();
            var tcpClient = await tcpListener.AcceptTcpClientAsync();
            using (var networkStream = tcpClient.GetStream())
            {
                var buffer = new byte[1024];
                var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                var formatter = new BinaryFormatter();
                var message = formatter.Deserialize(networkStream) as StorageChangedMessage;
                UpdateData(message);
            }
        }

        private async void NotifySlaves(StorageChangedMessage message)
        {
            foreach (var addr in Slaves)
            {
                using (var tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(addr.Address, addr.Port);
                    using (var networkStream = tcpClient.GetStream())
                    {
                        var stream = new MemoryStream();
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, message);
                        stream.Flush();
                        await networkStream.WriteAsync(stream.GetBuffer(), 0, stream.GetBuffer().Length);
                    }
                }
            }
        }

        private void UpdateData(StorageChangedMessage message)
        {
            if (message.IsAdded) userRepository.Create(message.User);
            if (message.IsRemoved) userRepository.Delete(message.User);
        }
        #endregion
    }
}
