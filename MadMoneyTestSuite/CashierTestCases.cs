using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
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
            string userAddress = "India/mh/pune/kharadi/c340791f19d9485795665fd5fd0647b1-1";
            Credential credential = new Credential();
            credential.name = "sujaan";
            credential.key = "kanwar";
            CashierService cashierService = new CashierService();
            var result = cashierService.DepositeMoney(userAddress, amount, 8, credential);
        }        
    }
}
