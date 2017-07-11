[Coveo for Sitecore](http://www.coveo.com/en/solutions/coveo-for-sitecore)

These instructions will help you compile and install the processor (step 2 and 3 of the CreateSecuredItemPreview global installation). Please refer to the [blog post](ToUpdate) for pre and post steps.

1. Copy the following .dlls to the lib folder

    * Coveo.AbstractLayer.dll
    * Coveo.Framework.dll
    * Coveo.SearchProvider.dll
    * Coveo.SearchProviderBase.Rest.dll
    * Coveo.SearchProviderBase.dll
    * Sitecore.ContentSearch.dll
    * Sitecore.Kernel.dll

2. Compile the CreateSecuredItemPreviewProcessor
3. Copy CreateSecuredItemPreview.dll to your [your website repository]\Website\bin
4. Copy the CreateSecuredItemPreview.config to [your website repository]\Website\App_Config\Include\Coveo