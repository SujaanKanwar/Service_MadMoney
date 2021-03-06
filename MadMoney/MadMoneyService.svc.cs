﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Transactions;
using MadMoney.Cashier;
using MadMoney.DataBaseTools;
using MadMoney.DataClasses;
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

        public APKFileResponse GetAPKFile()
        {
            return new APKFileResponse(true, apkTree.GetAPKTree());
        }

        public bool RegenerateSmallerMoney(RegenerateSmallerMoneyRequest request)
        {
            if (IsValidRequest(request))
            {
                try
                {
                    using (var scope = new System.Transactions.TransactionScope())
                    {
                        VerifyMoney(request.money);

                        CheckOldMoneyStore(request.money);

                        StoreMoneyInOldMoneyStore(request.money);

                        CashierServiceWrapper.DepositSmallerMoney(request.userAddressId, request.money.value);

                        scope.Complete();
                    }
                }
                catch (Exception )
                {
                    //Important log.. Need to log the cause of the failure
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool DepositMoneyToAccount(DepositMoneyToAcountRQ request)
        {
            if (IsValidRequest(request))
            {
                try
                {
                    if (IsValidMoneyList(request.moneyList))
                    {
                        using (var scope = new System.Transactions.TransactionScope(TransactionScopeOption.Suppress, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
                        {
                            foreach (var money in request.moneyList)
                            {
                                StoreMoneyInOldMoneyStore(money);

                                CashierServiceWrapper.DepositSameAmount(request.userAddressId, money.value);
                            }

                            scope.Complete();
                        }
                    }
                }
                catch (Exception )
                {
                    //Important log.. Need to log the cause of the failure
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool DepositMoneyToBankAccount(DepositMoneyToBankAcountRQ request)
        {
            if (IsValidRequest(request))
            {
                try
                {
                    if (IsValidMoneyList(request.moneyList) && IsNotTravelMoney(request))
                    {
                        using (var scope = new System.Transactions.TransactionScope())
                        {
                            foreach (var money in request.moneyList)
                            {
                                StoreMoneyInOldMoneyStore(money);

                                StoreDepositRequest(request);
                            }

                            scope.Complete();
                        }
                    }
                }
                catch (Exception )
                {
                    //Important log.. Need to log the cause of the failure
                    return false;
                }
                return true;
            }
            return false;
        }

        public bool SaveTeleportLocation(SaveTeleportLocationRQ request)
        {
            if (IsValidRequest(request))
            {
                try
                {
                    if (IsValidLocationsData(request.TLocations))
                    {
                        using (var scope = new System.Transactions.TransactionScope())
                        {
                            SaveTeleportingLocations(request.TLocations);

                            scope.Complete();
                        }
                        return true;
                    }
                }
                catch (Exception)
                { return false; }
            }
            return false;
        }

        public GetTLocationsRS GetTLocations(GetTlocationsRQ request)
        {
            bool isSuccess = false;
            List<GeofenceLocation> positions = null;

            if (IsValidRequest(request))
            {
                try
                {
                    positions = GetTLocationsFromDB(request.currentLocation);
                    isSuccess = true;
                }
                catch (Exception )
                {

                }
            }
            return new GetTLocationsRS(positions, isSuccess);
        }

        private List<GeofenceLocation> GetTLocationsFromDB(string city)
        {
            TelePortingLocationDb tLocationDb = new TelePortingLocationDb();
            return tLocationDb.getTLocations(city);
        }

        private bool IsValidRequest(GetTlocationsRQ request)
        {
            if (string.IsNullOrEmpty(request.currentLocation))
                return false;
            return true;
        }


        #region RegenerateSmallerMoney

        private bool IsNotTravelMoney(DepositMoneyToBankAcountRQ request)
        {
            foreach (var money in request.moneyList)
            {
                if (string.Compare(money.ownerId, request.userAddressId, true) != 0)
                    return false;
            }
            return true;
        }

        private bool IsValidMoneyList(List<Money> moneyList)
        {
            foreach (var money in moneyList)
            {
                VerifyMoney(money);

                CheckOldMoneyStore(money);
            }
            return true;
        }

        private bool IsValidRequest(DepositMoneyToAcountRQ request)
        {
            if (request.moneyList == null
                || string.IsNullOrEmpty(request.userAddressId)
                || string.IsNullOrEmpty(usersDBTool.ValidateUsers(request.userAddressId)))
                return false;
            return true;
        }

        private bool IsValidRequest(DepositMoneyToBankAcountRQ request)
        {
            if (request.moneyList == null
                || string.IsNullOrEmpty(request.userAddressId)
                || string.IsNullOrEmpty(usersDBTool.ValidateUsers(request.userAddressId)))
                return false;
            return true;
        }

        private bool IsValidRequest(RegenerateSmallerMoneyRequest request)
        {
            if (request.money.value < 5
                || string.IsNullOrEmpty(request.userAddressId)
                || string.IsNullOrEmpty(usersDBTool.ValidateUsers(request.userAddressId)))
                return false;
            return true;
        }

        private bool IsValidRequest(SaveTeleportLocationRQ request)
        {
            if (request.TLocations != null)
                return true;
            return false;
        }

        private void StoreMoneyInOldMoneyStore(Money money)
        {
            OldMoneyDBTool oldMoneyTool = new OldMoneyDBTool();
            oldMoneyTool.Store(money);
        }

        private void CheckOldMoneyStore(Money money)
        {
            OldMoneyDBTool oldMoneyTool = new OldMoneyDBTool();
            if (oldMoneyTool.CheckPreviousMoneyEntry(money))
                throw new Exception("WARNING: Money already in old db store");
        }

        private void VerifyMoney(Money money)
        {
            try
            {
                string hash = GenerateHash(money.id + money.value + money.ownerId + money.dated);
                RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
                RSA.FromXmlString(File.ReadAllText(@"D:\CashierKeys\public.xml"));
                bool verify = RSA.VerifyData(Encoding.ASCII.GetBytes(hash), new SHA1Managed(), Convert.FromBase64String(money.signature));
                if (verify != true)
                {
                    throw new Exception("Exception while Verify money");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Exception while Verify money", e);
            }
        }

        private string GenerateHash(string toHash)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(toHash);
            SHA256Managed sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            return hashString;
        }

        private void StoreDepositRequest(DepositMoneyToBankAcountRQ request)
        {
            int totalAmount = CalculateTotalAmount(request.moneyList);

            ReportsDBTool reportsDbTool = new ReportsDBTool();

            reportsDbTool.Store(request, totalAmount);
        }

        private int CalculateTotalAmount(List<Money> moneyList)
        {
            int totalAmount = 0;
            foreach (var money in moneyList)
            {
                totalAmount += money.value;
            }
            return totalAmount;
        }
        #endregion

        #region private functions

        private UsersDBTool usersDBTool = new UsersDBTool();

        private CachierDBTool cachierDbTool = new CachierDBTool();

        private APKTree apkTree = APKTree.APKTreeInstance();
        private DiscoveredLocationsDBTool discoveredLocationdb;

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
            RSACryptoServiceProvider rsaCryptoServiceProvider = RSACryptoServiceProviderFromPublicKey(jsonPublicKey);

            byte[] bytes = Encoding.UTF8.GetBytes(inputString);

            byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(bytes, false);

            return Convert.ToBase64String(encryptedBytes);
        }

        private RSACryptoServiceProvider RSACryptoServiceProviderFromPublicKey(string jsonPublicKey)
        {
            RSAParameters rsaParameter = new RSAParameters();

            PublicKey publicKey = JsonDeserialize<PublicKey>(jsonPublicKey);

            rsaParameter.Exponent = Convert.FromBase64String(publicKey.EXP);

            rsaParameter.Modulus = Convert.FromBase64String(publicKey.MOD);

            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider();

            rsaCryptoServiceProvider.ImportParameters(rsaParameter);

            return rsaCryptoServiceProvider;
        }

        private T JsonDeserialize<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));

            T obj = (T)ser.ReadObject(ms);

            return obj;
        }

        private bool IsValidLocationsData(Location[] tLocationArray)
        {
            foreach (var location in tLocationArray)
            {
                float latitude, longitude;
                int radius, rating;
                string tLocationName;
                float.TryParse(location.latitude, out latitude);
                float.TryParse(location.longitude, out longitude);
                int.TryParse(location.radius, out radius);
                rating = location.rating;
                tLocationName = location.locationName;

                if (latitude == null || longitude == null || radius == null || rating < 0
                    || rating > 10 || string.IsNullOrEmpty(tLocationName)
                    || string.IsNullOrEmpty(location.city))
                    return false;
            }
            return true;
        }

        private void SaveTeleportingLocations(Location[] locations)
        {
            TelePortingLocationDb tpLocationDb = new TelePortingLocationDb();
            List<GeofenceLocation> geofenceLocations = new List<GeofenceLocation>();

            foreach (var location in locations)
            {
                geofenceLocations.Add(GeofenceLocation.get(location));
            }
            tpLocationDb.saveLocations(geofenceLocations);
        }

        #endregion


        public bool DiscoveredLocations(DiscoveredLocationsRQ request)
        {
            if (IsValidRequest(request))
            {
                try
                {
                    if (IsValidLocationsData(request))
                    {
                        using (var scope = new System.Transactions.TransactionScope())
                        {
                            SaveDiscoveredLocationsWRTUserAddress(request);

                            scope.Complete();
                        }
                        return true;
                    }
                }
                catch (Exception)
                { return false; }
            }
            return false;
        }

        private void SaveDiscoveredLocationsWRTUserAddress(DiscoveredLocationsRQ request)
        {
            DiscoveredLocationsDBTool discoveredLocationDB = new DiscoveredLocationsDBTool();
            foreach (var location in request.discoveredLocations)
            {
                discoveredLocationDB.StoreUserDiscoveredLocations(request.userAddressId, location.RequestId, location.DateAndTimeOfDiscover);
            }
        }

        private bool IsValidLocationsData(DiscoveredLocationsRQ request)
        {
            string userAddressId = request.userAddressId;
            if (IsValidUserAddress(userAddressId))
            {
                var userPublicKey = APKTree.APKTreeInstance().GetPublicKey(userAddressId);
                string verifyigData = GetVerifyingRequest(request);
                if (!string.IsNullOrEmpty(userPublicKey) && IsValidSignature(userPublicKey, verifyigData, request.signature))
                    return true;
            }
            return false;
        }

        private bool IsValidSignature(string userPublicKey, string verifyigData, string signature)
        {
            try
            {
                string hash = GenerateHash(verifyigData);
                RSACryptoServiceProvider RSA = RSACryptoServiceProviderFromPublicKey(userPublicKey);
                bool verify = RSA.VerifyData(Encoding.UTF8.GetBytes(verifyigData), new SHA256Managed(), Convert.FromBase64String(signature));
                if (verify == true)
                    return true;
            }
            catch (Exception )
            { }
            return false;
        }

        private string GetVerifyingRequest(DiscoveredLocationsRQ request)
        {
            string result = "";
            foreach (var pos in request.discoveredLocations)
            {
                result += pos.RequestId;
                result += pos.DateAndTimeOfDiscover;
            }
            result += request.userAddressId;
            return result;
        }

        private bool IsValidUserAddress(string userAddressId)
        {
            usersDBTool.ValidateUsers(userAddressId);
            return true;
        }

        private bool IsValidRequest(DiscoveredLocationsRQ request)
        {
            if (request.discoveredLocations == null || request.discoveredLocations.Count() == 0 ||
                string.IsNullOrEmpty(request.userAddressId) || string.IsNullOrEmpty(request.signature))
                return false;
            return true;
        }



        public bool BackgroundTasks()
        {
            try
            {
                DiscoveredLocationTask();
            }
            catch (Exception )
            {
                return false;
            }
            return true;
        }

        private void DiscoveredLocationTask()
        {
            discoveredLocationdb = new DiscoveredLocationsDBTool();
            List<UserGeoDiscoveredLocation> discoveredLocationList = discoveredLocationdb.SelectAllDiscoveredLocations();

            foreach (var location in discoveredLocationList)
            {
                HandleDiscoveredLocation(location);
            }
        }

        private void HandleDiscoveredLocation(UserGeoDiscoveredLocation location)
        {
            try {
                TelePortingLocationDb teleDb = new TelePortingLocationDb();
                var locationData = teleDb.getTLocation(location.geoRequestId);

                var userAddressId = location.userAddressId;

                usersDBTool.ValidateUsers(userAddressId);

                int amountToDeposit = CalculateAmountToDepositOfTelePosition(locationData);

                CashierServiceWrapper.DepositSameAmount(userAddressId, amountToDeposit);

                discoveredLocationdb.UpdateOperationStatus(location.id);
            }
            catch (Exception e) { throw new Exception("Error while handling geo location " 
                + location.geoRequestId + " of " + location.userAddressId + e.StackTrace); }
        }

        private int CalculateAmountToDepositOfTelePosition(GeofenceLocation locationData)
        {
            int value = 0;
            var rating = locationData.rating;
            value = rating * 2;
            return value;
        }

    }
}
