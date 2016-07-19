using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage
{
    /// <summary>
    /// Provides additional functionality for Master service.
    /// </summary>
    public interface IMasterUserService : IUserService
    {
        /// <summary>
        /// Event raised after user storage was changed.
        /// </summary>
        event EventHandler<StorageChangedEventArgs> StorageChanged;
    }
}
