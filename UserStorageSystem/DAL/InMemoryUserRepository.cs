//-----------------------------------------------------------------------
// <copyright file="InMemoryUserRepository.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using UserStorage;

    /// <summary>
    /// Represents common functionality for accessing user storage.
    /// </summary>
    [Serializable]
    public class InMemoryUserRepository : MarshalByRefObject, IUserRepository, IDisposable
    {
        #region Fields

        private readonly List<User> users;
        private readonly ReaderWriterLockSlim locker;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryUserRepository"/> class.
        /// </summary>
        public InMemoryUserRepository()
        {
            this.locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            this.users = new List<User>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all users in the storage.
        /// </summary>
        /// <returns> List of users.</returns>
        public IList<User> GetAll()
        {
            this.locker.EnterReadLock();
            var users = this.users;
            this.locker.ExitReadLock();
            return users;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Create(User user)
        {
            this.locker.EnterWriteLock();
            this.users.Add(user);
            this.locker.ExitWriteLock();
        }

        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Delete(User user)
        {
            this.locker.EnterWriteLock();
            this.users.Remove(user);
            this.locker.ExitWriteLock();
        }

        /// <summary>
        /// Performs a search for user using specified filter.
        /// </summary>
        /// <param name="filter"> Search criteria.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> Find(Expression<Func<User, bool>> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            this.locker.EnterReadLock();
            var result = new List<User>();
            Parallel.ForEach(
                this.users, 
                user =>
                {
                    this.users.Where(filter.Compile());
                    if ((filter.Compile().DynamicInvoke(user) as bool?).GetValueOrDefault())
                    {
                        result.Add(user);
                    }
                });
            this.locker.ExitReadLock();
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.locker.Dispose();
        }

        #endregion
    }
}