using System;
using System.Runtime.Serialization;

namespace UserStorage
{
    /// <summary>
    /// Enumeration of possible user genders.
    /// </summary>
    [Serializable]
    [DataContract]
    public enum Gender
    {
        /// <summary>
        /// User of male gender.
        /// </summary>
        [EnumMember]
        Male,

        /// <summary>
        /// User of female gender.
        /// </summary>
        [EnumMember]
        Female
    }
}
