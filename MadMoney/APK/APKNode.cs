using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MadMoney
{
    [Serializable]
    [DataContract]
    public class APKNode
    {        
        [DataMember(Name="Value")]
        public string value { get; set; }
        [DataMember(Name="SiblingIndex")]
        public int siblingIndex { get; set; }
        [DataMember(Name="PublicKey")]
        public string publicKey { get; set; }
    }
}