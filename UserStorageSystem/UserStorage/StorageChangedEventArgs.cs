using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage
{
    /// <summary>
    /// Represents storage changed event data.
    /// </summary>
    [Serializable]
    public sealed class StorageChangedEventArgs : EventArgs
    {
        #region Fields
        private User user;
        private bool isAdded;
        private bool isRemoved;
        #endregion

        #region Constructors
        /// <summary>
        /// Instanciates StorageChangedEventArgs with specified parameters.
        /// </summary>
        /// <param name="user"> User instance.</param>
        /// <param name="added"> True if user was added;otherwise - false.</param>
        /// <param name="removed"> True if user was removed;otherwise - fasle;</param>
        public StorageChangedEventArgs(User user, bool added = false, bool removed = false)
        {
            this.user = user;
            isAdded = added;
            isRemoved = removed;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets user.
        /// </summary>
        public User User => user;
        /// <summary>
        /// Gets true if user was added.
        /// </summary>
        public bool IsAdded => isAdded;
        /// <summary>
        /// Gets true if user was removed.
        /// </summary>
        public bool IsRemoved => isRemoved;
        #endregion
    }
}
