using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocodingApp
{
    public class GeocodingService
    {
        private readonly IGeocodingProvider _geocodingProvider;

        public GeocodingService(IGeocodingProvider geocodingProvider)
        {
            _geocodingProvider = geocodingProvider ?? throw new ArgumentNullException(nameof(geocodingProvider));
        }

        public Task<Coordinates> GetCoordinatesAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Адрес не может быть пустым.");

            return _geocodingProvider.GetCoordinatesAsync(address);
        }
    }
}
