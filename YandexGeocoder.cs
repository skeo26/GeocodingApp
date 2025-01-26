using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocodingApp
{
    public class YandexGeocoder : IGeocodingProvider
    {
        private const string ApiUrl = "https://geocode-maps.yandex.ru/1.x/";
        private const string ApiKey = "your-api-key";

        public async Task<Coordinates> GetCoordinatesAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адрес не может быть пустым.");

            using (var httpClient = new HttpClient())
            {
                var url = $"{ApiUrl}?apikey={ApiKey}&geocode={Uri.EscapeDataString(address)}&format=json";
                try
                {
                    var response = await httpClient.GetAsync(url);
                    Console.WriteLine($"HTTP Статус код: {response.StatusCode}");
                    response.EnsureSuccessStatusCode();

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var yandexResponse = JsonConvert.DeserializeObject<YandexResponse>(jsonResponse);

                    var position = yandexResponse?.Response?.GeoObjectCollection?.FeatureMember?[0]?.GeoObject?.Point?.Pos;
                    if (position == null) return null;

                    var coords = position.Split(' ');
                    return new Coordinates
                    {
                        Longitude = double.Parse(coords[0], CultureInfo.InvariantCulture),
                        Latitude = double.Parse(coords[1], CultureInfo.InvariantCulture)
                    };
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"Ошибка HTTP запроса: {httpEx.Message}");
                    throw;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Неизвестная ошибка: {ex.Message}");
                    throw;
                }
            }
        }


        private class YandexResponse
        {
            public Response Response { get; set; }
        }

        private class Response
        {
            public GeoObjectCollection GeoObjectCollection { get; set; }
        }

        private class GeoObjectCollection
        {
            public FeatureMember[] FeatureMember { get; set; }
        }

        private class FeatureMember
        {
            public GeoObject GeoObject { get; set; }
        }

        private class GeoObject
        {
            public Point Point { get; set; }
        }

        private class Point
        {
            public string Pos { get; set; }
        }
    }
}
