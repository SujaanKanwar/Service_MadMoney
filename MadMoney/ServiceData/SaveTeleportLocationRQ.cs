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
}
