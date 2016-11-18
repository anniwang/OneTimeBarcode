using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Shared;
using ZXing;
using ZXing.Common;

namespace AndroidApp.Handler
{
    public class Barcode
    {
        private User user;
        private string memberid;
        private TOTP totp;

        public Barcode(User _user)
        {
            if (!_user.IsLoggedIn || string.IsNullOrEmpty(_user.GetProperty(UserConstants.SECRET)))
            {
                throw new UnauthorizedAccessException();
            }
            this.user = _user;
            this.memberid = user.GetProperty(UserConstants.MemberID);
            var secret = user.GetProperty(UserConstants.SECRET);
            var secretBytes = Convert.FromBase64String(secret);
            this.totp = new TOTP(secretBytes);
        }

        public Bitmap CurrentCode()
        {
            var data = memberid + totp.GetCode();
            return this.makeBarcode(data);
        }

        private Bitmap makeBarcode(string data)
        {
            var writer = new ZXing.Mobile.BarcodeWriter
            {
                Format = BarcodeFormat.PDF_417,
                Options = new EncodingOptions { Width = 1000, Height = 400 }
            };
            return writer.Write(data);
        }
    }
}