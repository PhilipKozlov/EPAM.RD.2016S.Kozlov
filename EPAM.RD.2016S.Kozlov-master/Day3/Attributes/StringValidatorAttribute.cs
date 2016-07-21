using System;

namespace Attributes
{
    // Should be applied to properties and fields.
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class StringValidatorAttribute : Attribute
    {
        public StringValidatorAttribute(int length)
        {
            Length = length;
        }

        public int Length { get; }
    }
}
