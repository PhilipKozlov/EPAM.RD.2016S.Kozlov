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

namespace UserStorage
{
    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    [Serializable]
    public class UserService : MarshalByRefObject, IUserService
    {
        #region Fields
        private readonly IGenerator<int> idGenerator;
        private readonly IUserValidator userValidator;
        private readonly IUserRepository userRepository;
        private int lastUserId;

        private IEnumerable<IUserService> slaveServices;

        private static readonly BooleanSwitch boolSwitch = new BooleanSwitch("logSwitch", string.Empty);


        IUserService service;

        #endregion

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
            slaveServices = new List<IUserService>();
        }

        /// <summary>
        /// Instanciates MasterUserService with specified parameter.
        /// </summary>
        /// <param name="idGenerator"> IGenerator` instance.</param>
        /// <param name="userValidator"> UserValidator instance.</param>
        /// <param name="userRepository"> UserRepository instance.</param>
        public UserService(IGenerator<int> idGenerator, IUserValidator userValidator, IUserRepository userRepository) : this()
        {
            this.idGenerator = idGenerator;
            this.userValidator = userValidator;
            this.userRepository = userRepository;
        }

        // SLAVE .CTOR.
        public UserService(IUserService masterService, IUserRepository userRepository)
        {
            service = masterService;
            this.userRepository = userRepository;

            (service as UserService).StorageChanged += OnStorageChanged;
        }
        #endregion

        #region Properties
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


            OnStorageChanged(new StorageChangedEventArgs(user, added:true));


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


            OnStorageChanged(new StorageChangedEventArgs(user, removed:true));


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
                var users = new XmlSerializer(userRepository.GetAll().GetType()/*, new XmlRootAttribute("Users")*/).Deserialize(xmlReader) as List<User>;
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
                new XmlSerializer(userRepository.GetAll().GetType()/*, new XmlRootAttribute("Users")*/).Serialize(xmlWriter, userRepository.GetAll());
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
        #endregion
    }
}
