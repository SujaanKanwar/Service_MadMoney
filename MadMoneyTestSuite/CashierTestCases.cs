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
            string userAddress = "india/mh/pune/kharadi/72679187d89249f9a0bff6b1ffe26756-4";
            Credential credential = new Credential();
            credential.name = "sujaan";
            credential.key = "kanwar";
            CashierService cashierService = new CashierService();
            var result = cashierService.DepositeMoney(userAddress, amount, 8, credential);
        }

        [TestMethod]
        public void testHexToBigint()
        {
            string hexStr = "eb8e2005781dd5b3b4ced1981605bad081110ed0c8b016fc4707c58380e0c8e60fd6ea41edd639e4b21edaa457ea6971b79eea6ac9bf73335fb1cd18bd72d1a68fcedb28ddb26f56be3f1ce3f60baf9907deec97118e719309ab2e252ca8dcfdefb7d64b716a4b2372a128b771c26b5079692602f1edc95734df2779c1defccb";
            BigInteger bigInteger = BigInteger.Parse(hexStr, NumberStyles.AllowHexSpecifier);
            
        }
    }
}
