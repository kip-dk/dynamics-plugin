using System;
using System.Linq;
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
    public class PlainTxtCodeWriterFilter : ICodeWriterFilterService
    {
        string[] entities;
        string[] startWith;
        //reference to the default service.
        private ICodeWriterFilterService _defaultService = null;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="defaultService">default implementation</param>
        public PlainTxtCodeWriterFilter(ICodeWriterFilterService defaultService)
        {
            this._defaultService = defaultService;
            LoadFilterData();
        }

        /// <summary>
        /// loads the entity filter data from the filter.xml file
        /// </summary>
        private void LoadFilterData()
        {
            if (!System.IO.File.Exists("filter.txt"))
            {
                throw new System.IO.FileNotFoundException("You must add a file named [filter.txt] with a line for each entity to generate.");
            }

            var lines = System.IO.File.ReadAllLines("filter.txt");
            this.entities = lines.Select(r => r.ToLower().Trim()).Where(r => !r.StartsWith("all:")).ToArray();
            this.startWith = lines.Select(r => r.ToLower().Trim()).Where(r => r.StartsWith("all:")).Select(r => r.Split(':')[1]).ToArray();
        }

        /// <summary>
        /// /Use filter entity list to determine if the entity class should be generated.
        /// </summary>
        public bool GenerateEntity(EntityMetadata entityMetadata, IServiceProvider services)
        {
            var name = entityMetadata.LogicalName.ToLowerInvariant();

            if (this.entities.Contains(name)) return true;

            return ((from a in startWith
                    where name.StartsWith(a)
                    select a).Any());
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