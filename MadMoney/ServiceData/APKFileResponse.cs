using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MadMoney
{
    [DataContract]
    public class APKFileResponse
    {
        public APKFileResponse(bool isSuccess, List<APKNode> apkTree)
        {
            this.isSuccess = isSuccess;
            this.apkTree = apkTree;
        }

        [DataMember(Name = "IsSuccess")]
        public bool isSuccess { get; set; }

        [DataMember(Name = "APKTreeArray")]
        public List<APKNode> apkTree { get; set; }
    }
}