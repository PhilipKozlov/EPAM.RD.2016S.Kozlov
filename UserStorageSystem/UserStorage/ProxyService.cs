using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UserStorage
{
    /// <summary>
    /// Simple proxy, that uses Round-Robin algorithm to distribute load between services.
    /// </summary>
    public class ProxyService : IUserService
    {
        #region Fields
        private readonly IList<IUserService> servicePool;
        private int nextInLine;
        IPEndPoint address;
        #endregion

        #region Constructors
        /// <summary>
        /// Instanciates ProxyUserService with specified parameters.
        /// </summary>
        /// <param name="services"> Collection of services.</param>
        public ProxyService(IPEndPoint address, IList<IUserService> services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            servicePool = services;
            this.address = address;
            nextInLine = 0;
        }
        #endregion

        #region IUserService Methods
        /// <summary>
        /// Gets weather this service is master.
        /// </summary>
        public bool IsMaster() => false;

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> Id generated for a new user.</returns>
        public User CreateUser(User user)
        {
            return servicePool.SingleOrDefault(s => s.IsMaster()).CreateUser(user);
        }

        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> user instance.</param>
        public void DeleteUser(User user)
        {
            servicePool.SingleOrDefault(s => s.IsMaster()).DeleteUser(user);
        }

        /// <summary>
        /// Performs a search for user by specified user name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByName(string name)
        {
            if (nextInLine >= servicePool.Count) nextInLine = 0;
            var result = servicePool[nextInLine].FindByName(name);
            nextInLine++;
            return result;
        }

        /// <summary>
        /// Performs a search for user by specified user name and last name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <param name="lastName"> User last name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByNameAndLastName(string name, string lastName)
        {
            if (nextInLine >= servicePool.Count) nextInLine = 0;
            var result = servicePool[nextInLine].FindByNameAndLastName(name, lastName);
            nextInLine++;
            return result;
        }

        /// <summary>
        /// Performs a search for user by specified personal id.
        /// </summary>
        /// <param name="personalId"></param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByPersonalId(string personalId)
        {
            if (nextInLine >= servicePool.Count) nextInLine = 0;
            var result = servicePool[nextInLine].FindByPersonalId(personalId);
            nextInLine++;
            return result;
        }

        #endregion
    }
}
