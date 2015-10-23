using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MadMoney.ServiceData
{
    [DataContract]
    public class DepositMoneyToBankAcountRQ
    {
        [DataMember]
        public string userAddressId { get; set; }

        [DataMember]
        public List<Money> moneyList { get; set; }

        [DataMember]
        public BankAccountDetails bankAccountDetails { get; set; }

        [DataContract]
        public class BankAccountDetails
        {
            [DataMember]
            public string name { get; set; }

            [DataMember]
            public string accountNo { get; set; }

            [DataMember]
            public string ifsc { get; set; }
        }
    }
}
