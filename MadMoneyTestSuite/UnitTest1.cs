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
    public class UnitTest1
    {
        [TestMethod]
        public void InitializeAPKTree()
        {

            //apkTree.GrowTree(newAddress.Split('/'), rootNode.childList, 0);
            //storeHead = apkTree.APKTreeInStore(rootNode);
            //FileOperation.WriteToAPKFile(storeHead);            
        }


        [TestMethod]
        public void InsertIntoAPKTreeTest()
        {
            //Variables
            APKStaticStore[] storeHead, finalStoreHead;
            APKTree apkTree = new APKTree();            
            APKNode rootNode = new APKNode("WORLD");
            


            //1. Read APKStore[] from APKTree file 
            storeHead = FileOperation.ReadAPKFile();
            //2. Build Tree from APKStore[]
            //rootNode = apkTree.BuidTree(0, storeHead.Length - 1, storeHead);
            //3. Insert New Address GrowTree
            //apkTree.GrowTree(newAddress.Split('/'), rootNode.childList, 0);
            //4. Create APKStore[] from APKTree
            //storeHead = apkTree.APKTreeInStore(rootNode);
            //5. Write to file
            FileOperation.WriteToAPKFile(storeHead);
            //6.Assertion
            finalStoreHead = FileOperation.ReadAPKFile();
            Assert.AreEqual(finalStoreHead.Length, storeHead.Length);



            //var result = apkTree.GetPublicKey(address1.Split('/'), storeHead);                                   

            //apkTree.GrowTree(address5.Split('/'), node.childList, 0);            
        }


        [TestMethod]
        public void PublicKeyGeneration()
        {
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = "Cashier forging key";

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(cspParams);           

            File.WriteAllText(@"D:\CashierKeys\private.xml", RSA.ToXmlString(true));  // secure and save this dude

            File.WriteAllText(@"D:\CashierKeys\public.xml", RSA.ToXmlString(false)); // send this to all 
            //PublicKeyClient();
        }

        [TestMethod]
        public void PublicKeyClient()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024);
            RSA.FromXmlString(File.ReadAllText(@"D:\public.xml"));
            byte[] toEncryptData = Encoding.Unicode.GetBytes("This is the data to encrypt");
            byte[] encryptedData = RSA.Encrypt(toEncryptData, false);
            File.WriteAllText(@"D:\encryptedMessage.txt", Encoding.Unicode.GetString(encryptedData));

            PrivateKeyServer();
        }


        [TestMethod]
        public void PrivateKeyServer()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024);
            RSA.FromXmlString(File.ReadAllText(@"D:\public.xml"));
            RSA.FromXmlString(File.ReadAllText(@"D:\private.xml"));
            byte[] encryptedData = Encoding.Unicode.GetBytes(File.ReadAllText(@"D:\encryptedMessage.txt"));
            
            byte[] decryptedData = RSA.Decrypt(encryptedData, false);

            File.WriteAllText(@"D:\decryptedMessage.txt", Encoding.Unicode.GetString(decryptedData));
        }

        [TestMethod]
        public void SignPublicKeyClient()
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(File.ReadAllText(@"D:\CashierKeys\private0.xml"));
            byte[]  toEncryptData = Encoding.ASCII.GetBytes("This is the data to encrypt");
            byte[]  encryptedData = RSA.SignData(toEncryptData, new SHA1Managed());
            File.WriteAllText(@"D:\signedMessage.txt", Encoding.GetEncoding("ISO-8859-1").GetString(encryptedData));

            SignPrivateKeyServer();
        }


        public void SignPrivateKeyServer()
        {
            byte[] toEncryptData = Encoding.ASCII.GetBytes("This is the data to encrypt");
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(File.ReadAllText(@"D:\CashierKeys\public.xml"));

            byte[] encryptedData = Encoding.GetEncoding("ISO-8859-1").GetBytes(File.ReadAllText(@"D:\signedMessage.txt"));
            bool verify = RSA.VerifyData(toEncryptData, new SHA1Managed(), encryptedData);
            
        }

        public string EncryptString(string inputString, int dwKeySize,
                             string xmlString)
        {
            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider =
                                          new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int keySize = dwKeySize / 8;
            byte[] bytes = Encoding.UTF32.GetBytes(inputString);
            // The hash function in use by the .NET RSACryptoServiceProvider here 
            // is SHA1
            // int maxLength = ( keySize ) - 2 - 
            //              ( 2 * SHA1.Create().ComputeHash( rawBytes ).Length );
            int maxLength = keySize - 42;
            int dataLength = bytes.Length;
            int iterations = dataLength / maxLength;
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i <= iterations; i++)
            {
                byte[] tempBytes = new byte[
                        (dataLength - maxLength * i > maxLength) ? maxLength :
                                                      dataLength - maxLength * i];
                Buffer.BlockCopy(bytes, maxLength * i, tempBytes, 0,
                                  tempBytes.Length);
                byte[] encryptedBytes = rsaCryptoServiceProvider.Encrypt(tempBytes,
                                                                          true);
                // Be aware the RSACryptoServiceProvider reverses the order of 
                // encrypted bytes. It does this after encryption and before 
                // decryption. If you do not require compatibility with Microsoft 
                // Cryptographic API (CAPI) and/or other vendors. Comment out the 
                // next line and the corresponding one in the DecryptString function.
                Array.Reverse(encryptedBytes);
                // Why convert to base 64?
                // Because it is the largest power-of-two base printable using only 
                // ASCII characters
                stringBuilder.Append(Convert.ToBase64String(encryptedBytes));
            }
            return stringBuilder.ToString();
        }

        public string DecryptString(string inputString, int dwKeySize,
                                     string xmlString)
        {
            // TODO: Add Proper Exception Handlers
            RSACryptoServiceProvider rsaCryptoServiceProvider
                                     = new RSACryptoServiceProvider(dwKeySize);
            rsaCryptoServiceProvider.FromXmlString(xmlString);
            int base64BlockSize = ((dwKeySize / 8) % 3 != 0) ?
              (((dwKeySize / 8) / 3) * 4) + 4 : ((dwKeySize / 8) / 3) * 4;
            int iterations = inputString.Length / base64BlockSize;
            ArrayList arrayList = new ArrayList();
            for (int i = 0; i < iterations; i++)
            {
                byte[] encryptedBytes = Convert.FromBase64String(
                     inputString.Substring(base64BlockSize * i, base64BlockSize));
                // Be aware the RSACryptoServiceProvider reverses the order of 
                // encrypted bytes after encryption and before decryption.
                // If you do not require compatibility with Microsoft Cryptographic 
                // API (CAPI) and/or other vendors.
                // Comment out the next line and the corresponding one in the 
                // EncryptString function.
                Array.Reverse(encryptedBytes);
                arrayList.AddRange(rsaCryptoServiceProvider.Decrypt(
                                    encryptedBytes, true));
            }
            return Encoding.UTF32.GetString(arrayList.ToArray(
                                      Type.GetType("System.Byte")) as byte[]);
        }

        [TestMethod]
        public void TestRSAEncryption() { 
            string inputString ="This is the data to encrypt. so all the best";
            int dwKeySize = 1024;
            string encryptedString = EncryptString(inputString, dwKeySize, File.ReadAllText(@"D:\public.xml"));
            string descryptedString = DecryptString(encryptedString, dwKeySize, File.ReadAllText(@"D:\private.xml"));
        }
    }

}
