using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Yort.Otp;

namespace Shared
{
    public class TOTP
    {
        private const int digits = 8;
        private const int period = 10;
        private string secret;
        private TimeBasedPasswordGenerator totp;

        public TOTP(byte[] secret)
        {
            this.totp = new TimeBasedPasswordGenerator(true, secret);
            this.totp.TimeInterval = TimeSpan.FromSeconds(period);
            this.totp.PasswordLength = digits;
        }

        public string GetCode()
        {
            return totp.GeneratedPassword;
        }

        public bool ConfirmCode(string code)
        {
            return this.totp.GeneratedPassword.Equals(code);
        }
    }
}
