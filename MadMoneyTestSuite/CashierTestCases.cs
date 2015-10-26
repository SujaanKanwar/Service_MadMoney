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
            int[] amount = new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1 };
            string userAddress = "india/mh/pune/kharadi/6bef105a11ee406a8189404653d0895f-1";
            Credential credential = new Credential();
            credential.name = "sujaan";
            credential.key = "kanwar";
            CashierService cashierService = new CashierService();
            var result = cashierService.DepositeMoney(userAddress, amount, 1500, credential);
            Assert.IsTrue(result);
        }        
    }
}
