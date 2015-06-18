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
            string sqlConnection = p_PostConversion.GetParameter("sqlconnection");
            string projectNumber = SafeGetFieldValue(p_DocumentInfo, "ProjectNumber");

            using (IDbConnection cnnUsers = new SqlConnection(sqlConnection))
            {
                cnnUsers.Open();
                using (IDbCommand cmdUsers = cnnUsers.CreateCommand())
                {
                    cmdUsers.CommandText = "SELECT userName FROM ABC_ProjectMapping WHERE ProjectNumber ='" +projectNumber+"'";
                    using (IDataReader dbReader = cmdUsers.ExecuteReader())
                    {
                        while (dbReader.Read())
                        {
                            DocumentPermissionsEx test = p_DocumentInfo.Permissions;
                            DocumentPermissionLevel level = test.AddPermissionLevel();
                            DocumentPermissionSet set = level.AddPermissionSet();
                            set.Allowed.AddPermissionWithType(dbReader["userName"].ToString(), PermissionTypeEnumeration.USER_PERMISSION_TYPE,"salesforce regex");
                        }
                    }
                }
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
