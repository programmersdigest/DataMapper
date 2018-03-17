using System;

namespace programmersdigest.DataMapper.Attributes {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class NameAttribute : Attribute {
        public string Name { get; }

        public NameAttribute(string name) {
            Name = name;
        }
    }
}
