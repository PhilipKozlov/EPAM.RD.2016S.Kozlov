using System;
using System.Runtime.Serialization;

namespace UserStorage
{
    /// <summary>
    /// Contains Visa information.
    /// </summary>
    [Serializable]
    [DataContract]
    public struct VisaRecord
    {
        #region Properties
        /// <summary>
        /// Country for visa.
        /// </summary>
        [DataMember]
        public string Country { get; set; }

        /// <summary>
        /// Visa start date.
        /// </summary>
        [DataMember]
        public DateTime Start { get; set; }

        /// <summary>
        /// Visa end date.
        /// </summary>
        [DataMember]
        public DateTime End { get; set; }
        #endregion
    }
}
