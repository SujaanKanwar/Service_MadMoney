using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MadMoney
{
    [DataContract]
    public class UserCreateResponse
    {
        public UserCreateResponse(bool isSuccess, string failureDesc, string uid, APKStaticStore[] apkTree)
        {
            this.isSuccess = isSuccess;
            this.failureDesc = failureDesc;
            this.uid = uid;
            this.apkTree = apkTree;
        }

        [DataMember(Name="IsSuccess")]
        public bool isSuccess { get; set; }

        [DataMember(Name="FailureDesc")]
        public string failureDesc { get; set; }

        [DataMember(Name="UserAddressId")]
        public string uid { get; set; }

        [DataMember(Name="APKTreeArray")]
        public APKStaticStore[] apkTree { get; set; }
    }
}
