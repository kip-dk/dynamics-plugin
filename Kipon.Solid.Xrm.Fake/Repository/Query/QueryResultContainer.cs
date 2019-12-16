using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository.Query
{
    internal class QueryResultContainer
    {
        private Dictionary<string, Microsoft.Xrm.Sdk.Query.ColumnSet> columns = new Dictionary<string, Microsoft.Xrm.Sdk.Query.ColumnSet>();
        private EntityContainer[] Entities;
        private string logicalName;

        internal QueryResultContainer(Microsoft.Xrm.Sdk.Query.ColumnSet columns, EntityShadow[] initialEntities, string logicalName)
        {
            this.columns.Add("", columns);
            this.logicalName = logicalName;
            this.Entities = initialEntities.Select(r => new EntityContainer(r)).ToArray();
        }

        internal void ApplyFilter(string alias, Microsoft.Xrm.Sdk.Query.FilterExpression criteria)
        {
            if (criteria != null && criteria.Conditions != null && criteria.Conditions.Count > 0)
            {
                this.Entities = (from e in this.Entities
                                 where e.Match(alias, criteria.FilterOperator, criteria.Conditions)
                                 select e).ToArray();
            }
        }

        internal Microsoft.Xrm.Sdk.EntityCollection ToEntities(bool distinct, Microsoft.Xrm.Sdk.DataCollection<Microsoft.Xrm.Sdk.Query.OrderExpression> orderby)
        {
            if (distinct)
            {
                this.Entities = this.Entities.Distinct().ToArray();
            }

            if (orderby != null && orderby.Count > 0)
            {
                throw new NotSupportedException("orderby not supported yet");
            }

            var result = new List<Microsoft.Xrm.Sdk.Entity>();

            foreach (var ent in this.Entities)
            {
                var entity = new Microsoft.Xrm.Sdk.Entity { Id = ent.Id, LogicalName = ent.LogicalName };

                result.Add(entity);
                foreach (var cdefkey in columns.Keys)
                {
                    var cdef = columns[cdefkey];
                    if (cdef != null)
                    {
                        var data = ent[cdefkey];
                        if (data != null)
                        {
                            if (cdef.AllColumns)
                            {
                                foreach (var key in data.Keys)
                                {
                                    if (cdefkey == string.Empty)
                                    {
                                        var fieldname = key;
                                        entity[fieldname] = data.ValueOf(key);
                                    }
                                    else
                                    {
                                        var fieldname = cdefkey + "." + key;
                                        // this should be an attribute wrapper
                                        new Microsoft.Xrm.Sdk.AliasedValue();
                                        entity[fieldname] = data.ValueOf(key);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var r = new Microsoft.Xrm.Sdk.EntityCollection(result);
            r.EntityName = this.logicalName;
            r.TotalRecordCount = result.Count;
            return r;
        }
    }
}
