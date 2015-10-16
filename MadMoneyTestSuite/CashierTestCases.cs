using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadMoney.Cashier;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MadMoneyTestSuite
{
    [TestClass]
    public class CashierTestCases
    {
        [TestMethod]
        public void DepositeMoneyTests()
        {
            int[] amount = new int[] { 1, 1, 1, 0, 0, 0, 0, 0, 0 };
            string userAddress = "in/MH/Pune/kharadi/2e179ab097924cf2b4ae572b257f6cc2-1";
            Credential credential = new Credential();
            credential.name = "sujaan";
            credential.key = "kanwar";
            CashierService cashierService = new CashierService();
            var result = cashierService.DepositeMoney(userAddress, amount, 8, credential);
        }
    }
}
