﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
            int[] amount = new int[] { 1, 1, 1, 1, 1, 1, 1, 1, 0 };
            string userAddress = "INDIA/MH/PUNE/Kharadi/ec038f028f9a4f66868a12d655853e3e-18";//"INDIA/MH/PUNE/Kharadi/0c7cc9dbe1fc4e54a4378a4e314c0958-10";
            Credential credential = new Credential();
            credential.name = "sujaan";
            credential.key = "kanwar";
            CashierService cashierService = new CashierService();
            bool result;
            result = cashierService.DepositeMoney(userAddress, amount, 688, credential);
            Assert.IsTrue(result);
        }
    }
}
