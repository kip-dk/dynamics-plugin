using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Crm.Services.Utility;
using Microsoft.Xrm.Sdk.Metadata;

// To acknowledge the author, this sources comes from
// http://erikpool.blogspot.nl/2011/03/filtering-generated-entities-with.html


namespace Kipon.Dynamics.SvcFilter
{
    /// <summary>
    /// CodeWriterFilter for CrmSvcUtil that reads list of entities from an xml file to
    /// determine whether or not the entity class should be generated.
    /// </summary>
    public class CodeWriterFilter : ICodeWriterFilterService
    {
        public static Dictionary<string, string> ENTITIES = new Dictionary<string, string>();

        //list of entity names to generate classes for.
        private Dictionary<string, string> _validEntities = new Dictionary<string, string>();

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
            XElement entitiesElement = xml.Element("entities");
            foreach (XElement entityElement in entitiesElement.Elements("entity"))
            {
                var uowName = entityElement.Attribute("servicename").Value;
                _validEntities.Add(entityElement.Value.ToLowerInvariant(), uowName);
            }
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