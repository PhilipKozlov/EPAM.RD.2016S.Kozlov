using System;

namespace Attributes
{
    // Should be applied to .ctors.
    // Matches a .ctor parameter with property. Needs to get a default value from property field, and use this value for calling .ctor.
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
    public class MatchParameterWithPropertyAttribute : Attribute
    {

        public MatchParameterWithPropertyAttribute(string parameterName, string propertyName)
        {
           PropertyName = propertyName;
           ParameterName = parameterName;
        }

        public string PropertyName { get; }
        public string ParameterName { get; }
    }
}
