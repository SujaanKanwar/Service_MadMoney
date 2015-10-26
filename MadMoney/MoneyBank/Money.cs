using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MadMoney.MoneyBank
{
    public class Money
    {        
        public string id { get; private set; }
        public int value { get; private set; }
        public string hash { get; private set; }
        //Cashier to set
        public string ownerId { get; private set; }
        public string dated { get; private set; }
        public byte[] signature { get; private set; }

        //While money retrieval from money bank
        public Money(string id, int value)
        {
            this.id = id;
            this.value = value;
        }
        //while money generation
        public Money(int value)
        {
            this.value = value;
            this.id = GenerateMoneyId(value);
        }

        private string GenerateMoneyId(int value)
        {
            return Guid.NewGuid().ToString().Replace("-","") + value;
        }

        public void Initialize(string ownerId, string cashierPrivateKeyXmlFile) {
            SetDate();
            SetOwner(ownerId);
            SetHash();            
            SignMoney(cashierPrivateKeyXmlFile);
        }

        private void SetOwner(string ownerId)
        {
            this.ownerId = ownerId;
        }

        private void SignMoney(string cashierPrivateKeyXmlFile)
        {
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(File.ReadAllText(cashierPrivateKeyXmlFile));
            byte[] hash = Encoding.ASCII.GetBytes(this.hash);
            byte[] encryptedData = RSA.SignData(hash, new SHA1Managed());

            this.signature = encryptedData;
        }

        private void SetDate() {
            this.dated = DateTime.Now.ToString();
        }

        private void SetHash()
        {            
            string toHash = this.id + this.value + this.ownerId + this.dated;

            byte[] bytes = Encoding.Unicode.GetBytes(toHash);
            SHA256Managed sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }
            this.hash = hashString;
        }
    }

}
