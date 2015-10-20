using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MadMoney
{
    [DataContract]
    public class MoneyFetchRequest
    {
        [DataMember(Name="DecryptedOTP")]
        public string decryptdOTP { get; set; }

        [DataMember(Name="UserAddressId")]
        public string userAddressId { get; set; }

        [DataMember(Name = "RequestType")]
        public string requestType { get; set; }
    }
}
