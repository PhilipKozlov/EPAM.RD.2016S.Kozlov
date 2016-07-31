//-----------------------------------------------------------------------
// <copyright file="VisaRecord.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace UserStorage
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Contains Visa information.
    /// </summary>
    [Serializable]
    [DataContract]
    public struct VisaRecord
    {
        #region Properties

        /// <summary>
        /// Gets or sets Country for visa.
        /// </summary>
        [DataMember]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets Visa start date.
        /// </summary>
        [DataMember]
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets Visa end date.
        /// </summary>
        [DataMember]
        public DateTime End { get; set; }

        #endregion
    }
}
