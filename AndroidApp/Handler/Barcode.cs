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
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.PDF_417,
                Options = new EncodingOptions { Width = 200, Height = 50 } //optional
            };
            var bytes = writer.Write(data);
            var imgBitMap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
            return imgBitMap;
            return null;
        }
    }
}