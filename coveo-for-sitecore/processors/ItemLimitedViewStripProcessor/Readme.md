1. Copy the following .dlls to the lib folder
	-Coveo.AbstractLayer.dll
	-Coveo.Framework.dll
	-Coveo.SearchProviderBase.dll
	-Sitecore.ContentSearch.dll
	-Sitecore.Kernel.dll
2. Compile the Item Limited Access processor.
3. Copy ItemLimitedViewStripProcessor.dll to your bin
4. Patch your coveo config files with the content of ItemLimitedAccess.config (you can copy it to [your website repository]\Website\App_Config\Include\Coveo).
5. Patch your SearchView.cshtml (if you're using MVC) with the content of SearchViewSnippet.txt
