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

        public void saveLocations(Location[] locations)
        {
            foreach (var location in locations)
            {
                saveLocation(location);
            }
        }

        private void saveLocation(Location location)
        {
            string spName = "sp_TLocations_Insert";
            try
            {
                SqlCommand cmd = new SqlCommand(spName, CON);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@LocationName", location.locationName);
                cmd.Parameters.AddWithValue("@Latitude", location.latitude);
                cmd.Parameters.AddWithValue("@Longitude", location.longitude);
                cmd.Parameters.AddWithValue("@City", location.city);
                cmd.Parameters.AddWithValue("@Radius", location.radius);
                cmd.Parameters.AddWithValue("@Rating", location.rating);
                cmd.Parameters.AddWithValue("@Description", location.description);
                CON.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e) { throw new Exception("Error while connecting database", e); }
            finally { if (CON != null)CON.Close(); }
        }

        public List<Position> getTLocations(string city)
        {
            List<Position> locations = new List<Position>();
            Position temp;
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
                        temp = new Position();
                        temp.latitude = reader["Latitude"].ToString();
                        temp.longitude = reader["Longitude"].ToString();
                        temp.radius = reader["Radius"].ToString();
                        locations.Add(temp);
                    }
                }
            }
            catch (Exception e) { throw new Exception("DB Exception :" + e.InnerException); }
            finally { CON.Close(); }

            return locations;
        }
    }
}
