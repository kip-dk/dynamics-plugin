
namespace Kipon.Xrm.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RelationshipAttribute : IfAttribute
    {
        public RelationshipAttribute(string schemaName) : base(typeof(Implementations.RelationshipMethodCondition))
        {
            this.SchemaName = schemaName;
        }

        public string SchemaName { get; private set; }
    }
}
