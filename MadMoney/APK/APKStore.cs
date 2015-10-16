using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney
{
    public class APKStore
    {
        public string value { get; set; }
        public int sublingIndex { get; set; }
        public APKStore next { get; set; }
        public string publicKey { get; set; }
    }
}