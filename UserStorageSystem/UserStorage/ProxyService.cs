//-----------------------------------------------------------------------
// <copyright file="ProxyService.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace UserStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// Simple proxy, that uses Round-Robin algorithm to distribute load between services.
    /// </summary>
    public class ProxyService : IUserService
    {
        #region Fields

        private readonly IList<IUserService> servicePool;
        private readonly IPEndPoint address;
        private int nextInLine;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyService"/> class.
        /// </summary>
        /// <param name="address"> Proxy address.</param>
        /// <param name="services"> Collection of services.</param>
        public ProxyService(IPEndPoint address, IList<IUserService> services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            this.servicePool = services;
            this.address = address;
            this.nextInLine = 0;
        }

        #endregion

        #region IUserService Methods

        /// <summary>
        /// Gets weather this service is master.
        /// </summary>
        /// <returns> True if master; otherwise - false.</returns>
        public bool IsMaster() => false;

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> Id generated for a new user.</returns>
        public User CreateUser(User user)
        {
            return this.servicePool?.SingleOrDefault(s => s.IsMaster())?.CreateUser(user);
        }

        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> user instance.</param>
        public void DeleteUser(User user)
        {
            this.servicePool?.SingleOrDefault(s => s.IsMaster())?.DeleteUser(user);
        }

        /// <summary>
        /// Performs a search for user by specified user name.
        /// </summary>
        /// <param name="name"> User name.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByName(string name)
        {
            if (this.nextInLine >= this.servicePool.Count)
            {
                this.nextInLine = 0;
            }

            var result = this.servicePool[this.nextInLine].FindByName(name);
            this.nextInLine++;
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
            if (this.nextInLine >= this.servicePool.Count)
            {
                this.nextInLine = 0;
            }

            var result = this.servicePool[this.nextInLine].FindByNameAndLastName(name, lastName);
            this.nextInLine++;
            return result;
        }

        /// <summary>
        /// Performs a search for user by specified personal id.
        /// </summary>
        /// <param name="personalId"> User personal id.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> FindByPersonalId(string personalId)
        {
            if (this.nextInLine >= this.servicePool.Count)
            {
                this.nextInLine = 0;
            }

            var result = this.servicePool[this.nextInLine].FindByPersonalId(personalId);
            this.nextInLine++;
            return result;
        }

        #endregion
    }
}
