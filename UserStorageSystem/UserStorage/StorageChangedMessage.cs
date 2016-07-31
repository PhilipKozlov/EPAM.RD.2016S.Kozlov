//-----------------------------------------------------------------------
// <copyright file="StorageChangedMessage.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace UserStorage
{
    using System;

    /// <summary>
    /// Contains information about change in user storage.
    /// </summary>
    [Serializable]
    public class StorageChangedMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets User instance.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user was removed.
        /// </summary>
        public bool IsRemoved { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user was added.
        /// </summary>
        public bool IsAdded { get; set; }

        #endregion
    }
}
