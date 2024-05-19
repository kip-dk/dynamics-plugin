using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Fake.Repository.Query
{
    internal class QueryResultContainer
    {
        private Dictionary<string, AliasContainer> columns = new Dictionary<string, AliasContainer>();
        private EntityContainer[] Entities;
        private string logicalName;

        internal QueryResultContainer(Microsoft.Xrm.Sdk.Query.ColumnSet columns, EntityShadow[] initialEntities, string logicalName)
        {
            this.columns.Add("", new AliasContainer { Alias = string.Empty, LogicalName = logicalName, ColumnSet = columns });
            this.logicalName = logicalName;
            this.Entities = initialEntities.Select(r => new EntityContainer(r)).ToArray();
        }

        internal void ApplyFilter(string alias, Microsoft.Xrm.Sdk.Query.FilterExpression criteria)
        {
            if (criteria != null && criteria.Conditions != null && criteria.Conditions.Count > 0)
            {

                this.Entities = (from e in this.Entities
                                 where e.Match(alias, criteria.FilterOperator, criteria.Conditions, criteria.Filters)
                                 select e).ToArray();
            }

            if (criteria != null && criteria.Filters != null && criteria.Filters.Count > 0)
            {
                foreach (var filter in criteria.Filters)
                {
                    this.ApplyFilter(alias, filter);
                }
            }
        }

        internal void LinkEntity(string fromAlias, Microsoft.Xrm.Sdk.Query.LinkEntity link, EntityShadow[] allEntities)
        {
            var relevant = (from a in allEntities where a.LogicalName == link.LinkToEntityName && a[link.LinkToAttributeName] != null select a).ToArray();

            columns.Add(link.EntityAlias, new AliasContainer
            {
                Alias = link.EntityAlias,
                LogicalName = link.LinkToEntityName,
                ColumnSet = link.Columns
            });

            switch(link.JoinOperator)
            {
                case Microsoft.Xrm.Sdk.Query.JoinOperator.Inner:
                    {
                        // First remove all thouse in current resultset that does not have the reference field
                        this.Entities = (from e in this.Entities where e[fromAlias] != null && e[fromAlias][link.LinkFromAttributeName] != null select e).ToArray();

                        this.Entities = (from e in this.Entities
                                         join r in relevant on e[fromAlias].KeyOf(link.LinkFromAttributeName) equals r.KeyOf(link.LinkToAttributeName)
                                         select e.Add(link.EntityAlias, r)).ToArray();

                        break;
                    }
                default:
                    throw new NotImplementedException($"Join operation {link.JoinOperator} not implemented");
            }

            if (link.LinkCriteria != null)
            {
                if (link.LinkCriteria.Conditions != null && link.LinkCriteria.Conditions.Count > 0)
                {
                    throw new NotImplementedException($"LinkCriteria.Conditions not implemented");
                }

                if (link.LinkCriteria.Filters != null && link.LinkCriteria.Filters.Count > 0)
                {
                    throw new NotImplementedException($"LinkCriteria.Filters not implemented");
                }
            }

            if (link.LinkEntities != null && link.LinkEntities.Count > 0)
            {
                throw new NotImplementedException("Nested LinkEntities not implemented");
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
                            if (cdef.ColumnSet == null)
                            {
                                continue;
                            }

                            if (cdef.ColumnSet.AllColumns)
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
                                        entity[fieldname] = new Microsoft.Xrm.Sdk.AliasedValue(cdef.LogicalName, fieldname, data.ValueOf(key));
                                    }
                                }
                            } else
                            {
                                foreach (var key in cdef.ColumnSet.Columns)
                                {
                                    if (cdefkey == string.Empty)
                                    {
                                        var fieldname = key;
                                        entity[fieldname] = data.ValueOf(key);
                                    }
                                    else
                                    {
                                        var fieldname = cdefkey + "." + key;
                                        entity[fieldname] = new Microsoft.Xrm.Sdk.AliasedValue(cdef.LogicalName, fieldname, data.ValueOf(key));
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

        internal class AliasContainer
        {
            internal string Alias { get; set; }
            internal string LogicalName { get; set; }
            internal Microsoft.Xrm.Sdk.Query.ColumnSet ColumnSet { get; set; }
        }
    }
}
