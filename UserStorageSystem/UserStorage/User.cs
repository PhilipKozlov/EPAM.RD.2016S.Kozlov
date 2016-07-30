using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace UserStorage
{
    /// <summary>
    /// Represents user entity.
    /// </summary>
    [Serializable]
    [DataContract]
    public class User : IEquatable<User>
    {
        #region Constructors
        /// <summary>
        /// Instanciates User.
        /// </summary>
        public User()
        {
            VisaRecords = new List<VisaRecord>();
            Name = string.Empty;
            LastName = string.Empty;
            PersonalId = string.Empty;
        }
        #endregion

        #region Properties
        /// <summary>
        /// User identificator.
        /// </summary>
        [DataMember]
        public int Id { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// User last name.
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// User date of birth.
        /// </summary>
        [DataMember]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// User personal identificator.
        /// </summary>
        [DataMember]
        public string PersonalId { get; set; }

        /// <summary>
        /// User gender.
        /// </summary>
        [DataMember]
        public Gender Gender { get; set; }

        /// <summary>
        /// Collection of user visa records.
        /// </summary>
        [DataMember]
        public List<VisaRecord> VisaRecords { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a string that represents the current User.
        /// </summary>
        /// <returns> A string that represents the current User.</returns>
        public override string ToString() => $"{Id} {Name} {LastName} {PersonalId}";

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj"> The object to compare with the current object.</param>
        /// <returns> True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            var other = obj as User;
            if (other == null) return false;
            if (Equals(other)) return true;
            return false;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns> A hash code for the current User.</returns>
        public override int GetHashCode()
        {
            int? hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Name?.GetHashCode();
            hash = hash * 23 + LastName?.GetHashCode();
            hash = hash * 23 + DateOfBirth.GetHashCode();
            hash = hash * 23 + PersonalId?.GetHashCode();
            hash = hash * 23 + Gender.GetHashCode();
            return hash.GetValueOrDefault();
        }

        /// <summary>
        /// Determines whether the specified User is equal to the current User.
        /// </summary>
        /// <param name="other"> The User to compare with the current User.</param>
        /// <returns> True if the specified User is equal to the User object; otherwise, false.</returns>
        public bool Equals(User other)
        {
            if (other.Name == Name && other.LastName == LastName) return true;
            return false;
        }
        #endregion
    }
}
