using Coveo.CES.DotNetConverterLoader;
using Coveo.CES.Interops.COMCoveoConvertersWrappers;
using System.Text;
using System;

namespace IDSA.CES.CustomConverters.Scripts
{
    class AllFieldValues : CustomConverter
    {
        //This script can be run as a Pre-Conversion script
        public override void RunPreConverter(PreConversion p_PreConversion, DocumentInfo p_DocumentInfo)
        {
            ConversionLogger logger = new ConversionLogger(p_PreConversion);
            try
            {
                base.RunPreConverter(p_PreConversion, p_DocumentInfo);
                SetAllFieldValues(p_DocumentInfo);
            }
            catch (Exception ex)
            {
                logger.Log(typeof(AllFieldValues), log4net.Core.Level.Error, "Unhandled exception", ex);
                throw ex;
            }
        }

        //This script can be run as an Open Converter script
        public override void RunCustomConverter(CustomConversion p_CustomConversion, DocumentInfo p_DocumentInfo)
        {
            ConversionLogger logger = new ConversionLogger(p_CustomConversion);
            try
            {
                base.RunCustomConverter(p_CustomConversion, p_DocumentInfo);
                SetAllFieldValues(p_DocumentInfo);
            }
            catch (Exception ex)
            {
                logger.Log(typeof(AllFieldValues), log4net.Core.Level.Error, "Unhandled exception", ex);
                throw ex;
            }
        }

        //This script can be run as a Post-Conversion script
        public override void RunPostConverter(PostConversion p_PostConversion, DocumentInfo p_DocumentInfo)
        {
            ConversionLogger logger = new ConversionLogger(p_PostConversion);
            try
            {
                base.RunPostConverter(p_PostConversion, p_DocumentInfo);
                SetAllFieldValues(p_DocumentInfo);
            }
            catch (Exception ex)
            {
                logger.Log(typeof(AllFieldValues), log4net.Core.Level.Error, "Unhandled exception", ex);
                throw ex;
            }
        }

        private void SetAllFieldValues(DocumentInfo p_DocumentInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<AllFieldValues>");
            foreach (string metadataName in p_DocumentInfo.Fields())
            {
                sb.AppendFormat("<Field name=\"{0}\" value=\"{1}\"/>{2}", 
                    metadataName, 
                    p_DocumentInfo.GetFieldValue(metadataName), 
                    Environment.NewLine);
            }
            sb.AppendLine("</AllFieldValues>");
            p_DocumentInfo.SetFieldValue("AllFieldValues", sb.ToString());
        }
    }
}
