using System;

namespace Attributes
{
    // Should be applied to properties and fields.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IntValidatorAttribute : Attribute
    {

        public IntValidatorAttribute(int firstId, int lastId)
        {
            FirstId = firstId;
            LastId = lastId;
        }

        public int FirstId { get; }
        public int LastId { get; }
    }
}
