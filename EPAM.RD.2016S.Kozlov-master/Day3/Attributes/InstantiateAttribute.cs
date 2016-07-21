using System;

namespace Attributes
{
    // Should be applied to classes only.
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InstantiateUserAttribute : Attribute
    {
        public InstantiateUserAttribute(string name, string lastName) : this(0, name, lastName) { }

        public InstantiateUserAttribute(int id, string name, string lastName)
        {
            Id = id;
            Name = name;
            LastName = lastName;
        }

        public int Id { get; }
        public string Name { get; }
        public string LastName { get; }
    }
}
