using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using MadMoney.MoneyBank;

namespace MadMoney
{
    public class MoneyBankDBTool
    {
        private string[] MONEY_TABLE_NAME_ARRAY = new string[] { "One", "Two", "Five", "Ten", "Twenty", "Fifty", "Hundred", "FiveHundred", "Thousand" };
        private static string DB_CON_NAME = "MadMoneyUser";
        private string CONNECTION_STR;
        SqlConnection CON = null;

        public MoneyBankDBTool()
        {
            CONNECTION_STR = ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString();
        }

        public void InsertMoney(Dictionary<int, List<MoneyBank.Money>> moneys)
        {
            List<MoneyBank.Money> moneyList;

            for (var i = 0; i < moneys.Count; i++)
            {
                if (moneys.TryGetValue(MoneyGenerator.MoneySequence[i], out moneyList))
                {
                    if (moneyList.Count > 0)
                        Insert(moneyList, i);
                }
            }
        }

        public List<Money> FetchMoney(int moneyTableIndex, int howManyMoney)
        {
            List<Money> listMoney = new List<Money>();
            SqlConnection CON = new SqlConnection(CONNECTION_STR);
            SqlCommand cmd = new SqlCommand();
            Money money; string id; int value;

            cmd.CommandText = "SELECT TOP " + howManyMoney + "* FROM " + MONEY_TABLE_NAME_ARRAY[moneyTableIndex];
            cmd.Connection = CON;
            try
            {
                CON.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = reader["Id"].ToString();
                        value = (int)reader["Value"];
                        money = new Money(id, value);
                        listMoney.Add(money);
                    }
                }
            }
            catch (Exception) { throw new Exception("Exception while fetching money from table " + MONEY_TABLE_NAME_ARRAY[moneyTableIndex]); }
            finally { CON.Close(); }

            return listMoney;
        }

        public void DeleteFetchedMoney(Dictionary<int, List<Money>> moneyDictionary)
        {
            var thread = new Thread(() =>
            {
                for (int i = 0; i < moneyDictionary.Count; i++)
                {
                    if (moneyDictionary[MoneyBank.MoneyGenerator.MoneySequence[i]].Count > 0)
                        DeleteMoney(i, moneyDictionary[MoneyBank.MoneyGenerator.MoneySequence[i]]);
                }
            });
            thread.Start();
            thread.Join();
        }

        #region private function

        private void Insert(List<MoneyBank.Money> moneyList, int tableNameIndex)
        {
            string TABLE_NAME = MONEY_TABLE_NAME_ARRAY[tableNameIndex];
            StringBuilder sqlString = new StringBuilder();
            sqlString.Append(@"INSERT INTO " + TABLE_NAME + " VALUES ");

            for (var i = 0; i < moneyList.Count; i++)
            {
                string values = string.Format("({0}, {1}) ,", "@id" + i, "@value" + i);
                sqlString.Append(values);
            }
            string sql = sqlString.ToString().TrimEnd(',');
            try
            {
                CON = new SqlConnection(CONNECTION_STR);
                SqlCommand cmd = new SqlCommand(sql, CON);

                for (var i = 0; i < moneyList.Count; i++)
                {
                    cmd.Parameters.AddWithValue("@id" + i, moneyList[i].id);
                    cmd.Parameters.AddWithValue("@value" + i, moneyList[i].value);
                }
                CON.Open();
                cmd.ExecuteNonQuery();
            }

            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null) CON.Close(); }
        }

        private void DeleteMoney(int moneyTableIndex, List<Money> list)
        {
            List<Money> listMoney = new List<Money>();
            SqlConnection CON = new SqlConnection(CONNECTION_STR);
            SqlCommand cmd = new SqlCommand();

            string ids = GetIds(list);            
            cmd.Connection = CON;

            try
            {
                CON.Open();
                for (var i = 0; i < list.Count; i++)
                {
                    cmd.CommandText = "DELETE FROM " + MONEY_TABLE_NAME_ARRAY[moneyTableIndex] + " WHERE [Id] = '" + list[i] +"'";
                    cmd.ExecuteNonQuery();
                }                                
            }
            catch (Exception e) { throw new Exception("Exception while Deleting money from table " + MONEY_TABLE_NAME_ARRAY[moneyTableIndex] + ": " + e.InnerException); }
            finally { CON.Close(); }
        }

        private string GetIds(List<Money> list)
        {
            string ids = "";

            for (var i = 0; i < list.Count; i++)
            {
                ids += "'" + list[i].id + "' ,";
            }
            ids = ids.TrimEnd(',');

            return ids;
        }

        #endregion
    }
}