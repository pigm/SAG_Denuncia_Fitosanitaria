namespace DenunciaFitosanitaria.Data.Common.Models
{
    /// <summary>
    /// Sub categoria.
    /// </summary>
    public class SubCategoria
    {
        public int IdCategoria { get; set; }
        public int IdSubCategoria { get; set; }
        public string Nombre { get; set; }
        public string ImagenUrl { get; set; }
        public string ImagenEncrypt { get; set; }
        public bool Anonimo { get; set; }
        public string Descripcion { get; set; }
    }
}
