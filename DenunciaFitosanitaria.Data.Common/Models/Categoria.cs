namespace DenunciaFitosanitaria.Data.Common.Models
{
    /// <summary>
    /// Categoria.
    /// </summary>
    public class Categoria
    {
        public int IdTipoDenuncia { get; set; }
        public int IdCategoria { get; set; }
        public string Nombre { get; set; }
        public string ImagenUrl { get; set; }
        public string ImagenEncrypt { get; set; }
    }
}
