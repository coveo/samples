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
    public class ABCCaseSecurity : CustomConverter
    {
        public override void RunPostConverter(Coveo.CES.Interops.COMCoveoConvertersWrappers.PostConversion p_PostConversion, Coveo.CES.Interops.COMCoveoConvertersWrappers.DocumentInfo p_DocumentInfo)
        {
          
            string contactemail = SafeGetFieldValue(p_DocumentInfo, "contactemail");

            if (!String.IsNullOrEmpty(contactemail))
            {
                p_DocumentInfo.Permissions.Clear();
                DocumentPermissionsEx permissions = p_DocumentInfo.Permissions;
                DocumentPermissionLevel level = permissions.AddPermissionLevel();
                DocumentPermissionSet set = level.AddPermissionSet();
                string[] email = contactemail.Split('@');
                set.Allowed.AddPermissionWithType(contactemail, PermissionTypeEnumeration.USER_PERMISSION_TYPE,"Email Security Provider");
                set.Allowed.AddPermissionWithType("*@"+email[1], PermissionTypeEnumeration.GROUP_PERMISSION_TYPE,"Email Security Provider");
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

