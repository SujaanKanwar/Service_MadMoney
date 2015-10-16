using System;
using System.IO;
using System.Security.Cryptography;
using MadMoney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MadMoneyTestSuite
{
    [TestClass]
    public class CreateUserTestCases
    {
        [TestMethod]
        public void CreateUserTests()
        {
            UserCreateRequest request = new UserCreateRequest();
            InitializeCreateUserRequest(request);
            MadMoneyService madMoneyService = new MadMoneyService();
            var response = madMoneyService.CreateUser(request);
            Assert.IsTrue(response.isSuccess);
            Assert.IsNotNull(response.apkTree);
            Assert.IsNotNull(response.uid);
        }

        private void InitializeCreateUserRequest(UserCreateRequest request)
        {
            Address address = new Address();
            address.CountryCode = "IN";
            address.State = "MH";
            address.City = "PUNE";
            address.Local = "KHARADI";
            request.Address = address;
            request.FName = "SUJAN";
            request.LName = "SINGH";
            request.PublicKey = GetPublicKey();

        }

        private string GetPublicKey()
        {            
            //CspParameters cspParams = new CspParameters();
            //cspParams.KeyContainerName = "User Key Generation";
            //RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cspParams);
            //File.WriteAllText(@"D:\CreateUser\private.xml", RSA.ToXmlString(true));  // secure and save this dude
            //File.WriteAllText(@"D:\CreateUser\public.xml", RSA.ToXmlString(false)); // send this to all 
            return File.ReadAllText(@"D:\CreateUser\public.xml");            
        }
    }
}
