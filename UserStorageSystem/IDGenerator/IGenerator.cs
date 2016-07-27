namespace IDGenerator
{
    /// <summary>
    /// Defines common methods for generating id.
    /// </summary>
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
