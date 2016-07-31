//-----------------------------------------------------------------------
// <copyright file="User.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace UserStorage
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents user entity.
    /// </summary>
    [Serializable]
    [DataContract]
    public class User : IEquatable<User>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User()
        {
            this.VisaRecords = new List<VisaRecord>();
            this.Name = string.Empty;
            this.LastName = string.Empty;
            this.PersonalId = string.Empty;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets User Id.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets User name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets  User last name.
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets User date of birth.
        /// </summary>
        [DataMember]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets User personal Id.
        /// </summary>
        [DataMember]
        public string PersonalId { get; set; }

        /// <summary>
        /// Gets or sets User gender.
        /// </summary>
        [DataMember]
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets Collection of user visa records.
        /// </summary>
        [DataMember]
        public List<VisaRecord> VisaRecords { get; set; }
        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a string that represents the current User.
        /// </summary>
        /// <returns> A string that represents the current User.</returns>
        public override string ToString() => $"{this.Id} {this.Name} {this.LastName} {this.PersonalId}";

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"> The object to compare with the current object.</param>
        /// <returns> True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as User;
            if (other == null)
            {
                return false;
            }

            if (this.Equals(other))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns> A hash code for the current User.</returns>
        public override int GetHashCode()
        {
            int? hash = 17;
            hash = (hash * 23) + this.Id.GetHashCode();
            hash = (hash * 23) + this.Name?.GetHashCode();
            hash = (hash * 23) + this.LastName?.GetHashCode();
            hash = (hash * 23) + this.DateOfBirth.GetHashCode();
            hash = (hash * 23) + this.PersonalId?.GetHashCode();
            hash = (hash * 23) + this.Gender.GetHashCode();
            return hash.GetValueOrDefault();
        }

        /// <summary>
        /// Determines whether the specified User is equal to the current User.
        /// </summary>
        /// <param name="other"> The User to compare with the current User.</param>
        /// <returns> True if the specified User is equal to the User object; otherwise, false.</returns>
        public bool Equals(User other)
        {
            if (other.Name == this.Name && other.LastName == this.LastName)
            {
                return true;
            }

            return false;
        }

        #endregion
    }
}
