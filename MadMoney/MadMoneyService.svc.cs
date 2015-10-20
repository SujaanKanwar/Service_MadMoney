using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
            string status = string.Empty, encryptedOTP = string.Empty;
            try
            {
                if (ValidateMoneyFetchRequest(moneyFetchRequest))
                {
                    switch (moneyFetchRequest.requestType)
                    {
                        case Constants.FetchMoneyRequest.INIT:

                            if (IsMoneyInAccount(moneyFetchRequest.userAddressId))
                            {
                                encryptedOTP = GenerateEncryptedOTP(moneyFetchRequest);

                                status = Constants.FetchMoneyResponse.OTP_SENT;
                            }
                            else
                                status = Constants.FetchMoneyResponse.EMPTY_AC;

                            isSuccess = true;

                            break;

                        case Constants.FetchMoneyRequest.DECRYPTED_OTP:

                            string storedOTP = GetStoredOTP(moneyFetchRequest.userAddressId);

                            if (!string.IsNullOrEmpty(storedOTP) && moneyFetchRequest.decryptdOTP.CompareTo(storedOTP) == 0)
                            {
                                moneyList = GetAllMoneyFromAccount(moneyFetchRequest.userAddressId);

                                status = Constants.FetchMoneyResponse.MONEY_SENT;
                            }
                            else
                                status = Constants.FetchMoneyResponse.OTP_MISMATCHED;

                            isSuccess = true;

                            break;

                        case Constants.FetchMoneyRequest.RECEIVED_OK:
                            UpdateFetchedMoneyStatus(moneyFetchRequest.userAddressId);
                            break;
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

        private CachierDBTool cachierDbTool = new CachierDBTool();

        private APKTree apkTree = APKTree.APKTreeInstance();

        private void UpdateFetchedMoneyStatus(string userAddressId)
        {
            string uid = usersDBTool.ValidateUsers(userAddressId);

            cachierDbTool.UpdatedOwnerMoneyStatus(uid);
        }

        private bool IsMoneyInAccount(string userAddressId)
        {
            string uid = usersDBTool.ValidateUsers(userAddressId);

            return cachierDbTool.GetMoneyCountFromUserAccount(userAddressId, uid) == 0 ? false : true;
        }

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
            string OTP = Guid.NewGuid().ToString().Replace("-", "");

            string userPublicKey = apkTree.GetPublicKey(moneyFetchRequest.userAddressId);

            StoreTheOTPwrtAddressId(moneyFetchRequest.userAddressId, OTP);

            return EncryptString(OTP, userPublicKey);
        }

        private string EncryptString(string inputString, string jsonPublicKey)
        {
            RSAParameters rsaParameter = new RSAParameters();

            PublicKey publicKey = JsonDeserialize<PublicKey>(jsonPublicKey);

            rsaParameter.Exponent = Convert.FromBase64String(publicKey.EXP);

            rsaParameter.Modulus = Convert.FromBase64String(publicKey.MOD);

            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();

            rsaCryptoServiceProvider.ImportParameters(rsaParameter);

            byte[] bytes = Encoding.UTF8.GetBytes(inputString);

            byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(bytes, false);

            return Convert.ToBase64String(encryptedBytes);
        }

        private T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            T obj = (T)ser.ReadObject(ms);

            return obj;
        }

        #endregion
    }
}
