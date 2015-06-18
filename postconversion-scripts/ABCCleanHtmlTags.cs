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
    public class ABCCleanHTMLTags : CustomConverter
    {
        public override void RunPostConverter(Coveo.CES.Interops.COMCoveoConvertersWrappers.PostConversion p_PostConversion, Coveo.CES.Interops.COMCoveoConvertersWrappers.DocumentInfo p_DocumentInfo)
        {
			string body = p_PostConversion.Text.ReadString(p_PostConversion.Text.CharsCount);
            string result = Regex.Replace(body, @"<[^>]*>", String.Empty);
            p_PostConversion.TextToOverride.WriteString(result);				
        }

        
    }
}
