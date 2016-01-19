using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using MadMoney.DataClasses;

namespace MadMoney.DataBaseTools
{
    public class DiscoveredLocationsDBTool
    {
        private static string DB_CON_NAME = "DiscoveredLocation";
        private SqlConnection CON = null;
        private string DISCOVERED_LOCATION_TABLE_NAME = "UserDiscoveredLocations";

        public DiscoveredLocationsDBTool()
        {
            CON = new SqlConnection(ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString());
        }

        public void StoreUserDiscoveredLocations(string userAddressId, string geoRequestId, string timeOfDiscovery)
        {
            string sqlQUery = "INSERT INTO " + DISCOVERED_LOCATION_TABLE_NAME + " ([UserAddressId] ,[GeofencRequestId],[TimeOfDiscovery]) VALUES(@UserAddressId,@GeofencRequestId,@TimeOfDiscovery)";
            try
            {
                SqlCommand cmd = new SqlCommand(sqlQUery, CON);
                cmd.Parameters.AddWithValue("@UserAddressId", userAddressId);
                cmd.Parameters.AddWithValue("@GeofencRequestId", geoRequestId);
                cmd.Parameters.AddWithValue("@TimeOfDiscovery", timeOfDiscovery != null ? timeOfDiscovery : "");
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }

        public List<UserGeoDiscoveredLocation> SelectAllDiscoveredLocations()
        {
            List<UserGeoDiscoveredLocation> discoveredLocations = new List<UserGeoDiscoveredLocation>();
            var sqlQuery = "SELECT * FROM " + DISCOVERED_LOCATION_TABLE_NAME + " WHERE OperationStatus = '" + 0 + "'";
            SqlCommand cmd = new SqlCommand(sqlQuery, CON);
            string userAddressId, requestId, timeOfDiscovery;
            UserGeoDiscoveredLocation temp;
            int id;
            try
            {
                CON.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        id = (int)reader["Id"];
                        userAddressId = reader["UserAddressId"].ToString();
                        requestId = reader["GeofencRequestId"].ToString();
                        timeOfDiscovery = reader["TimeOfDiscovery"].ToString();
                        temp = new UserGeoDiscoveredLocation(id, userAddressId, requestId, timeOfDiscovery);
                        discoveredLocations.Add(temp);
                    }
                }
            }
            catch (Exception) { throw new Exception("Exception while discovered locations from table "); }
            finally { CON.Close(); }
            return discoveredLocations;
        }

        public void UpdateOperationStatus(int id)
        {
            string sqlQUery = "UPDATE " + DISCOVERED_LOCATION_TABLE_NAME + " SET OperationStatus = 1 WHERE [Id] = '" + id + "'";
            try
            {
                SqlCommand cmd = new SqlCommand(sqlQUery, CON);
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }
    }
}