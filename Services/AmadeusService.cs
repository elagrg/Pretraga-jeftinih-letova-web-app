//**dotnet add package amadeus-dotnet
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using FlightSearch.Models;
using amadeus;
using amadeus.resources;
using amadeus.shopping;
using amadeus.exceptions;

namespace FlightSearch.Services
{
    public class AmadeusService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _tokenUrl = "https://test.api.amadeus.com/v1/security/oauth2/token";
        private readonly string _flightOffersUrl = "https://test.api.amadeus.com/v2/shopping/flight-offers";
        private readonly IHttpClientFactory _httpClientFactory;

        public AmadeusService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _clientId = "peVdw1NL5Pq7It5YEbacPA8kihftjqOL";     
            _clientSecret = "HlpfNsCEcRzFhpG8";     
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetAccessToken()
        {
            var client = _httpClientFactory.CreateClient();

            var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _clientId),
                new KeyValuePair<string, string>("client_secret", _clientSecret)
            });

            var response = await client.PostAsync(_tokenUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenObj = JObject.Parse(responseContent);
            return tokenObj["access_token"].ToString();
        }

        public async Task<FlightOfferResponse> GetFlightOffersAsync(FlightOfferSearch request)
        {
            var token = await GetAccessToken();
    
            using (var client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var queryString = string.Join("&",
                    $"originLocationCode={request.OriginLocationCode}",
                    $"destinationLocationCode={request.DestinationLocationCode}",
                    $"departureDate={request.DepartureDate}",
                    $"returnDate={request.ReturnDate}",
                    $"adults={request.Adults}",
                    $"currencyCode={request.Currency}");

                var url = $"{_flightOffersUrl}?{queryString}";
                
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var flightOfferResponse = JsonConvert.DeserializeObject<FlightOfferResponse>(responseContent);
                return flightOfferResponse;
            }
        }
    }
}