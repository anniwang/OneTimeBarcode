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
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            user = new User();

            // Create your application here
            SetContentView(Resource.Layout.Main);

            if (!user.IsLoggedIn)
            {
                requireLogin();
            }
        }

        private void requireLogin()
        {
            var intent = new Intent(this, typeof(LoginActivity));
            StartActivityForResult(intent, LOGIN_ACTIVITY);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == LOGIN_ACTIVITY)
            {
                if (resultCode == Result.Ok)
                {
                    //reset user
                    user = new User();
                }
                else
                {
                    requireLogin();
                }
            }
        }
    }
}