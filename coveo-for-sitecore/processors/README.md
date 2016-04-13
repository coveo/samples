[Coveo for Sitecore](http://www.coveo.com/en/solutions/coveo-for-sitecore) processor samples.

### General usage:

 1. Add a processor node to your Coveo for Sitecore configuration file (Coveo.SearchProvider.config) in the right pipeline with the namespace, class name, assembly and required attributes.

        <processor type="YourNamespace.TheProcessorClassName, YourAssemblyName" />

 2. Rebuild your Sitecore indexes managed by Coveo for Sitecore to re-index the documents.
