using System;
using System.Net.Http;
using System.Threading.Tasks;
using GeocodingApp;
using Newtonsoft.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Выберите провайдер (1 - Яндекс, 2 - 2ГИС):");
        var providerChoice = Console.ReadLine();

        IGeocodingProvider provider = providerChoice switch
        {
            "1" => new YandexGeocoder(),
            "2" => new DgisGeocoder(),
            _ => throw new InvalidOperationException("Неверный выбор провайдера.")
        };

        var geocodingService = new GeocodingService(provider);

        Console.WriteLine("Введите адрес для определения координат:");
        string address = Console.ReadLine();

        try
        {
            var coordinates = await geocodingService.GetCoordinatesAsync(address);

            Console.WriteLine(coordinates != null
                ? $"Координаты: Широта {coordinates.Latitude}, Долгота {coordinates.Longitude}"
                : "Координаты не найдены для указанного адреса.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}
