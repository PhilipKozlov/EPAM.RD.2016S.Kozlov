using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage
{
    /// <summary>
    /// Provides a way for validation users.
    /// </summary>
    public interface IUserValidator
    {
        /// <summary>
        /// Validates user.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <returns> True if user is valid; otherwise - false.</returns>
        bool IsValid(User user);
    }
}
