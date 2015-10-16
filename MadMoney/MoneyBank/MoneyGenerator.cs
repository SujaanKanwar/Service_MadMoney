using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney.MoneyBank
{
    public class MoneyGenerator
    {
        private static int[] MoneyMultiplier = new int[] { 100, 100, 20, 20, 10, 2, 1, 0, 0 };
        public static int[] MoneySequence = new int[] { 1, 2, 5, 10, 20, 50, 100, 500, 1000 };

        public enum Consignment
        { ES = 1, S, M, L, EL }

        public static bool GenerateMoney(Consignment consignment)
        {
            Dictionary<int, List<Money>> moneyDictionary = new Dictionary<int, List<Money>>();
            int value;
            int[] moneyMultiplier = GetMultiplierArray(consignment);
            try
            {
                for (var i = 0; i < moneyMultiplier.Length; i++)
                {
                    List<Money> moneyList = new List<Money>();
                    value = MoneySequence[i];
                    for (var j = 0; j < moneyMultiplier[i]; j++)
                    {
                        Money money = new Money(value);
                        moneyList.Add(money);
                    }
                    moneyDictionary.Add(value, moneyList);
                }
                MoneyBankDBTool mdDBTool = new MoneyBankDBTool();
                mdDBTool.InsertMoney(moneyDictionary);
            }
            catch (Exception) { return false; }
            return true;
        }

        private static int[] GetMultiplierArray(Consignment consignment)
        {
            int[] result = new int[MoneyMultiplier.Length];
            int multiple = 0;
            if (consignment == Consignment.ES)
                multiple = 1;
            else if (consignment == Consignment.S)
                multiple = 2;
            else if (consignment == Consignment.M)
                multiple = 3;
            else if (consignment == Consignment.L)
                multiple = 4;
            else if (consignment == Consignment.EL)
                multiple = 5;

            for (var i = 0; i < MoneyMultiplier.Length; i++)
            {
                result[i] = MoneyMultiplier[i] * multiple;
            }
            return result;
        }
    }
}