using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UserStorage;

namespace DAL
{
    /// <summary>
    /// Represents common functionality for accessing user storage.
    /// </summary>
    [Serializable]
    public class InMemoryUserRepository : IUserRepository
    {
        #region Fields
        private List<User> Users { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Instanciates InMemoryUserRepository.
        /// </summary>
        public InMemoryUserRepository()
        {
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
            return Users;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Create(User user)
        {
            Users.Add(user);
        }
        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> User instance.</param>
        public void Delete(User user)
        {
            Users.Remove(user);
        }
        /// <summary>
        /// Performs a search for user using specified filter.
        /// </summary>
        /// <param name="filter"> Search criteria.</param>
        /// <returns> Collection of users.</returns>
        public IEnumerable<User> Find(Expression<Func<User, bool>> filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));
            return Users.Where(filter.Compile());
        }
        #endregion
    }
}
