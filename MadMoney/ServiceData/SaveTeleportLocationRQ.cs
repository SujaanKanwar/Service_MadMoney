using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MadMoney
{
    [DataContract]
    public class SaveTeleportLocationRQ
    {
        [DataMember(Name = "TLocations")]
        public Location[] TLocations { get; set; }        
    }

    [DataContract]
    public class Location
    {
        
        [DataMember(Name = "Latitude")]
        public string latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public string longitude { get; set; }

        [DataMember(Name = "LocationName")]
        public string locationName { get; set; }

        [DataMember(Name = "City")]
        public string city { get; set; }

        [DataMember(Name = "Radius")]
        public string radius { get; set; }

        [DataMember(Name = "Rating")]
        public int rating { get; set; }

        [DataMember(Name = "Description")]
        public string description { get; set; }
    }

    public class GeofenceLocation : Location
    {
        const int LOITERING_TIME = 5 * 60 * 1000;
        const int EXPIRATION_TIME = 24 * 60 * 60 * 1000;
        const string DEFAULT_TRANSACTION_TYPE = "DWELL";        

        public GeofenceLocation() { }

        public GeofenceLocation(Location location)
        {                        
            this.latitude = location.latitude;
            this.longitude = location.longitude;
            this.locationName = location.locationName;
            this.city = location.city;
            this.radius = location.radius;
            this.rating = location.rating;
            this.description = location.description;
        }

        [DataMember(Name = "RequestId")]
        public string RequestId { get; set; }

        [DataMember(Name = "LoiteringTime")]
        public int LoiteringTime { get; set; }

        [DataMember(Name = "GeofenceTransactionType")]
        public string GeofenceTransactionType { get; set; }

        [DataMember(Name = "ExpirationTime")]
        public int ExpirationTime { get; set; }

        public static GeofenceLocation get(Location location)
        {
            GeofenceLocation geofenceLocation = new GeofenceLocation(location);            
            geofenceLocation.RequestId = Guid.NewGuid().ToString().Replace("-","");
            geofenceLocation.LoiteringTime = LOITERING_TIME;
            geofenceLocation.GeofenceTransactionType = DEFAULT_TRANSACTION_TYPE;
            geofenceLocation.ExpirationTime = EXPIRATION_TIME;
            return geofenceLocation;
        }
    }
}
