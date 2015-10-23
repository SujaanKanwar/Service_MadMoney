using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MadMoney.AdminUserData;

namespace MadMoney.Cashier
{
    public class CashierService
    {

        public bool DepositeMoney(string userAddressId, int[] amount, int totalAmount, Credential credential)
        {
            if (ValidateDepositRequest(userAddressId, amount, totalAmount, credential))
            {
                try
                {
                    MoneyBank.MoneyBankService bankService = new MoneyBank.MoneyBankService(credential);
                    Dictionary<int, List<MoneyBank.Money>> moneyDictionary = bankService.GetCash(amount);

                    MoneyForger.MoneyForgerService forgeService = new MoneyForger.MoneyForgerService(credential);
                    forgeService.ForgeMoney(moneyDictionary, userAddressId);

                    CachierDBTool cDBTool = new CachierDBTool();
                    cDBTool.DepositMoneyInUserAC(moneyDictionary, userAddressId);
                    return true;
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message, e);
                }
            }
            else
            {
                throw new Exception("Invalid deposit money request");
            }
        }

        #region private functions

        private bool ValidateDepositRequest(string userAddressId, int[] amount, int totalAmount, Credential credential)
        {
            try
            {
                ValidateCredentials(credential);
                ValidateUserAddressId(userAddressId);
                validateAmount(amount, totalAmount);
            }
            catch (Exception e)
            {
                throw new Exception("Validation failed", e);
            }
            return true;
        }

        private void validateAmount(int[] amount, int totalAmount)
        {
            int[] moneySequenceArray = MoneyBank.MoneyGenerator.MoneySequence;
            int tempTotalAmount = 0;
            if (amount.Length != moneySequenceArray.Length)
                throw new Exception("Wrong Amount breakup");
            for (var i = 0; i < moneySequenceArray.Length; i++)
            {
                tempTotalAmount += moneySequenceArray[i] * amount[i];
            }
            if (tempTotalAmount != totalAmount)
                throw new Exception("Amount breakup and total amount are not matching.");

        }

        private void ValidateUserAddressId(string userAddressId)
        {
            UsersDBTool dbTool = new UsersDBTool();
            dbTool.ValidateUsers(userAddressId);
        }

        private void ValidateCredentials(Credential credential)
        {
            AdminUserDBTool userDBTool = new AdminUserDBTool();
            userDBTool.IsValidUserCredentials(credential);
        }

        #endregion

    }
}