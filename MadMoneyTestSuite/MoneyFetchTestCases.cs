using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MadMoney;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MadMoneyTestSuite
{
    [TestClass]
    public class MoneyFetchTestCases
    {
        [TestMethod]
        public void FetchMoneyTestCase()
        {
            MadMoneyService madMoneyService = new MadMoneyService();
            MoneyFetchRequest request = new MoneyFetchRequest();
            request.userAddressId = "in/MH/Pune/kharadi/2e179ab097924cf2b4ae572b257f6cc2-1";

            var response = madMoneyService.FetchMoney(request);

            request.decryptdOTP = DecryptString(response.encryptedOTP, 1024, File.ReadAllText(@"D:\CreateUser\private.xml"));

            response = madMoneyService.FetchMoney(request);

            verifyMoneyWithCachierPrivateKey(response);
        }

        private void verifyMoneyWithCachierPrivateKey(MoneyFetchResponse response)
        {

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(File.ReadAllText(@"D:\CashierKeys\public.xml"));

            foreach (var money in response.moneyList)
            {
                bool verify = RSA.VerifyData(Encoding.ASCII.GetBytes(money.hash), new SHA1Managed(), Convert.FromBase64String(money.signature));
                Assert.IsTrue(verify);
            }
        }

        private string DecryptString(string inputString, int dwKeySize,
                                     string xmlString)
        {
            RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ?
              (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(
                     inputString.Substring(base64BlockSize * i, base64BlockSize));
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(
                                    encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(Type.GetType("System.Byte")) as byte[]);
        }
    }
}
