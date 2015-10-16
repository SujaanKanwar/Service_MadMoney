using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using System.Text;

namespace CSEncryptDecrypt
{
    class Class1
    {
        static void Main()
        {

            string sSecretKey = "sujansingh";
                        
            GCHandle gch = GCHandle.Alloc(sSecretKey, GCHandleType.Pinned);

            // Encrypt the file.        
            EncryptFile(@"D:\MyData.txt",
               @"D:\Encrypted.txt",
               sSecretKey);

            // Decrypt the file.
            DecryptFile(@"D:\Encrypted.txt",
               @"D:\Decrypted.txt",
               sSecretKey);

            // Remove the Key from memory.             
            gch.Free();
        }

        private static void EncryptFile(string inputFile, string outputFile, string password)
        {
            try
            {
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch (Exception e)
            {
                throw new Exception("Encryption failed! Error", e);
            }
        }

        private static void DecryptFile(string inputFile, string outputFile, string password)
        {
            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] key = UE.GetBytes(password);

            FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

            RijndaelManaged RMCrypto = new RijndaelManaged();

            CryptoStream cs = new CryptoStream(fsCrypt,
                RMCrypto.CreateDecryptor(key, key),
                CryptoStreamMode.Read);

            FileStream fsOut = new FileStream(outputFile, FileMode.Create);

            int data;
            while ((data = cs.ReadByte()) != -1)
                fsOut.WriteByte((byte)data);

            fsOut.Close();
            cs.Close();
            fsCrypt.Close();
        }
    }


}