using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MadMoney.ServiceData
{
    [DataContract]
    public class PublicKey
    {
        [DataMember]
        public string MOD { get; set; }
        [DataMember]
        public string EXP { get; set; }
    }
}