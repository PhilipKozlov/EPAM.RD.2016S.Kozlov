using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using UserStorage;

namespace DAL
{
    /// <summary>
    ///     Represents common functionality for accessing user storage.
    /// </summary>
    [Serializable]
    public class InMemoryUserRepository : MarshalByRefObject, IUserRepository, IDisposable
    {
        #region Constructors

        /// <summary>
        ///     Instanciates InMemoryUserRepository.
        /// </summary>
        public InMemoryUserRepository()
        {
            locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            users = new List<User>();
        }

        #endregion

        #region Fields

        private readonly List<User> users;
        private readonly ReaderWriterLockSlim locker;

        #endregion

        #region Public Methods

        /// <summary>
        ///     Gets all users in the storage.
        /// </summary>
        /// <returns></returns>
        public IList<User> GetAll()
        {
            locker.EnterReadLock();
            var users = this.users;
            locker.ExitReadLock();
            return users;
        }

        /// <summary>
        ///     Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Create(User user)
        {
            locker.EnterWriteLock();
            users.Add(user);
            locker.ExitWriteLock();
        }

        /// <summary>
        ///     Deletes user from storage.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Delete(User user)
        {
            locker.EnterWriteLock();
            users.Remove(user);
            locker.ExitWriteLock();
        }

        /// <summary>
        ///     Performs a search for user using specified filter.
        /// </summary>
        /// <param name="filter"> Search criteria.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> Find(Expression<Func<User, bool>> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            locker.EnterReadLock();

            var result = new List<User>();
            Parallel.ForEach(users, user =>
            {
                users.Where(filter.Compile());
                if ((filter.Compile().DynamicInvoke(user) as bool?).GetValueOrDefault())
                {
                    result.Add(user);
                }
            });
            locker.ExitReadLock();
            return result;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            locker.Dispose();
        }

        #endregion
    }
}