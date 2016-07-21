using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage
{
    /// <summary>
    /// Represents common functionality for accessing user storage.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Gets all users in the storage.
        /// </summary>
        /// <returns></returns>
        IList<User> GetAll();
        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        void Create(User user);
        /// <summary>
        /// Deletes user from storage.
        /// </summary>
        /// <param name="user"> User instance.</param>
        void Delete(User user);
        /// <summary>
        /// Performs a search for user using specified filter.
        /// </summary>
        /// <param name="filter"> Search criteria.</param>
        /// <returns> Collection of users.</returns>
        IEnumerable<User> Find(Expression<Func<User, bool>> filter);
    }
}
