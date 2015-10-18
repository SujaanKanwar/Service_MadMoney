using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace MadMoney.DataBaseTools
{
    public class APKDBTool
    {
        private static string DB_CON_NAME = "MadMoneyUser";
        SqlConnection CON = null;
        private string APKSTORE_TABLE_NAME = "APKStore";

        public APKDBTool()
        {
            CON = new SqlConnection(ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString());
        }

        public void Store(List<APKNode> list)
        {
            string jsonString = JsonSerializer(list);
            string spName = "sp_APKData_Insert_Update";
            try
            {
                SqlCommand cmd = new SqlCommand(spName, CON);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Data", jsonString);
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }


            //store in Db
        }

        public List<APKNode> Retrive()
        {
            List<APKNode> list = null;
            string dbString = "";

            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT Document from " + APKSTORE_TABLE_NAME + " WHERE [id] = 1";

            cmd.Connection = CON;

            CON.Open();
            using (var reader = cmd.ExecuteReader())
            {
                if(reader.Read())
                dbString = reader.GetSqlValue(0).ToString();
            }
            CON.Close();

            if (!string.IsNullOrEmpty(dbString) && dbString.CompareTo("null")!=0)
                list = JsonDeserialize<List<APKNode>>(dbString);

            return list;
        }

        private string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            MemoryStream ms = new MemoryStream();

            ser.WriteObject(ms, t);

            string jsonString = Encoding.UTF8.GetString(ms.ToArray());

            ms.Close();

            return jsonString;
        }

        private T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            T obj = (T)ser.ReadObject(ms);

            return obj;
        }
    }
}