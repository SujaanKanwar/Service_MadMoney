using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MadMoney
{
    public class TelePortingLocationDb
    {
        SqlConnection CON = null;
        string DB_CON_NAME = "TeleportingLocation";
        string T_LOCATION_TABLE = "TLocations";

        public TelePortingLocationDb()
        {
            CON = new SqlConnection(ConfigurationManager.ConnectionStrings[DB_CON_NAME].ToString());
        }

        public void saveLocations(List<GeofenceLocation> locations)
        {
            foreach (var location in locations)
            {
                saveLocation(location);
            }
        }

        private void saveLocation(GeofenceLocation location)
        {
            string spName = "sp_TLocations_Insert";
            try
            {
                SqlCommand cmd = new SqlCommand(spName, CON);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationName", location.locationName);
                cmd.Parameters.AddWithValue("@Latitude", location.latitude);
                cmd.Parameters.AddWithValue("@Longitude", location.longitude);
                cmd.Parameters.AddWithValue("@City", location.city.ToUpper());
                cmd.Parameters.AddWithValue("@Radius", location.radius);
                cmd.Parameters.AddWithValue("@Rating", location.rating);
                cmd.Parameters.AddWithValue("@Description", location.description);

                cmd.Parameters.AddWithValue("@RequestId", location.RequestId);
                cmd.Parameters.AddWithValue("@LoiteringTime", location.LoiteringTime);
                cmd.Parameters.AddWithValue("@GeoTransactionType", location.GeofenceTransactionType);
                cmd.Parameters.AddWithValue("@ExpirationTime", location.ExpirationTime);
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }

        public List<GeofenceLocation> getTLocations(string city)
        {
            List<GeofenceLocation> locations = new List<GeofenceLocation>();
            GeofenceLocation temp;
            city = city.ToUpper();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM " + T_LOCATION_TABLE + " WHERE City = '" + city + "' AND Enable = " + 1;
            cmd.Connection = CON;
            CON.Open();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        temp = new GeofenceLocation();
                        temp.locationName = reader["LocationName"].ToString();
                        temp.latitude = reader["Latitude"].ToString();
                        temp.longitude = reader["Longitude"].ToString();
                        temp.city = reader["City"].ToString();
                        temp.radius = reader["Radius"].ToString();
                        temp.description = reader["Description"].ToString();
                        temp.RequestId = reader["RequestId"].ToString();
                        temp.LoiteringTime = (int)reader["LoiteringTime"];
                        temp.GeofenceTransactionType = reader["GeoTransactionType"].ToString();
                        temp.ExpirationTime = (int)reader["ExpirationTime"];
                        locations.Add(temp);
                    }
                }
            }
            catch (Exception e) { throw new Exception("DB Exception :" + e.InnerException); }
            finally { CON.Close(); }

            return locations;
        }

        public GeofenceLocation getTLocation(string requestId)
        {
            GeofenceLocation location = null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT * FROM " + T_LOCATION_TABLE + " WHERE RequestId = '" + requestId + "'";
            cmd.Connection = CON;
            CON.Open();
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        location = new GeofenceLocation();
                        location.locationName = reader["LocationName"].ToString();
                        location.latitude = reader["Latitude"].ToString();
                        location.longitude = reader["Longitude"].ToString();
                        location.city = reader["City"].ToString();
                        location.radius = reader["Radius"].ToString();
                        location.rating = int.Parse(reader["Rating"].ToString());
                        location.description = reader["Description"].ToString();
                        location.RequestId = reader["RequestId"].ToString();
                        location.LoiteringTime = (int)reader["LoiteringTime"];
                        location.GeofenceTransactionType = reader["GeoTransactionType"].ToString();
                        location.ExpirationTime = (int)reader["ExpirationTime"];
                    }
                }
            }
            catch (Exception e) { throw new Exception("DB Exception :" + e.InnerException); }
            finally { CON.Close(); }

            return location;
        }
    }
}
