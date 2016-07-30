using System.Collections.Generic;
using System.ServiceModel;

namespace UserStorage
{
    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    [ServiceContract]
    public interface IUserService
    {
        /// <summary>
        /// Gets weather this service is master.
        /// </summary>
        [OperationContract]
        bool IsMaster();
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> A new user.</returns>
        [OperationContract]
        User CreateUser(User user);
        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> user instance.</param>
        [OperationContract]
        void DeleteUser(User user);
        /// <summary>
        /// Performs a search for user by specified user name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <returns> Collection of users.</returns>
        [OperationContract]
        IEnumerable<User> FindByName(string name);
        /// <summary>
        /// Performs a search for user by specified user name and last name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <param name="lastName"> User last name.</param>
        /// <returns> Collection of users.</returns>
        [OperationContract]
        IEnumerable<User> FindByNameAndLastName(string name, string lastName);
        /// <summary>
        /// Performs a search for user by specified personal id.
        /// </summary>
        /// <param name="personalId"></param>
        /// <returns> Collection of users.</returns>
        [OperationContract]
        IEnumerable<User> FindByPersonalId(string personalId);
    }
}
