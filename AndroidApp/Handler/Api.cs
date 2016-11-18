using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Shared;
using Shared.Models;
using Xamarin.Auth;

namespace AndroidApp.Handler
{
    public class Api
    {
        private const string serverUrl = "http://10.0.2.2:60203/";
        private HttpClient client;
        private string token;
        private User user;

        public Api(User user)
        {
            this.user = user;
            initClient();
        }

        private void initClient()
        {
            this.client = new HttpClient(new HttpClientHandler(), false);
            this.client.MaxResponseContentBufferSize = 256000;
            if (user.IsLoggedIn)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                    user.GetProperty(UserConstants.Token));
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
                    return false;
                }
                var strResponse = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<LoginResultModel>(strResponse);

                user.SetProperty(UserConstants.Token, resp.access_token);
                initClient();
                return true;
            }
            catch (WebException ex)
            {
                using (var stream = ex.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var err = reader.ReadToEnd();
                    Console.WriteLine(err);
                }
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
            var stringKey = Convert.ToBase64String(key);
            user.SetProperty(UserConstants.SECRET,stringKey);
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