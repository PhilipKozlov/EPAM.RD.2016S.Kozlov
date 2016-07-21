using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserStorage
{
    /// <summary>
    /// Contains Visa information.
    /// </summary>
    [Serializable]
    public struct VisaRecord
    {
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
    }
}
