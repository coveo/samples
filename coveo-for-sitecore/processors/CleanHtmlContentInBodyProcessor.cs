using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Coveo.AbstractLayer.FieldManagement;
using Coveo.AbstractLayer.RepositoryItem;
using Coveo.Framework.CNL;
using Coveo.Framework.Log;
using Coveo.Framework.Processor;
using Coveo.SearchProvider.Pipelines;

// CleanHtmlContentInBodyProcessor
//
// Problem:
//   The rendered HTML version of a Sitecore item usually contains a header,
//   footer, navigation elements and sidebars whose text is polluting the
//   search index and reducing the relevancy of search results.
//
// Solution:
//   This processor is meant to be used when you already have a processor
//   grabbing the rendered HTML version of a Sitecore item. It can be used to
//   remove the undesirable parts of the HTML before indexing the item in the
//   search index.
//
// Usage:
//   1. Add the following processor node after your existing HTML grabbing
//      processor in your Coveo for Sitecore configuration file
//      (Coveo.SearchProvider.config) or even better, to a patch file:
//
//      <configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
//        <sitecore>
//          <pipelines>
//            <coveoPostItemProcessingPipeline>
//              <!-- Your existing HTML grabbing processor -->
//              <processor type="Coveo.For.Sitecore.Samples.Processors.CleanHtmlContentInBodyProcessor, Coveo.For.Sitecore.Samples">
//                <StartCommentText>BEGIN NOINDEX</StartCommentText>
//                <EndCommentText>END NOINDEX</EndCommentText>
//              </processor>
//            </coveoPostItemProcessingPipeline>
//          </pipelines>
//        </sitecore>
//      </configuration>
//
//   2. Add comment elements in your layouts, sublayouts and views around the
//      HTML markup you want to exclude from the indexed documents:
//
//      <body>
//        <!-- BEGIN NOINDEX -->
//          <header>...</header>
//        <!-- END NOINDEX -->
//
//        <div class="main-content">...</div>
//
//        <!-- BEGIN NOINDEX -->
//          <footer>...</footer>
//        <!-- END NOINDEX -->
//
//        <script src="IncludedScript.js"></script>
//
//        <!-- BEGIN NOINDEX -->
//          <script src="ExcludedScript.js"></script>
//        <!-- END NOINDEX -->
//      </body>
//
//   3. Rebuild your Sitecore indexes managed by Coveo for Sitecore to
//      index the cleaned HTML content.

namespace Coveo.For.Sitecore.Samples.Processors
{
    /// <summary>
    /// <c>coveoPostItemProcessingPipeline</c> processor that cleans the HTML
    /// content from undesirable sections.
    /// </summary>
    /// <remarks>This processor assumes the HTML is using UTF8 encoding.</remarks>
    public class CleanHtmlContentInBodyProcessor : IProcessor<CoveoPostItemProcessingPipelineArgs>
    {
        private static readonly ILogger s_Logger = CoveoLogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Gets or sets the text of the HTML comment element where to start removing markup.
        /// </summary>
        /// <remarks>Valid comment characters are a-z, A-Z, 0-9 and space.</remarks>
        public string StartCommentText { get; set; }

        /// <summary>
        /// Gets or sets the text of the HTML comment element where to stop removing markup.
        /// </summary>
        /// <remarks>Valid comment characters are a-z, A-Z, 0-9 and space.</remarks>
        public string EndCommentText { get; set; }

        /// <summary>
        /// Cleans the Coveo indexable item's HTML content set by previous processors.
        /// </summary>
        /// <param name="p_Args">The pipeline arguments in which the original HTML content is defined.</param>
        public void Process(CoveoPostItemProcessingPipelineArgs p_Args)
        {
            s_Logger.TraceEntering();
            Precondition.NotNull(p_Args, () => () => p_Args);

            if (ShouldProcess(p_Args)) {
                string originalHtmlContent = Encoding.UTF8.GetString(p_Args.CoveoItem.BinaryData);
                string cleanedHtmlContent = CleanHtmlContent(originalHtmlContent);
                p_Args.CoveoItem.BinaryData = Encoding.UTF8.GetBytes(cleanedHtmlContent);
            }

            s_Logger.TraceExiting();
        }

        /// <summary>
        /// Gets whether to run the processor.
        /// </summary>
        /// <param name="p_Args">The pipeline arguments in which the original HTML content is defined.</param>
        /// <returns><c>true</c> if the processor should be run, <c>false</c> otherwise.</returns>
        private bool ShouldProcess(CoveoPostItemProcessingPipelineArgs p_Args)
        {
            return p_Args.CoveoItem != null &&
                   p_Args.CoveoItem.BinaryData != null &&
                   p_Args.CoveoItem.BinaryData.LongLength > 0 &&
                   ArePropertiesCorrectlyConfigured() &&
                   ItemHasLayout(p_Args.CoveoItem);
        }

        /// <summary>
        /// Gets whether the processor's start and end comment text are correctly set and valid.
        /// </summary>
        /// <returns><c>true</c> if the properties are correctly configured, <c>false</c> otherwise.</returns>
        private bool ArePropertiesCorrectlyConfigured()
        {
            return IsValidCommentText(StartCommentText) &&
                   IsValidCommentText(EndCommentText);
        }

        /// <summary>
        /// Gets whether a comment text is valid.
        /// </summary>
        /// <param name="p_CommentText">The text to validate.</param>
        /// <returns><c>true</c> if the comment text is valid, <c>false</c> otherwise.</returns>
        private bool IsValidCommentText(string p_CommentText)
        {
            return !String.IsNullOrEmpty(p_CommentText) &&
                   !String.IsNullOrEmpty(p_CommentText.Trim()) &&
                   p_CommentText.All(IsValidCommentCharacter);
        }

        /// <summary>
        /// Gets whether a comment character is valid.
        /// </summary>
        /// <remarks>Valid comment characters are a-z, A-Z, 0-9 and space.</remarks>
        /// <param name="p_CommentCharacter">The charcacter to validate.</param>
        /// <returns><c>true</c> if the character is a valid comment character, <c>false</c> otherwise.</returns>
        private bool IsValidCommentCharacter(char p_CommentCharacter)
        {
            return char.IsLetterOrDigit(p_CommentCharacter) ||
                   p_CommentCharacter == ' ';
        }

        /// <summary>
        /// Gets whether the item to index has a layout.
        /// </summary>
        /// <param name="p_Item">The item to index.</param>
        /// <returns><c>true</c> if the item has a layout, <c>false</c> otherwise.</returns>
        private bool ItemHasLayout(CoveoIndexableItem p_Item)
        {
            bool itemHasLayout = false;
            object hasLayoutMetadata;
            if (p_Item.Metadata.TryGetValue(MetadataNames.s_HasLayout, out hasLayoutMetadata)) {
                itemHasLayout = hasLayoutMetadata.Equals("1");
            }

            return itemHasLayout;
        }

        /// <summary>
        /// Cleans HTML content from markup between specific HTML comment elements.
        /// </summary>
        /// <param name="p_HtmlContent">The HTML content to clean.</param>
        /// <returns>The cleaned HTML content.</returns>
        private string CleanHtmlContent(string p_HtmlContent)
        {
            return Regex.Replace(p_HtmlContent, @"<!--\s*" + StartCommentText + @"\s*-->.*?<!--\s*" + EndCommentText + @"\s*-->", "", RegexOptions.Singleline);
        }
    }
}
