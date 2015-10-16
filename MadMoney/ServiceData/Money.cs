using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MadMoney.ServiceData
{
    [DataContract]
    public class Money
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public int value { get; set; }
        [DataMember]
        public string hash { get; set; }
        [DataMember]
        public string ownerId { get; set; }
        [DataMember]
        public string dated { get; set; }
        [DataMember]
        public string signature { get; set; }
    }
}