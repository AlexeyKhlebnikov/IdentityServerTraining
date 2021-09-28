using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ConsoleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Client startup");
            using var httpClient = new HttpClient();
            try
            {
                var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5001");
                if (disco.IsError)
                {
                    Console.WriteLine(disco.Error);
                    return;
                }

                var token = await RequestToken(httpClient, disco);
                await RequestIdentities(httpClient, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task RequestIdentities(HttpClient httpClient, TokenResponse token)
        {
            httpClient.SetBearerToken(token.AccessToken);
            var response = await httpClient.GetAsync("https://localhost:6001/Identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }

            var stringResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine(stringResponse);
        }

        private static async Task<TokenResponse> RequestToken(HttpClient httpClient, DiscoveryDocumentResponse disco)
        {
            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "SpecTechMobile",
                ClientSecret = "spectechmobi",
                Scope = "api"
            });

            if (!tokenResponse.IsError) return tokenResponse;

            throw new Exception(tokenResponse.Error);
        }
    }
}