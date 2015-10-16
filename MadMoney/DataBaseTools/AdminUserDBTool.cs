using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MadMoney.AdminUserData
{
    public class AdminUserDBTool
    {
        private string CONNECTION_STR;
        private static string DB_CON_NAME = "UsersCredentials";

        public AdminUserDBTool()
        {
            CONNECTION_STR = ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString();
        }

        public void IsValidUserCredentials(Cashier.Credential credential)
        {
            SqlConnection CON = new SqlConnection(CONNECTION_STR);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM UsersCredentials";
            cmd.Connection = CON;
            CON.Open();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    string name, password;
                    while (reader.Read())
                    {
                        name = reader["Name"].ToString();
                        if (string.Compare(name, credential.name, true) == 0)
                        {
                            password = reader["Password"].ToString();
                            if (password.CompareTo(credential.key) == 0)
                                return;
                            else
                                throw new Exception("Wrong Password");
                        }
                    }
                    throw new Exception("User Name doesn't exists");
                }
            }
            catch (Exception)
            {
                throw new Exception("Database exception while retriving credentials");
            }
            finally {
                CON.Close();
            }
        }
    }
}