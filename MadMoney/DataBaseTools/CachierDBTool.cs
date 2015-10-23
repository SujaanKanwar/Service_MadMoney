using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using MadMoney.MoneyBank;

namespace MadMoney
{
    public class CachierDBTool
    {
        private string[] MONEY_TABLE_NAME_ARRAY = new string[] { "One", "Two", "Five", "Ten", "Twenty", "Fifty", "Hundred", "FiveHundred", "Thousand" };
        private static string DB_CON_NAME = "MadMoneyUser";
        private static string USER_AC_TABLE = "UsersAccounts";
        private string CONNECTION_STR;
        SqlConnection CON = null;

        public CachierDBTool()
        {
            CONNECTION_STR = ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString();
            CON = new SqlConnection(CONNECTION_STR);
        }

        public void DepositMoneyInUserAC(Dictionary<int, List<Money>> moneyDictionary, string userAddressId)
        {
            UsersDBTool userDBTool = new UsersDBTool();
            int uid;
            int.TryParse(userDBTool.ValidateUsers(userAddressId), out uid);

            string spName = "sp_UsersAccounts_Insert";
            try
            {                                
                SqlCommand cmd;                
                CON.Open();
                foreach (var moneyList in moneyDictionary.Values)
                {                    
                    for (var i = 0; i < moneyList.Count; i++)
                    {
                        cmd = new SqlCommand(spName, CON);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", moneyList[i].id);
                        cmd.Parameters.AddWithValue("@Value", moneyList[i].value);
                        cmd.Parameters.AddWithValue("@Hash", moneyList[i].hash);
                        cmd.Parameters.AddWithValue("@OwnerId", moneyList[i].ownerId);
                        cmd.Parameters.AddWithValue("@Dated", moneyList[i].dated);
                        cmd.Parameters.Add("@Signature", SqlDbType.Binary).Value = moneyList[i].signature;
                        cmd.Parameters.AddWithValue("@UserId", uid);
                        cmd.ExecuteNonQuery();                        
                    }                    
                }

            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }

        public List<ServiceData.Money> GetMoneyFromUserAccount(string userAddressId, string uid)
        {
            List<ServiceData.Money> moneyList = new List<ServiceData.Money>();
            ServiceData.Money money;
            CON = new SqlConnection(CONNECTION_STR);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM " + USER_AC_TABLE + " WHERE UserId = " + uid + " AND Status = " + 0;
            cmd.Connection = CON;
            CON.Open();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    int value;
                    while (reader.Read())
                    {
                        money = new ServiceData.Money();
                        money.id = reader["Id"].ToString();
                        int.TryParse(reader["Value"].ToString(), out value);
                        money.value = value;
                        money.hash = reader["Hash"].ToString();
                        money.ownerId = reader["OwnerId"].ToString();
                        money.dated = reader["Dated"].ToString();
                        money.signature = Convert.ToBase64String((byte[])reader["Signature"]);
                        moneyList.Add(money);
                    }
                }
            }
            catch (Exception e) { throw new Exception("DB Exception :" + e.InnerException); }
            finally { CON.Close(); }
            return moneyList;
        }

        public int GetMoneyCountFromUserAccount(string userAddressId, string uid)
        {
            int count;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM " + USER_AC_TABLE + " WHERE UserId = " + uid + " AND Status = " + 0;
                cmd.Connection = CON;
                CON.Open();
                count = (int)cmd.ExecuteScalar();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }

            return count;
        }

        public void UpdatedOwnerMoneyStatus(string uid)
        {
            string spName = "sp_UsersAccounts_Update_Status";
            try
            {
                SqlCommand cmd = new SqlCommand(spName, CON);
                cmd.CommandType = CommandType.StoredProcedure;
                CON.Open();
                cmd.Parameters.AddWithValue("@UserId", uid);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }
    }
}