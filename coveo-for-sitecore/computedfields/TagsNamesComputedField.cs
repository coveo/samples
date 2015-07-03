using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coveo.Framework.CNL;
using Coveo.Framework.Databases;
using Coveo.Framework.Items;
using Coveo.Framework.Log;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Globalization;

// TagsNamesComputedField
// Usage:
//   1. Add the following fieldType and field nodes to your Coveo for Sitecore
//      configuration file (Coveo.SearchProvider.config).
//
//      <configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
//        <sitecore>
//          <contentSearch>
//            <configuration type="Coveo.SearchProvider.Configuration.CoveoSearchConfiguration, Coveo.SearchProvider">
//              <defaultIndexConfiguration type="Coveo.Framework.Configuration.CoveoIndexConfiguration, Coveo.Framework">
//                <fieldMap type="Coveo.SearchProvider.CoveoFieldMap, Coveo.SearchProvider">
//                  <fieldNames hint="raw:AddFieldByFieldName">
//                    <!-- This fieldType node must be added to set the tagsnames field as a multi-value facet field in the CES field sets. -->
//                    <fieldType fieldName="tagsnames" isMultiValue="true" settingType="Coveo.Framework.Configuration.FieldConfiguration, Coveo.Framework" />
//                  </fieldNames>
//                </fieldMap>
//                <fields hint="raw:AddComputedIndexField">
//                  <!-- This field node must be added to create the tagsnames computed field.
//                       Change the namespace and assembly names according to the project you add this class to. -->
//                  <field fieldName="tagsnames">Coveo.For.Sitecore.Samples.ComputedFields.TagsNamesComputedField, Coveo.For.Sitecore.Samples</field>
//                </fields>
//              </defaultIndexConfiguration>
//            </configuration>
//          </contentSearch>
//        </sitecore>
//      </configuration>
//
//   2. Rebuild your Sitecore indexes managed by Coveo for Sitecore to
//      synchronize the fields configuration, fill the field in documents and
//      populate the CES cache for the tagsnames facet field.

namespace Coveo.For.Sitecore.Samples.ComputedFields
{
    /// <summary>
    /// A Coveo for Sitecore computed field to index the names of the tags
    /// associated to a Sitecore item.
    /// </summary>
    public class TagsNamesComputedField : IComputedIndexField
    {
        private static readonly ILogger s_Logger = CoveoLogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string[] s_SemanticsValueSeparators = { SEMANTICS_FIELD_VALUE_SEPARATOR };

        [ThreadStatic]
        private static IDatabaseWrapper s_Database;

        private const string SEMANTICS_FIELD_NAME = "__semantics";
        private const string SEMANTICS_FIELD_VALUE_SEPARATOR = "|";

        /// <inheritdoc />
        public string FieldName { get; set; }

        /// <inheritdoc />
        public string ReturnType
        {
            get
            {
                return "string";
            }
            set
            {
            }
        }

        /// <inheritdoc />
        public object ComputeFieldValue(IIndexable p_Indexable)
        {
            s_Logger.TraceEntering("ComputeFieldValue");
            Precondition.NotNull(p_Indexable, () => () => p_Indexable);

            IItem item = new ItemWrapper(new IndexableWrapper(p_Indexable));

            object value = GetItemTagsNames(item);

            s_Logger.TraceExiting("ComputeFieldValue");
            return value;
        }

        /// <summary>
        /// Gets the name of the tags associated to an <see cref="IItem"/>.
        /// </summary>
        /// <param name="p_Item">The item that is being indexed.</param>
        /// <returns>A list of tag names.</returns>
        private object GetItemTagsNames(IItem p_Item)
        {
            s_Logger.TraceEntering("GetItemTagsName");
            Precondition.NotNull(p_Item, () => () => p_Item);
            s_Logger.Debug(p_Item.Uri.DataUri);

            s_Database = p_Item.Database;

            object tagsNames = null;
            IEnumerable<string> tagItemsIds = GetTagItemsIds(p_Item);
            if (tagItemsIds.Any()) {
                tagsNames = GetTagItemsNames(tagItemsIds, p_Item.Language);
            }

            s_Logger.Debug("Value: " + tagsNames);
            s_Logger.TraceExiting("GetItemTagsName");
            return tagsNames;
        }

        /// <summary>
        /// Gets the IDs of the tag items associated to an <see cref="IItem"/>.
        /// </summary>
        /// <param name="p_Item">The item that is being indexed.</param>
        /// <returns>A collection of IDs.</returns>
        private IEnumerable<string> GetTagItemsIds(IItem p_Item)
        {
            IEnumerable<string> tagItemsIds = new List<string>();
            string semanticsFieldValue = p_Item.GetFieldValue(SEMANTICS_FIELD_NAME);

            if (!String.IsNullOrEmpty(semanticsFieldValue)) {
                tagItemsIds = semanticsFieldValue.Split(s_SemanticsValueSeparators, StringSplitOptions.RemoveEmptyEntries);
            }
            return tagItemsIds;
        }

        /// <summary>
        /// Gets the names of tag items from their ID.
        /// </summary>
        /// <param name="p_TagItemsIds">The IDs of tag items.</param>
        /// <param name="p_SourceItemLanguage">The language of the indexed item used to get the tag names.</param>
        /// <returns>A collection of tag names.</returns>
        private List<string> GetTagItemsNames(IEnumerable<string> p_TagItemsIds,
                                              Language p_SourceItemLanguage)
        {
            s_Logger.TraceEntering("ComputeStringValue");

            List<string> tagsNames = new List<string>();
            foreach (string tagItemId in p_TagItemsIds) {
                string itemName = GetItemName(tagItemId, p_SourceItemLanguage);

                if (!String.IsNullOrEmpty(itemName)) {
                    tagsNames.Add(itemName);
                }
            }

            s_Logger.TraceExiting("ComputeStringValue");

            return tagsNames.Any() ? tagsNames : null;
        }

        /// <summary>
        /// Gets the name of an item from its ID.
        /// </summary>
        /// <param name="p_Id">The ID of the item.</param>
        /// <param name="p_SourceItemLanguage">The language of the indexed item used to get the item name.</param>
        /// <returns>The name of the item if found, <c>null</c> otherwise.</returns>
        private string GetItemName(string p_Id,
                                   Language p_SourceItemLanguage)
        {
            string itemName = null;

            IItem item = ResolveReferencedItem(p_Id, p_SourceItemLanguage);
            if (item != null) {
                itemName = item.Name;
            }
            return itemName;
        }

        /// <summary>
        /// Gets an item from its ID.
        /// </summary>
        /// <param name="p_Id">The ID of the item.</param>
        /// <param name="p_SourceItemLanguage">The language of the indexed item used to get the item.</param>
        /// <returns>The item in the same language as the indexed item if found, the item in the default language otherwise. <c>null</c> if the item doesn't exist.</returns>
        private IItem ResolveReferencedItem(string p_Id,
                                            Language p_SourceItemLanguage)
        {
            IItem item = s_Database.GetItem(p_Id, p_SourceItemLanguage);

            // When an item does exist but not in the requested language, Sitecore returns an
            // incomplete item. We must then verify if the item
            // has a version to know whether it is a real item or not.
            if (item == null || !item.Versions.HasVersion()) {
                item = s_Database.GetItem(p_Id);
            }

            return item;
        }
    }
}