using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney.DataClasses
{
    public class UserGeoDiscoveredLocation
    {        

        public UserGeoDiscoveredLocation(int id, string userAddressId, string requestId, string timeOfDiscovery)
        {            
            this.id = id;
            this.userAddressId = userAddressId;
            this.geoRequestId = requestId;
            this.timeOfDiscovery = timeOfDiscovery;
        }
        public int id { get; private set; }
        public string userAddressId { get; private set; }
        public string geoRequestId { get; private set; }
        public string timeOfDiscovery { get; private set; }
    }
}