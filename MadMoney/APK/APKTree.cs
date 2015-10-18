using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MadMoney.DataBaseTools;

namespace MadMoney
{
    public class APKTree
    {
        List<APKNode> apkList;        

        APKDBTool apkdbTool;

        public APKTree()
        {
            apkdbTool = new APKDBTool();
            if (apkList == null)
                apkList = apkdbTool.Retrive();
        }

        public List<APKNode> InserUser(string address, string userPublicKey)
        {
            string[] addressArray = address.Split('/');
            int j = 1;
            int i = 0;

            if (apkList == null)
            {
                apkList = new List<APKNode>();
                apkList.Add(InitializeAPKRoot());
            }

            while (i < addressArray.Length && j < apkList.Count)
            {
                if (string.Compare(apkList[j].value, addressArray[i], true) == 0)
                {
                    j++;
                    i++;
                }
                else if (apkList[j].siblingIndex != -1)
                    j = apkList[j].siblingIndex;
                else
                {
                    apkList[j].siblingIndex = apkList.Count;
                    j = apkList.Count;
                    break;
                }
            }
            while (i < addressArray.Length)
            {
                APKNode node = new APKNode();
                node.value = addressArray[i];
                node.siblingIndex = -1;
                if (++i == addressArray.Length)                
                    node.publicKey = userPublicKey;                
                apkList.Add(node);
            }
            apkdbTool.Store(apkList);
            return apkList;
        }

        private APKNode InitializeAPKRoot()
        {
            string rootPublicKey = File.ReadAllText(@"D:\CashierKeys\public.xml");
            APKNode node = new APKNode();
            node.publicKey = rootPublicKey;
            node.value = "WORLD";
            node.siblingIndex = -1;

            return node;
        }

        public string GetPublicKey(string userAddress)
        {
            int i = 0, j = 1;
            string[] inputArray = userAddress.Split('/');
            while (i < inputArray.Length && j < apkList.Count)
            {
                if (apkList[j].value == inputArray[i])
                {
                    i++; j++;
                }
                else
                {
                    if (apkList[j].siblingIndex == -1)
                        return null;
                    else
                        j = apkList[j].siblingIndex;
                }
            }
            return apkList[j - 1].publicKey;
        }
    }
}