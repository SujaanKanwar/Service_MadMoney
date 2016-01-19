using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MadMoney
{
    [DataContract]
    public class GetTLocationsRS
    {
        public GetTLocationsRS(List<GeofenceLocation> position, bool isSuccess)
        {
            this.tLocations = position;
            this.IsSuccess = isSuccess;
        }
        [DataMember(Name = "TLocations")]
        public List<GeofenceLocation> tLocations { get; set; }

        [DataMember(Name = "IsSuccess")]
        public bool IsSuccess { get; set; }
    }

    [DataContract]
    public class Position
    {
        [DataMember(Name = "Latitude")]
        public string latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public string longitude { get; set; }

        [DataMember(Name = "Radius")]
        public string radius { get; set; }
    }
}
