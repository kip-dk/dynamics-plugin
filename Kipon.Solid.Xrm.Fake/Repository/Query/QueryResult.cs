using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository.Query
{
    internal class QueryResult
    {

        private Dictionary<string, EntityContainer> results = new Dictionary<string, EntityContainer>();

        internal QueryResult(Entity entity, string[] columns)
        {

            results.Add(entity.LogicalName, new EntityContainer { Entity = entity, Columns = columns });
        }

        internal QueryResult Add(string alias, Entity entity, string[] columns)
        {
            results.Add(alias, new EntityContainer { Alias = alias, Entity = entity, Columns = columns });
            return this;
        }

        internal Entity this[string alias]
        {
            get
            {
                return results[alias].Entity;
            }
        }

        internal object this[string alias, string attribute]
        {
            get
            {
                var ent = results[alias];
                if (ent.Entity != null)
                {
                    return ent.Entity[attribute];
                }
                return null;
            }
        }

        internal class EntityContainer
        {
            internal string Alias { get; set; }
            internal Entity Entity { get; set; }
            internal string[] Columns { get; set; }
        }

    }
}
