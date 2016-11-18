using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Org.Apache.Http.Client.Params;
using Shared;
using Shared.Models;
using Xamarin.Auth;

namespace AndroidApp.ApiHandler
{
    public class Api
    {
        private const string serverUrl = "http://10.0.2.2:60203/";
        private HttpClient client;
        private string token;

        public Api()
        {
            //use New API after loggin in or out
            this.client = new HttpClient();
            this.client.MaxResponseContentBufferSize = 256000;
            var acc = AccountStore.Create().FindAccountsForService("ExpiringBarcode").First();
            if (acc != null && acc.Properties.ContainsKey("Token"))
            {
                this.token = acc.Properties["Token"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        public async Task<bool> Login(string username, string password)
        {
            try
            {
                var info = new[]
                {
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("password", password),
                    new KeyValuePair<string, string>("grant_type", "password")
                };

                var content = new FormUrlEncodedContent(info);
                var response = await client.PostAsync(serverUrl + "token", content);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new UnauthorizedAccessException();
                }
                var strResponse = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<LoginResultModel>(strResponse);

                var acc = new Account(resp.access_token);
                AccountStore.Create().Save(acc, "Token");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public async Task RequestSharedKey()
        {
            DiffieHellman DH = new DiffieHellman();
            var mypublic = DH.GetMyPublic();
            var serverKey = await PostApi<string>("api/barcode", new { key = mypublic });

            var key = DH.getFinalKey(BigInteger.Parse(serverKey));

            // assume logged in if you're calling this.
            // not checking here.
            var acc = AccountStore.Create().FindAccountsForService("ExpiringBarcode").First();
            acc.Properties.Add("BarcodeSharedSecret",key.ToString()); //todo: tostring() probably doesn't work.
            AccountStore.Create().Save(acc, "userInfo");
        }

        public async Task GetMembershipId()
        {
            //TODO: make server api to return membership id and store it in acc. refer to RequestSharedKey
        }

        public async Task<RegisterResultModel> Register(string username, string password, string confirmpassword)
        {
            return await PostApi<RegisterResultModel>("api/account/register", new { username, password, confirmpassword });
        }

        private async Task<T> PostApi<T>(string invoke, dynamic param, bool throwIfNotOK = false)
        {
            var json = JsonConvert.SerializeObject(param);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(serverUrl + invoke, content);

            var strResponse = await response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK && throwIfNotOK)
            {
                throw new WebException(response.StatusCode + " " + strResponse);
            }
            return JsonConvert.DeserializeObject<T>(strResponse);
        }
    }
}