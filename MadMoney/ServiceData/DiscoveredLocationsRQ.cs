using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MadMoney
{
    [DataContract]
    public class DiscoveredLocationsRQ
    {
        [DataMember(Name = "Locations")]
        public List<DiscoveredLocations> discoveredLocations { get; set; }

        [DataMember(Name = "UserAddressId")]
        public String userAddressId { get; set; }

        [DataMember(Name = "Signature")]
        public String signature { get; set; }
    }

    public class DiscoveredLocations
    {
        [DataMember(Name = "RequestId")]
        public String RequestId { get; set; }

        [DataMember(Name = "DateAndTimeOfDiscover")]
        public String DateAndTimeOfDiscover { get; set; }
    }
}
