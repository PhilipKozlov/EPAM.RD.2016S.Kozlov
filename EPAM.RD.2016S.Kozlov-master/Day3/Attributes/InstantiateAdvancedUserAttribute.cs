using System;

namespace Attributes
{
    // Should be applied to assembly only.
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class InstantiateAdvancedUserAttribute : InstantiateUserAttribute
    {

        public InstantiateAdvancedUserAttribute(int id, string name, string lastName, int externalId) : base(id, name, lastName)
        {
            ExternalId = externalId;
        }

        public int ExternalId { get; }
    }
}
