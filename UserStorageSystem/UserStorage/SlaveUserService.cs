using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage
{
    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    public class SlaveUserService : MarshalByRefObject, IUserService
    {
        #region Fields
        private readonly IUserRepository userRepository;
        private readonly IMasterUserService masterService;

        private static readonly BooleanSwitch boolSwitch = new BooleanSwitch("logSwitch", string.Empty);
        #endregion

        #region Constructors

        /// <summary>
        /// Instanciates SlaveUserService.
        /// </summary>
        public SlaveUserService() { }

        /// <summary>
        /// Instanciates SlaveUserService with specified parameters.
        /// </summary>
        /// <param name="masterService"> MasterUserService instance.</param>
        /// <param name="userRepository"> userRepository instance.</param>
        public SlaveUserService(IMasterUserService masterService, IUserRepository userRepository)
        {
            if (masterService == null) throw new ArgumentNullException(nameof(masterService));
            if (userRepository == null) throw new ArgumentNullException(nameof(userRepository));
            this.masterService = masterService;
            this.userRepository = userRepository;

            masterService.StorageChanged += OnStorageChanged;
        }
        #endregion

        #region Not Supported Methods
        public int CreateUser(User user)
        {
            throw new NotSupportedException();
        }

        public void DeleteUser(User user)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region Public Methods
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
        /// <returns></returns>
        public IEnumerable<int> FindByPersonalId(string personalId)
        {
            if (boolSwitch.Enabled)
            {
                Trace.TraceInformation($"Find user by personalId, personalId = {personalId}.");
            }
            return userRepository.Find(u => u.PersonalId == personalId).Select(u => u.Id).ToList();
        }
        #endregion

        #region Private methods
        private void OnStorageChanged(object sender, StorageChangedEventArgs e)
        {
            var user = e.User;
            if (e.IsAdded) userRepository.GetAll().Add(e.User);
            if (e.IsRemoved) userRepository.GetAll().Remove(e.User);
        }
        #endregion
    }
}
