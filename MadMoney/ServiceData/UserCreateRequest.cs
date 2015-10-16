using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MadMoney
{
    //fiddler request: {"data":{ "FName":"Sujaan", "LName":"Singh", "Address":{"CountryCode":"in","State":"MH","City":"Pune","Local":"kharadi"}, "PublicKey":"test"}}
    [DataContract]
    public class UserCreateRequest
    {
        [DataMember(Name = "FName")]
        public string FName { get; set; }

        [DataMember(Name = "LName")]
        public string LName { get; set; }

        [DataMember(Name = "Address")]
        public Address Address { get; set; }

        [DataMember(Name = "PublicKey")]
        public string PublicKey { get; set; }
    }

    [DataContract]
    public class Address
    {
        [DataMember(Name = "CountryCode")]
        public string CountryCode { get; set; }

        [DataMember(Name = "State")]
        public string State { get; set; }

        [DataMember(Name = "City")]
        public string City { get; set; }

        [DataMember(Name = "Local")]
        public string Local { get; set; }
    }
}