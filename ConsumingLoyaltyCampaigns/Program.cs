﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsumingLoyaltyCampaigns
{
    class Program
    {
        const string userName = "frankenstine@gmail.com";
        const string password = "Abcd1234.";
        const string apiBaseUri = "http://localhost:27203";
        const string apiGetPeoplePath = "/api/EmailConfiguration/1";

        static void Main(string[] args)
        {
            //Get the token
            var token = GetAPIToken(userName, password, apiBaseUri).Result;
            Console.WriteLine("Token: {0}\n\n\n", token);

            //Make the call
            var response = GetRequest(token, apiBaseUri, apiGetPeoplePath).Result;
            Console.WriteLine("response: {0}", response);

            //wait for key press to exit
            Console.ReadKey();
        }

        private static async Task<string> GetAPIToken(string userName, string password, string apiBaseUri)
        {
            using (var client = new HttpClient())
            {
                //setup client
                client.BaseAddress = new Uri(apiBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //setup login data
                var formContent = new FormUrlEncodedContent(new[]
                {
                 new KeyValuePair<string, string>("grant_type", "password"),
                 new KeyValuePair<string, string>("username", userName),
                 new KeyValuePair<string, string>("password", password),
                });

                //send request
                HttpResponseMessage responseMessage = await client.PostAsync("/Token", formContent);

                //get access token from response body
                var responseJson = await responseMessage.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(responseJson);
                return jObject.GetValue("access_token").ToString();
            }
        }

        static async Task<string> GetRequest(string token, string apiBaseUri, string requestPath)
        {
            using (var client = new HttpClient())
            {
                // Setup client
                client.BaseAddress = new Uri(apiBaseUri);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                // Make request
                HttpResponseMessage response = await client.GetAsync(requestPath);
                var responseString = await response.Content.ReadAsStringAsync();
                return responseString;
            }
        }
    }
}
