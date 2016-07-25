using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UserStorage
{
    /// <summary>
    /// Provides functionality for working with users.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets or sets weather this service is master.
        /// </summary>
        bool IsMaster { get; }
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> Id generated for a new user.</returns>
        int CreateUser(User user);
        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> user instance.</param>
        void DeleteUser(User user);
        /// <summary>
        /// Performs a search for user by specified user name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <returns> Collection of users.</returns>
        IEnumerable<int> FindByName(string name);
        /// <summary>
        /// Performs a search for user by specified user name and last name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <param name="lastName"> User last name.</param>
        /// <returns> Collection of users.</returns>
        IEnumerable<int> FindByNameAndLastName(string name, string lastName);
        /// <summary>
        /// Performs a search for user by specified personal id.
        /// </summary>
        /// <param name="personalId"></param>
        /// <returns> Collection of users.</returns>
        IEnumerable<int> FindByPersonalId(string personalId);
    }
}
