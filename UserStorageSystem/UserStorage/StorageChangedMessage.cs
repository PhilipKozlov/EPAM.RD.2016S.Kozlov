using System;

namespace UserStorage
{
    /// <summary>
    /// Contains information about change in user storage.
    /// </summary>
    [Serializable]
    public class StorageChangedMessage
    {
        #region Properties
        /// <summary>
        /// User instance.
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// True if user has been removed; otherwise - false.
        /// </summary>
        public bool IsRemoved { get; set; }
        /// <summary>
        /// True if user has been added; otherwise - false.
        /// </summary>
        public bool IsAdded { get; set; }
        #endregion
    }
}
