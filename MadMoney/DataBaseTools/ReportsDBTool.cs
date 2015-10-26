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
    public class ReportsDBTool
    {
        private static string DB_CON_NAME = "MadMoneyReport";
        SqlConnection CON = null;
        private string APKSTORE_TABLE_NAME = "RefundRequest";

        public ReportsDBTool()
        {
            string CONNECTION_STR = ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString();
            CON = new SqlConnection(CONNECTION_STR);
        }

        public void Store(DepositMoneyToBankAcountRQ request, int totalAmount)
        {
            string spName = "sp_RefundRequest_Insert";
            try
            {
                SqlCommand cmd = new SqlCommand(spName, CON);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserAddressId", request.userAddressId);
                cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                cmd.Parameters.AddWithValue("@Name", request.bankAccountDetails.name);
                cmd.Parameters.AddWithValue("@AccountNo", request.bankAccountDetails.accountNo);
                cmd.Parameters.AddWithValue("@IFSC", request.bankAccountDetails.ifsc);
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }


            //store in Db
        }        
    }
}