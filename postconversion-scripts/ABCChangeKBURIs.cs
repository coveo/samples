using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Coveo.CES.DotNetConverterLoader;
using Coveo.CES.Interops.COMCoveoConvertersWrappers;
using System.Data;


namespace ABC.CustomConverters
{
    public class ABCProjectsSecurity : CustomConverter
    {
        public override void RunPostConverter(Coveo.CES.Interops.COMCoveoConvertersWrappers.PostConversion p_PostConversion, Coveo.CES.Interops.COMCoveoConvertersWrappers.DocumentInfo p_DocumentInfo)
        {
         
            string objecttype = SafeGetFieldValue(p_DocumentInfo, "objecttype");

            if (objecttype == "KBBlog")
            {
                uri = "https://ABC.com/Blog/"+p_DocumentInfo.Title.Replace(" ", "-");
                p_DocumentInfo.ClickableURI = uri;
                p_DocumentInfo.PrintableURI = uri;
            }
        }

        public static string SafeGetFieldValue(DocumentInfo p_DocumentInfo, string p_FieldName)
        {
            try
            {
                return p_DocumentInfo.GetFieldValue(p_FieldName).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
