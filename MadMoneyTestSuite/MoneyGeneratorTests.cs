using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadMoney;
using MadMoney.MoneyBank;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MadMoneyTestSuite
{
    [TestClass]
    public class MoneyGeneratorTests
    {
        [TestMethod]
        public void GenerateMoneyTest()
        {
           var returnValue=  MoneyGenerator.GenerateMoney(MoneyGenerator.Consignment.EL); 
        }

        [TestMethod]
        public void InsertTestCases()
        {
            var dbTool = new UsersDBTool();
            List<MadMoney.MoneyBank.Money> oneMoneyList = new List<MadMoney.MoneyBank.Money>();
            oneMoneyList.Add(new Money(1));

            //dbTool.InsertOne(oneMoneyList, );
        }
        
    }
}
