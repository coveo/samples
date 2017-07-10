// Copyright (c) 2005-2017, Coveo Solutions Inc.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coveo.AbstractLayer.RepositoryItem;
using Coveo.AbstractLayer.Security;
using Coveo.Framework.CNL;
using Coveo.Framework.Log;
using Coveo.Framework.Processor;
using Coveo.Framework.Fields;
using Coveo.SearchProvider.Pipelines;
using Coveo.SearchProvider;
using Coveo.SearchProvider.ContentSearch;
using Sitecore.ContentSearch;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace CreateSecuredItemPreview
{
  /// <summary>
  /// <c>coveoPostItemProcessingPipeline</c> processor that creates a stripped down public copy of a marked item.
  /// </summary>
  /// <remarks>This processor assumes the HTML is using UTF8 encoding.</remarks>
  public class CreateSecuredItemPreviewProcessor : IProcessor<CoveoPostItemProcessingPipelineArgs>
  {
    private static readonly ILogger s_Logger = CoveoLogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private const string CHECKBOX_FIELD_CHECKED_VALUE = "1";
    private const string STRING_SEPARATOR_FOR_FIELDS_TO_REMOVE = ";";
    private const string EMPTY_SITECORE_SECURITY_DOMAIN = "";
    private const string NEW_METADATA_ITEM_IS_A_COPY = "ItemIsACopy";
    private const string NEW_METADATA_FIELDS_TO_REMOVE = "FieldsToRemove";
    private const string SITECORE_FIELDS_TO_HIDE_ID_LIST = "fieldstoremoveids";
    private const char CHAR_SEPARATOR_FOR_FIELDS_TO_REMOVE = ';';

    /// <summary>
    /// ID of the field where the Field Remover is activated
    /// </summary>
    public string ActivateFieldRemoverID { get; set; }

    /// <summary>
    /// ID of the field where the Preview Remover is activated
    /// </summary>
    public string ActivatePreviewRemoverID { get; set; }

    /// <summary>
    /// Suffix that will be added at the end of the duplicated item URI
    /// </summary>
    public string LimitedItemSuffix { get; set; }

    /// <inheritDoc />
    public void Process(CoveoPostItemProcessingPipelineArgs p_Args)
    {
      s_Logger.TraceEntering();

      Precondition.NotNull(p_Args, () => () => p_Args);
      CoveoIndexableItem coveoIndexableItem = p_Args.CoveoItem;
      SitecoreIndexableItem sitecoreIndexableItem = p_Args.Item as SitecoreIndexableItem;
      ID activateFieldRemoverID = new ID(ActivateFieldRemoverID);
      ID activatePreviewRemoverID = new ID(ActivatePreviewRemoverID);
      ISearchIndex searchIndex = new CoveoIndexFetcher().GetCoveoSearchIndex(sitecoreIndexableItem);
      IFieldNameTranslator translator = searchIndex.FieldNameTranslator as IFieldNameTranslator;

      bool fieldRemoverActive = false;
      bool previewRemoverActive = false;

      string newUniqueId = coveoIndexableItem.UniqueId + LimitedItemSuffix;
      string newUri = coveoIndexableItem.Uri + LimitedItemSuffix;

      byte[] hiddenButSearchableItemPreview = coveoIndexableItem.BinaryData;

      if (sitecoreIndexableItem.GetFieldById(activateFieldRemoverID).Value.ToString() == CHECKBOX_FIELD_CHECKED_VALUE) {
        fieldRemoverActive = true;
      };

      if (sitecoreIndexableItem.GetFieldById(activatePreviewRemoverID).Value.ToString() == CHECKBOX_FIELD_CHECKED_VALUE) {
        previewRemoverActive = true;
      };

      if (coveoIndexableItem != null &&
          sitecoreIndexableItem != null &&
          (fieldRemoverActive || previewRemoverActive)) {

        // Add information in metadata to spot the duplicate and to indicates which fields needs to be stripped
        Dictionary<string, object> newMetadata = new Dictionary<string, object>(coveoIndexableItem.Metadata);
        List<string> fieldsToHideList = newMetadata[SITECORE_FIELDS_TO_HIDE_ID_LIST].
            ToString()
            .Split(CHAR_SEPARATOR_FOR_FIELDS_TO_REMOVE)
            .Select(field => sitecoreIndexableItem.GetFieldById(new ID(field)))
            .Select(field => translator.TranslateToCoveoFormat(field.Name.ToString()))
            .ToList();

        newMetadata.Add(NEW_METADATA_FIELDS_TO_REMOVE, string.Join(STRING_SEPARATOR_FOR_FIELDS_TO_REMOVE, fieldsToHideList));
        newMetadata.Add(NEW_METADATA_ITEM_IS_A_COPY, CHECKBOX_FIELD_CHECKED_VALUE);

        CoveoIndexableItem strippedItem = new CoveoIndexableItem
        {
          //Modify the ID, the metadata and the URI
          UniqueId = newUniqueId,
          Metadata = newMetadata,
          Uri = newUri,
          //Add anonymous permission
          Permissions = CreateAnonymousAccessRule(),
          //Copy the rest of the original item data
          BinaryData = coveoIndexableItem.BinaryData,
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
          PrintablePath = coveoIndexableItem.PrintablePath,
          Title = coveoIndexableItem.Title,
        };
        p_Args.OutputCoveoItems.Add(strippedItem);
      };
      s_Logger.TraceExiting();
    }

    private AccessRulesHierarchy CreateAnonymousAccessRule()
    {
      IndexableAccount account = new IndexableAccount(EMPTY_SITECORE_SECURITY_DOMAIN, SecurityConstants.EVERYONE_ROLE, IndexableAccountType.Role);
      account.RoleInfo = new IndexableAccount.AccountRoleInfo(true);
      AccessRulesHierarchy accessRuleHierarchy = new AccessRulesHierarchy();
      IEnumerable<IndexableReadAccessRule> accessRules = new[] {
                new IndexableReadAccessRule() {
                    Account = account,
                    PermissionType = IndexablePermissionType.Access,
                    PropagationType = IndexablePropagationType.Entity,
                    SecurityPermission = IndexableSecurityPermission.AllowAccess
                }
            };

      accessRuleHierarchy.AddChildRules(accessRules);

      return accessRuleHierarchy;
    }

    public void OnItemDeleted(object p_Sender,
                              EventArgs p_Args)
    {
      Item item = Event.ExtractParameter<Item>(p_Args, 0);

      if (item != null) {
        CoveoIndexFetcher indexFetcher = new CoveoIndexFetcher();
        IEnumerable<IProviderIndex> providerIndexes = indexFetcher.GetCoveoSearchIndexesForDatabase(item.Database.Name);

        foreach (IProviderIndex providerIndex in providerIndexes) {
          ISearchIndex searchIndex = providerIndex as ISearchIndex;
          searchIndex.Delete(new SitecoreItemId(new ID(item.ID + LimitedItemSuffix)));
        }
      }
    }
  }
}