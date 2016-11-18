using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidApp.Handler;
using Xamarin.Auth;

namespace AndroidApp
{
    [Activity(Label = "Expiring Barcode Demo App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const int LOGIN_ACTIVITY = 0;
        private User user;
        private Barcode barcodeGenerator ;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            user = new User();

            // Create your application here
            SetContentView(Resource.Layout.Main);

            if (!user.IsLoggedIn)
            {
                requireLogin();
            }
            else
            {
                if (string.IsNullOrEmpty(user.GetProperty(UserConstants.SECRET)))
                {
                    var api = new Api(user);
                    await api.RequestSharedKey();
                }
                initBarcode();
            }
        }

        private void requireLogin()
        {
            var intent = new Intent(this, typeof(LoginActivity));
            StartActivityForResult(intent, LOGIN_ACTIVITY);
        }

        private void initBarcode()
        {
            this.barcodeGenerator = new Barcode(this.user);
            var barcodeImage = FindViewById<ImageView>(Resource.Id.barcodeImage);
            barcodeImage.SetImageBitmap(barcodeGenerator.CurrentCode());
            //find a way to refresh every 30 sec
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == LOGIN_ACTIVITY)
            {
                if (resultCode == Result.Ok)
                {
                    //reset user
                    user = new User();
                    initBarcode();
                }
                else
                {
                    requireLogin();
                }
            }
        }
    }
}