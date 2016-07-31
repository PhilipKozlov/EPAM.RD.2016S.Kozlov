//-----------------------------------------------------------------------
// <copyright file="IUserValidator.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

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
