using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeocodingApp
{
    public interface IGeocodingProvider
    {
        Task<Coordinates> GetCoordinatesAsync(string address);
    }
}
