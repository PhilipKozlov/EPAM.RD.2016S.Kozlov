//-----------------------------------------------------------------------
// <copyright file="UserValidator.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Validator
{
    using System;
    using System.Collections.Generic;
    using UserStorage;

    /// <summary>
    /// Provides functionality for validating User entities.
    /// </summary>
    [Serializable]
    public class UserValidator : IUserValidator
    {
        #region Public Methods

        /// <summary>
        /// Validates user.
        /// </summary>
        /// <param name="user"> IUser instance to validate.</param>
        /// <returns> True if user instance is valid; otherwise - false.</returns>
        public bool IsValid(User user)
        {
            if (user == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(user.Name) || string.IsNullOrEmpty(user.LastName))
            {
                return false;
            }

            if (this.HasNonLetters(user.Name) || this.HasNonLetters(user.LastName))
            {
                return false;
            }

            if (!this.IsIdValid(user.PersonalId))
            {
                return false;
            }

            if (!this.AreVisaRecordsValid(user.VisaRecords))
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Private Methods

        private bool HasNonLetters(string str)
        {
            foreach (var c in str)
            {
                if (!char.IsLetter(c))
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsIdValid(string personalId)
        {
            if (string.IsNullOrEmpty(personalId) || string.IsNullOrEmpty(personalId))
            {
                return false;
            }

            if (personalId.Length > 14 || personalId.Length < 14)
            {
                return false;
            }

            return true;
        }

        private bool AreVisaRecordsValid(ICollection<VisaRecord> visaRecords)
        {
            if (visaRecords == null)
            {
                return false;
            }

            foreach (var vr in visaRecords)
            {
                if (!this.IsVisaValid(vr))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsVisaValid(VisaRecord visa)
        {
            if (visa.Start > visa.End)
            {
                return false;
            }

            if (this.HasNonLetters(visa.Country))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}