using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using OtpSharp;

namespace Shared
{
    public class TOTP
    {
        private const int digits = 8;
        private const int period = 10;
        private string secret;
        private Totp totp;

        public TOTP(byte[] secret)
        {
            this.totp = new Totp(secret, period, OtpHashMode.Sha512,digits);
        }

        public string GetCode()
        {
            return totp.ComputeTotp();
        }

        public bool ConfirmCode(string code)
        {
            long ts;
            return totp.VerifyTotp(code, out ts);
        }
    }
}
