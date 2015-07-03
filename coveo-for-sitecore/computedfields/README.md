[Coveo for Sitecore](http://www.coveo.com/en/solutions/coveo-for-sitecore) computed field samples.

### General usage:

 1. Add a fieldType node to your Coveo for Sitecore configuration file (Coveo.SearchProvider.config) &lt;fields hint="raw:AddComputedIndexField"&gt; section with the computed field name, namespace, class name, assembly and required attributes.

        <field fieldName="YourFieldName">YourNamespace.TheComputedFieldClassName, YourAssemblyName</field>

 2. Rebuild your Sitecore indexes managed by Coveo for Sitecore to synchronize the fields configuration, fill the field in documents and populate the CES cache for the tagsnames facet field.