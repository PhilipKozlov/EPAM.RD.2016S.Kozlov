//-----------------------------------------------------------------------
// <copyright file="IGenerator.cs" company="No Company">
//     No Company. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace IDGenerator
{
    /// <summary>
    /// Defines common methods for generating id.
    /// </summary>
    /// <typeparam name="T"> Type of the generated object.</typeparam>
    public interface IGenerator<T>
    {
        /// <summary>
        /// Generates single id.
        /// </summary>
        /// <param name="prevId"> Previously generated id.</param>
        /// <returns> Id of type T.</returns>
        T GenerateId(T prevId);
    }
}
