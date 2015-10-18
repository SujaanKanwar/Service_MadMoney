using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using MadMoney.ServiceData;

namespace MadMoney
{
    public class MadMoneyService : IMadMoneyService
    {

        public UserCreateResponse CreateUser(UserCreateRequest userCreateRequest)
        {
            string userAddress = string.Empty; string failureDesc = string.Empty;
            bool isSuccess = false; List<APKNode> apkTreeStore = null;

            if (ValidateUserCreateRequest(userCreateRequest))
            {
                try
                {
                    int userNo = usersDBTool.GetUserCount();

                    userAddress = GenerateUserAddress(userCreateRequest, userNo);

                    usersDBTool.CreateUser(userCreateRequest, userAddress);

                    apkTreeStore = apkTree.InserUser(userAddress, userCreateRequest.PublicKey);

                    isSuccess = true;
                }
                catch (Exception e)
                {
                    failureDesc = "DataBase exception" + e.InnerException;
                }
            }
            else
            {
                failureDesc = "Invalid request";
            }

            return new UserCreateResponse(isSuccess, failureDesc, userAddress, apkTreeStore);
        }

        public MoneyFetchResponse FetchMoney(MoneyFetchRequest moneyFetchRequest)
        {
            List<Money> moneyList = new List<Money>(); bool isSuccess = false;
            string status = string.Empty; string encryptedOTP = string.Empty;
            try
            {
                if (ValidateMoneyFetchRequest(moneyFetchRequest))
                {
                    if (string.IsNullOrEmpty(moneyFetchRequest.decryptdOTP))
                    {
                        encryptedOTP = GenerateEncryptedOTP(moneyFetchRequest);
                        status = "OTP_SENT";
                    }
                    else
                    {
                        string storedOTP = GetStoredOTP(moneyFetchRequest.userAddressId);

                        if (!string.IsNullOrEmpty(storedOTP) && moneyFetchRequest.decryptdOTP.CompareTo(storedOTP) == 0)
                        {
                            moneyList = GetAllMoneyFromAccount(moneyFetchRequest.userAddressId);
                            status = "MONEY_SENT";
                            isSuccess = true;
                        }
                        else
                            status = "OTP_MISSMATCHED";
                    }
                }
            }
            catch (Exception e)
            {
                status = "Exception occured" + e.InnerException;
            }

            return new MoneyFetchResponse(isSuccess, status, moneyList, encryptedOTP);
        }

        #region private functions

        private UsersDBTool usersDBTool = new UsersDBTool();

        private APKTree apkTree = new APKTree();

        private string GenerateUserAddress(UserCreateRequest data, int userNo)
        {
            string uid = Guid.NewGuid().ToString();
            return data.Address.CountryCode + "/" + data.Address.State + "/" + data.Address.City + "/" + data.Address.Local + "/" + uid.Replace("-", "") + "-" + ++userNo;
        }

        private bool ValidateUserCreateRequest(UserCreateRequest data)
        {
            if (data == null || data.PublicKey == null || data.FName == null || data.LName == null || data.Address.CountryCode == null || data.Address.City == null)
                return false;
            return true;
        }


        private List<Money> GetAllMoneyFromAccount(string userAddressId)
        {
            CachierDBTool cachierDbTool = new CachierDBTool();

            string uid = usersDBTool.ValidateUsers(userAddressId);

            return cachierDbTool.GetMoneyFromUserAccount(userAddressId, uid);
        }

        private string GetStoredOTP(string userAddressId)
        {
            return usersDBTool.GetOTP(userAddressId);
        }

        private bool ValidateMoneyFetchRequest(MoneyFetchRequest moneyFetchRequest)
        {
            if (string.IsNullOrEmpty(moneyFetchRequest.userAddressId))
                return false;
            usersDBTool.ValidateUsers(moneyFetchRequest.userAddressId);
            return true;
        }

        private void StoreTheOTPwrtAddressId(string userAddressId, string otp)
        {
            usersDBTool.StoreOTP(userAddressId, otp);
        }

        private string GenerateEncryptedOTP(MoneyFetchRequest moneyFetchRequest)
        {
            string OTP = Guid.NewGuid().ToString();

            string userPublicKey = apkTree.GetPublicKey(moneyFetchRequest.userAddressId);

            StoreTheOTPwrtAddressId(moneyFetchRequest.userAddressId, OTP);


            return EncryptString(OTP, 1024, userPublicKey);

            //RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024);

            //RSA.FromXmlString(userPublicKey);

            //byte[] toEncryptData = Encoding.ASCII.GetBytes(OTP);

            //byte[] encryptedData = RSA.Encrypt(toEncryptData, false);

            //return Encoding.GetEncoding("ISO-8859-1").GetString(encryptedData);
        }

        private string EncryptString(string inputString, int dwKeySize, string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(inputString);
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[(dataLength - maxLength * i > maxLength) ? 
                    maxLength : dataLength - maxLength * i];

                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0, tempBytes.Length);

                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes, true);

                Array.Reverse(encryptedBytes);

                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }

            return stringBuilder.ToString();
        }

        #endregion
    }
}
