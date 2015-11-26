using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MadMoney.ServiceData
{
    [DataContract]
    public class DepositMoneyToAcountRQ
    {
        [DataMember(Name = "UserAddressId")]        
        public string userAddressId { get; set; }

        [DataMember(Name = "MoneyList")]        
        public List<Money> moneyList { get; set; }
    }
}