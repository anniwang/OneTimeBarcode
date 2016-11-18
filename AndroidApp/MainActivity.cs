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
using Xamarin.Auth;

namespace AndroidApp
{
    [Activity(Label = "Expiring Barcode Demo App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Main);
            
            var acc = AccountStore.Create().FindAccountsForService("ExpiringBarcode").First();
            if (acc == null || string.IsNullOrEmpty(acc.Username) || !acc.Properties.ContainsKey("Token"))
                // Also should check for Expires. but having trouble deserializing that rn.
            {
                requireLogin();
            }
        }

        private void requireLogin()
        {
            var intent = new Intent(this, typeof(LoginActivity));
            StartActivity(intent);
        }
    }
}