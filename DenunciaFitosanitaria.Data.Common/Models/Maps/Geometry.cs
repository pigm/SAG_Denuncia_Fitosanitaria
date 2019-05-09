namespace DenunciaFitosanitaria.Data.Common.Models.Maps
{
    /// <summary>
    /// Geometry.
    /// </summary>
    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
        public Bounds bounds { get; set; }
    }
}
