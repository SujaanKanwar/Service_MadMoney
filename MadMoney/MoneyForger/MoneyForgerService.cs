using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney.MoneyForger
{
    public class MoneyForgerService
    {
        private Cashier.Credential myCredentials;
        private MoneyForgerService() { }

        public MoneyForgerService(Cashier.Credential myCredentials)
        {
            AdminUserData.AdminUserDBTool adminDBTool = new AdminUserData.AdminUserDBTool();
            adminDBTool.IsValidUserCredentials(myCredentials);
            this.myCredentials = myCredentials;
        }

        public void ForgeMoney(Dictionary<int, List<MoneyBank.Money>> moneyDictionary, string userAddress)
        {
            string dateTime = DateTime.Now.ToString();
            string cashierPrivateKeyXmlFile = @"D:\CashierKeys\private.xml";
            foreach (var moneyList in moneyDictionary.Values)
            {
                foreach (var money in moneyList)
                {
                    money.Initialize(userAddress, cashierPrivateKeyXmlFile);
                }
            }
        }
    }
}