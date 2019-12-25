using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

// To acknowledge the initial author, this sources comes from
// http://erikpool.blogspot.nl/2011/03/filtering-generated-entities-with.html


namespace Kipon.Xrm.Tools.CodeWriter
 {
    /// <summary>
    /// CodeWriterFilter for CrmSvcUtil that reads list of entities from an xml file to
    /// determine whether or not the entity class should be generated.
    /// </summary>
    public class CodeWriterFilter : ICodeWriterFilterService
    {
        public static readonly Dictionary<string, Model.Entity> ENTITIES = new Dictionary<string, Model.Entity>();
        public static readonly Dictionary<string, Model.OptionSet> GLOBAL_OPTIONSET_INDEX = new Dictionary<string, Model.OptionSet>();

        //list of entity names to generate classes for.
        private Dictionary<string, Model.Entity> _validEntities = new Dictionary<string, Model.Entity>();

        //reference to the default service.
        private ICodeWriterFilterService _defaultService = null;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="defaultService">default implementation</param>
        public CodeWriterFilter(ICodeWriterFilterService defaultService)
        {
            this._defaultService = defaultService;
            LoadFilterData();
        }

        /// <summary>
        /// loads the entity filter data from the filter.xml file
        /// </summary>
        private void LoadFilterData()
        {
            XElement xml = XElement.Load("filter.xml");

            #region parse entity definitions
            {
                XElement entitiesElement = xml.Element("entities");

                var row = 0;
                foreach (XElement entityElement in entitiesElement.Elements("entity"))
                {
                    row++;
                    var uowName = entityElement.Attribute("servicename");
                    if (uowName == null)
                    {
                        throw new Exception($"No servicename on entity number {row}");
                    }

                    var logicalname = entityElement.Attribute("logicalname");
                    if (logicalname == null)
                    {
                        throw new Exception($"No logical name on entity number {row} : {uowName.Value}");
                    }

                    List<Model.OptionSet> optionsets = new List<Model.OptionSet>();
                    foreach (XElement optionset in entityElement.Elements("optionset"))
                    {
                        var optionsetLogicalname = optionset.Attribute("logicalname");
                        var optionsetName = optionset.Attribute("name");
                        var optionsetId = optionset.Attribute("id");
                        var next = new Model.OptionSet
                        {
                            Id = optionsetId?.Value,
                            Name = optionsetName.Value,
                            Logicalname = optionsetLogicalname.Value
                        };
                        if (next.Id == null)
                        {
                            var values = new List<Model.OptionSetValue>();
                            foreach (XElement optionsetValue in optionset.Elements("value"))
                            {
                                values.Add(new Model.OptionSetValue
                                {
                                    Name = optionsetValue.Attribute("name").Value,
                                    Value = int.Parse(optionsetValue.Value)
                                });
                            }
                            next.Values = values.ToArray();
                            if (next.Values.Length == 0)
                            {
                                throw new Exception($"Local optionset on {logicalname.Value} {next.Name} does not define any values");
                            }
                        }
                        optionsets.Add(next);
                    }
                    var entity = new Model.Entity
                    {
                        LogicalName = logicalname.Value,
                        ServiceName = uowName.Value,
                        Optionsets = optionsets.ToArray()
                    };
                    _validEntities.Add(entity.LogicalName, entity);
                }
            }
            #endregion

            #region parse global optionsets
            {
                XElement optionsetsElement = xml.Element("optionsets");
                var row = 0;
                foreach (XElement optionset in optionsetsElement.Elements("optionset"))
                {
                    var optionsetName = optionset.Attribute("name");
                    if (optionsetName == null)
                    {
                        throw new Exception($"Global optionset definition {row} does not have a name");
                    }
                    var optionsetId = optionset.Attribute("id");
                    if (optionsetId == null)
                    {
                        throw new Exception($"Global optionset definition {row} does not have an id");
                    }

                    if (GLOBAL_OPTIONSET_INDEX.ContainsKey(optionsetId.Value))
                    {
                        throw new Exception($"Global optionset definition {row} id is not unique");
                    }

                    var next = new Model.OptionSet
                    {
                        Id = optionsetId.Value,
                        Name = optionsetName.Value
                    };

                    var values = new List<Model.OptionSetValue>();
                    foreach (XElement optionsetValue in optionset.Elements("value"))
                    {
                        values.Add(new Model.OptionSetValue
                        {
                            Name = optionsetValue.Attribute("name").Value,
                            Value = int.Parse(optionsetValue.Value)
                        });
                    }
                    next.Values = values.ToArray();
                    if (next.Values.Length == 0)
                    {
                        throw new Exception($"Global optionset {row} does not define any values");
                    }
                    GLOBAL_OPTIONSET_INDEX.Add(next.Id, next);
                }
            }
            #endregion
        }

        /// <summary>
        /// /Use filter entity list to determine if the entity class should be generated.
        /// </summary>
        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            var generate = _validEntities.ContainsKey(entityMetadata.LogicalName.ToLowerInvariant());
            if (generate)
            {
                ENTITIES[entityMetadata.SchemaName] = _validEntities[entityMetadata.LogicalName.ToLowerInvariant()];
            }
            return generate;
        }

        //All other methods just use default implementation:

        public bool GenerateAttribute(AttributeMetadata attributeMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateAttribute(attributeMetadata, services);
        }

        public bool GenerateOption(OptionMetadata optionMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateOption(optionMetadata, services);
        }

        public bool GenerateOptionSet(OptionSetMetadataBase optionSetMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateOptionSet(optionSetMetadata, services);
        }

        public bool GenerateRelationship(RelationshipMetadataBase relationshipMetadata, EntityMetadata otherEntityMetadata, IServiceProvider services)
        {
            return _defaultService.GenerateRelationship(relationshipMetadata, otherEntityMetadata, services);
        }

        public bool GenerateServiceContext(IServiceProvider services)
        {
            return _defaultService.GenerateServiceContext(services);
        }
    }
}