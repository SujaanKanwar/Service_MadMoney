using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MadMoney
{
    [DataContract]
    public class GetTlocationsRQ
    {
        [DataMember(Name = "CurrentLocation")]
        public string currentLocation { get; set; }

    }
}
