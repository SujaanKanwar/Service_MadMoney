using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MadMoney.ServiceData
{
    public static class Constants
    {
        public static class FetchMoneyRequest
        {
            public const string INIT = "INIT";
            public const string DECRYPTED_OTP = "DECRYPTED_OTP";
            public const string RECEIVED_OK = "RECEIVED_OK";
        }

        public static class FetchMoneyResponse
        {
            public const string EMPTY_AC = "EMPTY_AC";
            public const string OTP_SENT = "OTP_SENT";
            public const string MONEY_SENT = "MONEY_SENT";            
            public const string OTP_MISMATCHED = "OTP_MISMATCHED";
        }
    }
}