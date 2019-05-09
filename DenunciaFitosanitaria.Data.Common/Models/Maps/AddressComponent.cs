using System.Collections.Generic;

namespace DenunciaFitosanitaria.Data.Common.Models.Maps
{
    /// <summary>
    /// Address component.
    /// </summary>
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }
}
