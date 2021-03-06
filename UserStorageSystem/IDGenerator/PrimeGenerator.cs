﻿//-----------------------------------------------------------------------
// <copyright file="PrimeGenerator.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace IDGenerator
{
    using System;

    /// <summary>
    /// Provides methods for generating prime numbers.
    /// </summary>
    [Serializable]
    public class PrimeGenerator : IGenerator<int>
    {
        #region IGenerator methods

        /// <summary>
        /// Generates single id.
        /// </summary>
        /// <param name="prevId"> Previously generated id.</param>
        /// <returns> 32 bit integer representing Id.</returns>
        public int GenerateId(int prevId)
        {
            if (prevId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(prevId));
            }

            return this.GeneratePrime(prevId);
        }

        #endregion

        #region Private Methods

        private int GeneratePrime(int number)
        {
            while (true)
            {
                var isPrime = true;
                ++number;
                var sqrt = (int)Math.Sqrt(number);
                for (var i = 2; i <= sqrt; i++)
                {
                    if (number % i == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }

                if (isPrime)
                {
                    return number;
                }
            }
        }

        #endregion
    }
}