using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney.Cashier
{
    public class CashierServiceWrapper
    {
        private static Credential GetCredentials()
        {
            Credential credential = new Credential();
            credential.name = "sujaan";
            credential.key = "kanwar";
            return credential;
        }

        public static void DepositSameAmount(string depositAddress, int amount)
        {
            CashierService cashierService = new CashierService();

            int[] moneyBreakup = GenerateSameAmountMoneyBreakup(amount);

            cashierService.DepositeMoney(depositAddress, moneyBreakup, amount, GetCredentials());
        }

        public static void DepositSmallerMoney(string depositAddress, int amount)
        {
            CashierService cashierService = new CashierService();

            int[] moneyBreakup = GenerateSmallerMoneyBreakup(amount);

            cashierService.DepositeMoney(depositAddress, moneyBreakup, amount, GetCredentials());
        }



        private static int[] GenerateSameAmountMoneyBreakup(int amount)
        {
            int[] result = null;
            switch (amount)
            {
                //{ 1, 2, 5, 10, 20, 50, 100, 500, 1000 }
                case 1:
                    result = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case 2:
                    result = new int[] { 0, 1, 0, 0, 0, 0, 0, 0, 0 };
                    break;
                case 5:
                    result = new int[] { 0, 0, 1, 0, 0, 0, 0, 0, 0 };
                    break;
                case 10:
                    result = new int[] { 0, 0, 0, 1, 0, 0, 0, 0, 0 };
                    break;
                case 20:
                    result = new int[] { 0, 0, 0, 0, 1, 0, 0, 0, 0 };
                    break;
                case 50:
                    result = new int[] { 0, 0, 0, 0, 0, 1, 0, 0, 0 };
                    break;
                case 100:
                    result = new int[] { 0, 0, 0, 0, 0, 0, 1, 0, 0 };
                    break;
                case 500:
                    result = new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 0 };
                    break;
                case 1000:
                    result = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1 };
                    break;
            }
            return result;
        }

        private static int[] GenerateSmallerMoneyBreakup(int amount)
        {
            int[] result = null;
            switch (amount)
            {
                //{ 1, 2, 5, 10, 20, 50, 100, 500, 1000 }
                case 5:
                    result = new int[] { 3, 1, 0, 0, 0, 0, 0, 0, 0 };//{ 2, 1, 1, 1 };
                    break;
                case 10:
                    result = new int[] { 3, 1, 1, 0, 0, 0, 0, 0, 0 };//{ 5, 2, 1, 1, 1 };
                    break;
                case 20:
                    result = new int[] { 3, 1, 1, 1, 0, 0, 0, 0, 0 };//{ 10, 5, 2, 1, 1, 1 };
                    break;
                case 50:
                    result = new int[] { 4, 3, 2, 1, 1, 0, 0, 0, 0 };//{ 20, 10, 5, 5, 2, 2, 2, 1, 1, 1, 1 };
                    break;
                case 100:
                    result = new int[] { 4, 3, 2, 1, 1, 1, 0, 0, 0 };//{ 50, 20, 10, 5, 5, 2, 2, 2, 1, 1, 1, 1 };
                    break;
                case 500:
                    result = new int[] { 4, 3, 2, 1, 1, 1, 4, 0, 0 };//{ 100, 100, 100, 100, 50, 20, 10, 5, 5, 2, 2, 2, 1, 1, 1, 1 };
                    break;
                case 1000:
                    result = new int[] { 4, 3, 2, 1, 1, 1, 4, 1, 0 };//{ 500, 100, 100, 100, 100, 50, 20, 10, 5, 5, 2, 2, 2, 1, 1, 1, 1 };
                    break;
            }
            return result;
        }        
    }
}