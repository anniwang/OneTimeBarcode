using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using AndroidApp.ApiHandler;
using Xamarin.Auth;


namespace AndroidApp
{
    [Activity(Label = "Please Log In")]
    public class LoginActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Login);

            EditText username = FindViewById<EditText>(Resource.Id.username);
            EditText password = FindViewById<EditText>(Resource.Id.password);
            Button btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            Button btnRegister = FindViewById<Button>(Resource.Id.btnRegister);
            
            btnLogin.Click += async (object sender, EventArgs e) =>
            {
                var api = new Api();
                if (await api.Login(username.Text, password.Text))
                {
                    api = new Api(); // use new API to use authenticated requests
                    api.GetMembershipId();
                    api.RequestSharedKey();

                    this.Finish();
                }
                else
                {
                    Toast.MakeText(this, "Username or Password is Incorrect", ToastLength.Short);
                }
            };
            btnRegister.Click += async (object sender, EventArgs e) =>
            {
                var intent = new Intent(this, typeof(RegisterActivity));
                StartActivity(intent);
            };
        }
        public override void OnBackPressed()
        {
            //prevent back button
        }

    }
}

