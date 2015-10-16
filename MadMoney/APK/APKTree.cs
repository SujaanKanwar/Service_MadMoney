using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MadMoney
{
    public class APKTree
    {
        private APKNode rootNode = null;
        APKStaticStore[] storeHead;
        string userCreatePublicKey;

        public APKTree()
        {
            storeHead = FileOperation.ReadAPKFile();
            if (storeHead != null)
                rootNode = BuidTree(0, storeHead.Length - 1, storeHead);
        }

        public APKStaticStore[] InserUser(string address, string userPublicKey)
        {
            if (rootNode == null)
                rootNode = InitializeAPKRoot();

            userCreatePublicKey = userPublicKey;

            GrowTree(address.Split('/'), rootNode.childList, 0);
            
            storeHead = APKTreeInStore(rootNode);
            
            FileOperation.WriteToAPKFile(storeHead);

            return storeHead;
        }

        private APKNode InitializeAPKRoot()
        {
            string rootPublicKey = File.ReadAllText(@"D:\CashierKeys\public.xml");
            return new APKNode("WORLD", rootPublicKey);
        }

        public string GetPublicKey(string userAddress)
        {            
            int i = 0, j = 1;
            string[] inputArray = userAddress.Split('/');            
            while (i < inputArray.Length && j < storeHead.Length)
            {
                if (storeHead[j].value == inputArray[i])
                {
                    i++; j++;
                }
                else
                {
                    if (storeHead[j].siblingIndex == -1)
                        return null;
                    else
                        j = storeHead[j].siblingIndex;
                }
            }
            return storeHead[j - 1].publicKey;
        }

        #region private functions

        private void GrowTree(string[] address, List<APKNode> nodeList, int i)
        {
            if (i >= address.Length)
                return;
            if (nodeList.Count == 0 || SearchForValueInList(nodeList, address[i]) == -1)
            {
                APKNode newNode = CreateNewNode(address, i);
                nodeList.Add(newNode);
                
                GrowTree(address, newNode.childList, i + 1);

                return;
            }
            APKNode node = nodeList.ElementAt(SearchForValueInList(nodeList, address[i]));

            GrowTree(address, node.childList, i + 1);

            return;
        }

        private int SearchForValueInList(List<APKNode> nodeList, string value)
        {
            //TODO: use binary search
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (string.Compare(nodeList[i].value, value, true) == 0)
                    return i;
            }
            return -1;
        }

        private APKNode CreateNewNode(string[] address, int index)
        {
            if (index + 1 == address.Length)
                return new APKNode(address[index], userCreatePublicKey);
            return new APKNode(address[index]);
        }

        private APKStaticStore[] APKTreeInStore(APKNode rootNode)
        {
            int count = 0;
            int i = 0;
            APKStore dianamicArray = APKTreeInArray(rootNode, ref count);
            APKStaticStore[] staticArray = new APKStaticStore[count];
            APKStore node = dianamicArray;
            while (node != null)
            {
                staticArray[i] = new APKStaticStore();
                staticArray[i].siblingIndex = node.sublingIndex;
                staticArray[i].publicKey= node.publicKey;
                staticArray[i++].value = node.value;
                

                node = node.next;
            }
            return staticArray;
        }

        private APKStore APKTreeInArray(APKNode node, ref int count)
        {
            if (node == null) return null;
            APKStore headNode = new APKStore();
            APKStore newHead = new APKStore();
            CopyValue(headNode, node);
            newHead = headNode;
            count++;
            for (int i = 0; i < node.childList.Count; i++)
            {
                newHead.next = APKTreeInArray(node.childList[i], ref count);
                if (i + 1 < node.childList.Count)
                    newHead.next.sublingIndex = count;
                newHead = LastNode(newHead);
            }
            return headNode;
        }

        private APKStore LastNode(APKStore newHead)
        {
            while (newHead.next != null)
            {
                newHead = newHead.next;
            }
            return newHead;
        }

        private void CopyValue(APKStore headNode, APKNode node)
        {
            headNode.next = null;
            headNode.sublingIndex = -1;
            headNode.value = node.value;
            headNode.publicKey = node.publicKey;
        }        

        private APKNode BuidTree(int startIndex, int endIndex, APKStaticStore[] storedArray)
        {
            APKNode newNode = null;
            if (startIndex > endIndex || startIndex > storedArray.Length) return null;

            APKNode node = new APKNode(storedArray[startIndex].value,storedArray[startIndex].publicKey);
            startIndex++;
            if (startIndex < storedArray.Length)
            {
                int siblingIndex = storedArray[startIndex].siblingIndex;

                if (siblingIndex == -1)
                    newNode = BuidTree(startIndex, endIndex, storedArray);
                if (newNode != null)
                    node.childList.Add(newNode);
                else
                {
                    while (true)
                    {
                        newNode = BuidTree(startIndex, siblingIndex - 1, storedArray);
                        if (newNode != null)
                            node.childList.Add(newNode);
                        if (startIndex > siblingIndex || siblingIndex > endIndex)
                            break;
                        startIndex = siblingIndex;
                        if (storedArray[siblingIndex].siblingIndex == -1)
                            siblingIndex = storedArray.Length;
                        else
                            siblingIndex = storedArray[siblingIndex].siblingIndex;
                    }
                }
            }

            return node;
        }

        #endregion
    }
}