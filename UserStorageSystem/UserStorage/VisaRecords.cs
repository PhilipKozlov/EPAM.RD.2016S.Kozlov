using System;

namespace UserStorage
{
    /// <summary>
    /// Contains Visa information.
    /// </summary>
    [Serializable]
    public struct VisaRecord
    {
        #region Properties
        /// <summary>
        /// Country for visa.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Visa start date.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Visa end date.
        /// </summary>
        public DateTime End { get; set; }
        #endregion
    }
}
