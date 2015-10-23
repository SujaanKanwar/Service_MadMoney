using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MadMoney.ServiceData
{
    [DataContract]
    public class RegenerateSmallerMoneyRequest
    {
        [DataMember]
        public string userAddressId { get; set; }

        [DataMember]
        public Money money { get; set; }
    }
}