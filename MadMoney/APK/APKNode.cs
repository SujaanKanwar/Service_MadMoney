using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney
{
    public class APKNode
    {             

        public List<APKNode> childList { get; private set; }
        public string value { get; private set; }
        public string publicKey { get; set; }

        public APKNode(string value)
        {           
            this.value = value;
            this.childList = new List<APKNode>();
        }

        public APKNode(string value, string userCreatePublicKey)
        {
            this.value = value;
            this.childList = new List<APKNode>();
            this.publicKey = userCreatePublicKey;
        }
    }
}