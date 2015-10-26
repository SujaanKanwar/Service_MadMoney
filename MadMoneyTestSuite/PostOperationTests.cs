using System;
using System.Collections.Generic;
using MadMoney;
using MadMoney.ServiceData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MadMoneyTestSuite
{
    [TestClass]
    public class PostOperationTests
    {
        [TestMethod]
        public void TestRegenerateSmallerMoney()
        {
            CachierDBTool dbTool = new CachierDBTool();
            var moneyList = dbTool.GetMoneyFromUserAccount("india/mh/pune/kharadi/6bef105a11ee406a8189404653d0895f-1", "1");

            RegenerateSmallerMoneyRequest request = new RegenerateSmallerMoneyRequest();
            request.userAddressId = "india/mh/pune/kharadi/6bef105a11ee406a8189404653d0895f-1";

            request.money = GetTestMoney();           

            MadMoneyService madMoneyService = new MadMoneyService();

            var result = madMoneyService.RegenerateSmallerMoney(request);

            Assert.IsTrue(result);
        }

        private Money GetTestMoney()
        {
            Money money = new Money();
            money.id = "026908d67bc249fcb542fa2c4786c1b65";
            money.value = 5;
            money.signature = "NscaJHrr6XndK4PD8SDwnb5UYr5IpDF/W7U3BCB6aEz+zSNYEU2co932Q024DCiYMjqeS3e0odDuOid0qouI9yiu5kzUKuYgzRDXBHFX3jpBWC8c05XzG7PDc99H9K3imJ4eBWvRl4mMAD9zyTSiSNQHYrkYiMg7tAGW7l/2DNU=";
            money.dated = "10/23/2015 2:07:35 PM";
            money.ownerId = "india/mh/pune/kharadi/6bef105a11ee406a8189404653d0895f-1";        
            return money;
        }

        [TestMethod]
        public void TestDepositMoneyToAccount()
        {
            DepositMoneyToAcountRQ request = new DepositMoneyToAcountRQ();
            request.moneyList = new List<Money>();
            var money = GetTestMoney();
            request.moneyList.Add(money);
            request.userAddressId = "india/mh/pune/kharadi/6bef105a11ee406a8189404653d0895f-1";

            MadMoneyService madMoneyService = new MadMoneyService();

            var result = madMoneyService.DepositMoneyToAccount(request);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void TestDepositMoneyToBankAccount()
        {
            DepositMoneyToBankAcountRQ request = new DepositMoneyToBankAcountRQ();
            request.moneyList = new List<Money>();
            var money = GetTestMoney();
            request.moneyList.Add(money);
            request.userAddressId = "india/mh/pune/kharadi/6bef105a11ee406a8189404653d0895f-1";

            request.bankAccountDetails = new DepositMoneyToBankAcountRQ.BankAccountDetails();
            request.bankAccountDetails.name = "Sujan";
            request.bankAccountDetails.accountNo = "00391160018340";
            request.bankAccountDetails.ifsc = "HDFC0000039";


            MadMoneyService madMoneyService = new MadMoneyService();

            var result = madMoneyService.DepositMoneyToBankAccount(request);

            Assert.IsTrue(result);
        }
    }
}
