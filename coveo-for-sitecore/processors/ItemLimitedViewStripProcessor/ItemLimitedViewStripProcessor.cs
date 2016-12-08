// Copyright (c) 2005-2016, Coveo Solutions Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Coveo.AbstractLayer.RepositoryItem;
using Coveo.AbstractLayer.Security;
using Coveo.Framework.CNL;
using Coveo.Framework.Log;
using Coveo.Framework.Processor;
using Coveo.SearchProvider.Pipelines;
using Sitecore.ContentSearch;
using Sitecore.Data;

namespace ItemLimitedAccess.Processors
{
    /// <summary>
    /// <c>coveoPostItemProcessingPipeline</c> processor that creates a stripped down public copy of a marked item.
    /// </summary>
    /// <remarks>This processor assumes the HTML is using UTF8 encoding.</remarks>
    public class ItemLimitedViewStripProcessor : IProcessor<CoveoPostItemProcessingPipelineArgs>
    {

        private static readonly ILogger s_Logger = CoveoLogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string LIMITED_ACCESS_VALUE = "Strip";
        private readonly string LIMITED_ACCESS_ITEM_SUFFIX = "_DuplicatedCopy";
        private readonly string LIMITED_ACCESS_METADATA_FIELDNAME = "IsLimitedAccessDocument";
        private readonly string HIDDEN_CONTENT_METADATA_FIELDNAME = "HiddenContent";

        /// <summary>
        /// ID of the field where the limited access command is specified.
        /// </summary>
        public string LimitedAccessFieldId { get; set; }

        /// <summary>
        /// ID of a field that must be stripped from the Metadata. Only single field hiding is implemented for now, but the processor can be easily modified to allow for multiple field hiding.
        /// </summary>
        /// <remarks>Can be empty.</remarks>
        public string FieldToHideId { get; set; }

        /// <summary>
        /// ID of field containing an item preview.
        /// </summary>
        /// <remarks>Can be empty.</remarks>
        public string PreviewFieldId { get; set; }

        /// <inheritDoc />
        public void Process(CoveoPostItemProcessingPipelineArgs p_Args)
        {
            s_Logger.TraceEntering();

            Precondition.NotNull(p_Args, () => () => p_Args);

            ID limitedAccessFieldId = new ID(LimitedAccessFieldId);
            ID fieldToHideId = new ID(FieldToHideId);
            ID previewFieldId = new ID(PreviewFieldId);

            CoveoIndexableItem coveoIndexableItem = p_Args.CoveoItem;
            SitecoreIndexableItem sitecoreIndexableItem = p_Args.Item as SitecoreIndexableItem;
            if (coveoIndexableItem != null &&
                sitecoreIndexableItem != null &&
                !sitecoreIndexableItem.Item.Paths.IsMediaItem &&
                sitecoreIndexableItem.Item[limitedAccessFieldId] == LIMITED_ACCESS_VALUE) {

                // Check if a preview text has been specified.
                IIndexableDataField previewField = sitecoreIndexableItem.Fields.FirstOrDefault(arg => (ID) arg.Id == previewFieldId);
                byte[] encodedPreview = null;
                if (previewField != null) {
                    string previewText = previewField.Value.ToString();
                    if (!String.IsNullOrEmpty(previewText)) {
                        encodedPreview = Encoding.UTF8.GetBytes(previewText);
                    }
                }

                // Duplicates metadata.
                Dictionary<string, object> newMetadata = new Dictionary<string, object>(coveoIndexableItem.Metadata) {
                    { LIMITED_ACCESS_METADATA_FIELDNAME, true }
                };

                // Add a hidden field containing the original binary data for relevance
                if (coveoIndexableItem.BinaryData != null) {
                    newMetadata.Add(HIDDEN_CONTENT_METADATA_FIELDNAME, Encoding.UTF8.GetString(coveoIndexableItem.BinaryData));
                }

                if (!String.IsNullOrEmpty(FieldToHideId)) {
                    IIndexableDataField fieldToHide = sitecoreIndexableItem.Fields.FirstOrDefault(arg => (ID) arg.Id == fieldToHideId);
                    if (fieldToHide != null) {
                        newMetadata.Remove(fieldToHide.Name);
                    }
                }

                string newUniqueId = coveoIndexableItem.UniqueId + LIMITED_ACCESS_ITEM_SUFFIX;

                CoveoIndexableItem strippedItem = new CoveoIndexableItem {
                    // Custom fields.
                    // Replace the data with the preview text. This way, the preview will be used for the new item's quickview.
                    BinaryData = encodedPreview,
                    UniqueId = newUniqueId,
                    Metadata = newMetadata,
                    // Fields that are inherited from the parent item.
                    BinaryDataMimeType = coveoIndexableItem.BinaryDataMimeType,
                    BinaryDataPath = coveoIndexableItem.BinaryDataPath,
                    ClickableUri = coveoIndexableItem.ClickableUri,
                    FileName = coveoIndexableItem.FileName,
                    HasSubItems = coveoIndexableItem.HasSubItems,
                    Id = coveoIndexableItem.Id,
                    IsDeletedItem = coveoIndexableItem.IsDeletedItem,
                    ModifiedDate = coveoIndexableItem.ModifiedDate,
                    Parent = coveoIndexableItem.Parent,
                    ParentId = coveoIndexableItem.ParentId,
                    Path = coveoIndexableItem.Path,
                    Permissions = CreateAnonymousAccessRule(),
                    PrintablePath = coveoIndexableItem.PrintablePath,
                    Title = coveoIndexableItem.Title
                };
                p_Args.OutputCoveoItems.Add(strippedItem);
            }
            s_Logger.TraceExiting();
        }

        private AccessRulesHierarchy CreateAnonymousAccessRule()
        {
            AccessRulesHierarchy accessRuleHierarchy = new AccessRulesHierarchy();
            IEnumerable<IndexableReadAccessRule> accessRules = new[] {
                new IndexableReadAccessRule() {
                    Account = new IndexableAccount("", SecurityConstants.EVERYONE_ROLE, IndexableAccountType.Role),
                    PermissionType = IndexablePermissionType.Access,
                    PropagationType = IndexablePropagationType.Entity,
                    SecurityPermission = IndexableSecurityPermission.AllowAccess
                }
            };
            accessRuleHierarchy.AddChildRules(accessRules);

            return accessRuleHierarchy;
        }
    }
}
