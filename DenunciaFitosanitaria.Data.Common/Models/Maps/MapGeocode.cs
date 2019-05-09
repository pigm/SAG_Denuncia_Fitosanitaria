using System.Collections.Generic;

namespace DenunciaFitosanitaria.Data.Common.Models.Maps
{
    /// <summary>
    /// Map geocode.
    /// </summary>
    public class MapGeocode
    {
        public List<ResultGeocode> results { get; set; }
        public string status { get; set; }
    }
}
