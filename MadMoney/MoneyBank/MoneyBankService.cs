using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney.MoneyBank
{
    public class MoneyBankService
    {
        private Cashier.Credential myCredentials;

        private MoneyBankService() { }
        
        public MoneyBankService(Cashier.Credential myCredentials)
        {
            AdminUserData.AdminUserDBTool adminDBTool = new AdminUserData.AdminUserDBTool();
            adminDBTool.IsValidUserCredentials(myCredentials);
            this.myCredentials = myCredentials;    
        }

        public Dictionary<int, List<Money>> GetCash(int[] amount)
        {
            Dictionary<int, List<Money>> moneyDictionary = new Dictionary<int, List<Money>>();
            MoneyBankDBTool mbDBTool = new MoneyBankDBTool();
            for (var i = 0; i < amount.Length; i++)
            {
                moneyDictionary.Add(MoneyBank.MoneyGenerator.MoneySequence[i], mbDBTool.FetchMoney(i, amount[i]));
            }
            if (ValidateFetchedMoney(moneyDictionary, amount))
                mbDBTool.DeleteFetchedMoney(moneyDictionary);
            else
                throw new Exception("Fail to retrieve the money");
            
            return moneyDictionary;
        }

        private bool ValidateFetchedMoney(Dictionary<int, List<Money>> moneyDictionary, int[] amount)
        {
            for (var i = 0; i < amount.Length; i++)
            {
                if (moneyDictionary[MoneyBank.MoneyGenerator.MoneySequence[i]].Count != amount[i])
                    return false;
            }
            return true;
        }        
    }
}