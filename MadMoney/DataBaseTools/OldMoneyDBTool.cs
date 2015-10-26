using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MadMoney.ServiceData;

namespace MadMoney.DataBaseTools
{
    public class OldMoneyDBTool
    {
         private static string DB_CON_NAME = "MadMoneyUser";
        SqlConnection CON = null;
        private string APKSTORE_TABLE_NAME = "OldMoney";

        public OldMoneyDBTool()
        {
            string CONNECTION_STR = ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString();
            CON = new SqlConnection(CONNECTION_STR);
        }

        public void Store(Money money)
        {
            string spName = "sp_OldMoney_Insert";
            try
            {
                SqlCommand cmd = new SqlCommand(spName, CON);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", money.id);
                cmd.Parameters.AddWithValue("@Value", money.value);
                cmd.Parameters.AddWithValue("@OwnerId", money.ownerId);                
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }


            //store in Db
        }

        public bool CheckPreviousMoneyEntry(Money money)
        {
            bool result = false;
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT 1 FROM " + APKSTORE_TABLE_NAME + " WHERE [Id] = '" + money.id +"'";
            cmd.Connection = CON;
            CON.Open();

            using (var reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                    if (reader.HasRows)                    
                        result= true;  
            }
            CON.Close();
            return result;
        }
    }
}