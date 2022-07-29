using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    class Program
    {

        static void Main(string[] args)
        {
            GetToken();
        }

        private static void GetToken()
        {
            var authUrl = "https://auth.domain.com.au/v1/connect/token";
            var clientId = "gingin.gui@gmail.com";
            var clientSecret = "Trading0!";

            var postData = "grant_type=client_credentials&scope=api_agencies_read%20api_listings_read";

            var client = new System.Net.Http.HttpClient();

            client.DefaultRequestHeaders.Authorization =
              new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));

            var result = client.PostAsync(new Uri(authUrl), new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"));

            var token = result.Result.Content.ReadAsStringAsync().Result;
        }
    }
}
