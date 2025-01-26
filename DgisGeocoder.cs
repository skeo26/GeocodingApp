using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocodingApp
{
    public class DgisGeocoder : IGeocodingProvider
    {
        private const string ApiUrl = "https://catalog.api.2gis.com/3.0/items/geocode";
        private const string ApiKey = "57f832b7-e2c8-4b24-9caf-b1e5694066d9";
        
        public async Task<Coordinates> GetCoordinatesAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адрес не может быть пустым.");

            using (var httpClient = new HttpClient())
            {
                var url = $"{ApiUrl}?q={address}&fields=items.point&key={ApiKey}";

                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response JSON: {jsonResponse}");
                if (string.IsNullOrWhiteSpace(jsonResponse) || jsonResponse.Contains("\"items\":[]"))
                {
                    Console.WriteLine("Ответ от API не содержит данных. Убедитесь, что адрес правильный.");
                    return null;
                }

                var dgisResponse = JsonConvert.DeserializeObject<DgisResponse>(jsonResponse);

                var result = dgisResponse?.Result?.Items?[0]?.Point;
                if (result == null)
                {
                    Console.WriteLine("Координаты не найдены в ответе. Проверьте параметры запроса API.");
                    return null;
                }

                return new Coordinates
                {
                    Latitude = result.Latitude,
                    Longitude = result.Longitude
                };
            }
        }

        private class DgisResponse
        {
            public DgisResult Result { get; set; }
        }

        private class DgisResult
        {
            public DgisItem[] Items { get; set; }
        }

        private class DgisItem
        {
            public DgisPoint Point { get; set; }
        }

        private class DgisPoint
        {
            [JsonProperty("lat")]
            public double Latitude { get; set; }

            [JsonProperty("lon")]
            public double Longitude { get; set; }
        }
    }
}
