using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UserStorage;
using System.Threading;
using System.Runtime.Serialization;

namespace DAL
{
    /// <summary>
    /// Represents common functionality for accessing user storage.
    /// </summary>
    [Serializable]
    public class InMemoryUserRepository : IUserRepository, IDisposable
    {
        #region Fields
        private  List<User> Users { get; set; }
        [NonSerialized]
        private ReaderWriterLockSlim locker;
        #endregion


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        }

        #region Constructors
        /// <summary>
        /// Instanciates InMemoryUserRepository.
        /// </summary>
        public InMemoryUserRepository()
        {
            locker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
            Users = new List<User>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets all users in the storage.
        /// </summary>
        /// <returns></returns>
        public IList<User> GetAll()
        {
            locker.EnterReadLock();
            var users = Users;
            locker.ExitReadLock();
            return users;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Create(User user)
        {
            locker.EnterWriteLock();
            Users.Add(user);
            locker.ExitWriteLock();
        }
        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Delete(User user)
        {
            locker.EnterWriteLock();
            Users.Remove(user);
            locker.ExitWriteLock();
        }
        /// <summary>
        /// Performs a search for user using specified filter.
        /// </summary>
        /// <param name="filter"> Search criteria.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> Find(Expression<Func<User, bool>> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            locker.EnterReadLock();
            var users = Users.Where(filter.Compile());
            locker.ExitReadLock();
            return users;
        }

        public void Dispose()
        {
            locker.Dispose();
        }
        #endregion
    }
}
