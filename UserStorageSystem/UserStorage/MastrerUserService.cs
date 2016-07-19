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

namespace UserStorage
{
    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    public class MastrerUserService : IMasterUserService, IXmlSerializable
    {
        #region Fields
        private readonly IGenerator<int> idGenerator;
        private readonly IUserValidator userValidator;
        private readonly IUserRepository userRepository;
        private int lastUserId;

        private static readonly BooleanSwitch boolSwitch = new BooleanSwitch("logSwitch", string.Empty);

        #endregion

        #region Events
        /// <summary>
        /// Event raised after user storage was changed.
        /// </summary>
        public event EventHandler<StorageChangedEventArgs> StorageChanged = delegate { };
        #endregion

        #region Constructors
        /// <summary>
        /// Instanciates MasterUserService with specified parameter.
        /// </summary>
        /// <param name="idGenerator"> IGenerator` instance.</param>
        /// <param name="userValidator"> UserValidator instance.</param>
        /// <param name="userRepository"> UserRepository instance.</param>
        public MastrerUserService(IGenerator<int> idGenerator, IUserValidator userValidator, IUserRepository userRepository)
        {
            this.idGenerator = idGenerator;
            this.userValidator = userValidator;
            this.userRepository = userRepository;
        }
        #endregion

        #region IUserService Methods
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> Id generated for a new user.</returns>
        public int CreateUser(User user)
        {
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
            return userRepository.Find(u => u.Name == name).Select(u => u.Id);
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
            return userRepository.Find(u => u.Name == name && u.LastName == lastName).Select(u => u.Id);
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
            return userRepository.Find(u => u.PersonalId == personalId).Select(u => u.Id);
        }
        #endregion

        #region IXmlSerializable Methods

        public XmlSchema GetSchema() => null;

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader"> The System.Xml.XmlReader stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            try
            {
                reader.MoveToContent();
                reader.ReadStartElement();
                reader.ReadStartElement();
                int.TryParse(reader.ReadElementString("Value"), out lastUserId);
                reader.ReadEndElement();
                var users = new XmlSerializer(userRepository.GetAll().GetType(), new XmlRootAttribute("Users")).Deserialize(reader) as List<User>;
                InitRepository(users);
            }
            catch (XmlException ex)
            {
                if (boolSwitch.Enabled)
                {
                    Trace.TraceError($"Empty xml - file.", ex);
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer"> The System.Xml.XmlWriter stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("ServiceState");
            writer.WriteStartElement("LastId");
            writer.WriteElementString("Value", lastUserId.ToString());
            writer.WriteEndElement();
            new XmlSerializer(userRepository.GetAll().GetType(), new XmlRootAttribute("Users")).Serialize(writer, userRepository.GetAll());
            writer.WriteEndElement();
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

        #endregion

        #region Private Methods
        private void InitRepository(List<User> users)
        {
            foreach (var u in users)
            {
                userRepository.Create(u);
            }
        }
        #endregion
    }
}
