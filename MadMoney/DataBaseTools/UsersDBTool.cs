using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using MadMoney.MoneyBank;

namespace MadMoney
{
    public class UsersDBTool
    {

        private static string DB_CON_NAME = "MadMoneyUser";
        private static string USER_TABLE_NAME = "Users";
        private string OTP_TABLE_NAME = "OTPTable";
        SqlConnection CON = null;

        public UsersDBTool()
        {
            CON = new SqlConnection(ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString());
        }

        public int GetUserCount()
        {
            int count;
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM " + USER_TABLE_NAME;
                cmd.Connection = CON;
                CON.Open();
                count = (int)cmd.ExecuteScalar();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }

            return count;
        }

        public void CreateUser(UserCreateRequest data, string uid)
        {
            string sqlQUery = "INSERT INTO " + USER_TABLE_NAME + " VALUES(@FName, @LName, @Address, @UserPublicKey, @userAddressId)";
            try
            {
                SqlCommand cmd = new SqlCommand(sqlQUery, CON);
                cmd.Parameters.AddWithValue("@FName", data.FName);
                cmd.Parameters.AddWithValue("@LName", data.LName);
                cmd.Parameters.AddWithValue("@Address", data.Address.CountryCode + "/" + data.Address.State + "/" + data.Address.City + "/" + data.Address.Local);
                cmd.Parameters.AddWithValue("@UserPublicKey", data.PublicKey);
                cmd.Parameters.AddWithValue("@userAddressId", uid);
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }

        public void DeleteUser(string uid)
        {

        }

        public void GetUser(string uid)
        {

        }

        public string ValidateUsers(string userAddressId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT [Id], userAddressId FROM " + USER_TABLE_NAME;
            cmd.Connection = CON;
            CON.Open();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    string dbUserAddress;
                    while (reader.Read())
                    {
                        dbUserAddress = reader["userAddressId"].ToString();
                        if (string.Compare(dbUserAddress, userAddressId, true) == 0)
                            return reader["Id"].ToString();
                    }
                    throw new Exception("User doesn't exist in the database");
                }
            }
            catch (Exception e)
            { throw new Exception("Exception while connecting DB" +e.InnerException); }
            finally { CON.Close(); }
        }

        public void StoreOTP(string userAddressId, string otp)
        {
            string spName = "sp_OTPTable_Insert_Update";
            try
            {
                SqlCommand cmd = new SqlCommand(spName, CON);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserAddress", userAddressId);
                cmd.Parameters.AddWithValue("@OTP", otp);
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }

        public string GetOTP(string userAddressId)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT [OTP] FROM " + OTP_TABLE_NAME +" WHERE UserAddressId = '" + userAddressId +"'";
            cmd.Connection = CON;
            CON.Open();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    string dbUserAddress;
                    while (reader.Read())
                    {
                        return dbUserAddress = reader["OTP"].ToString();
                    }
                }
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally{CON.Close();}
            return null;
        }

    }
}
