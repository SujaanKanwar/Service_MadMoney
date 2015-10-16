using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using MadMoney.ServiceData;

namespace MadMoney
{
    [DataContract]
    public class MoneyFetchResponse
    {
        private MoneyFetchResponse() { }

        public MoneyFetchResponse(bool isSuccess, string status, List<Money> moneyList, string encryptedOTP)
        {            
            this.IsSuccess = isSuccess;
            this.status = status;
            this.moneyList = moneyList;
            this.encryptedOTP = encryptedOTP;
        }

        [DataMember]
        public string encryptedOTP { get; set; }

        [DataMember]
        public bool IsSuccess { get; set; }

        [DataMember]
        public string status { get; set; }

        [DataMember]
        public List<Money> moneyList { get; set; }
    }
}
