using System;
namespace DenunciaFitosanitaria.Data.Common.Models
{
    /// <summary>
    /// Tipo denuncia.
    /// </summary>
    public class TipoDenuncia
    {
        public string Nombre { get; set; }
        public int IdTipoDenuncia { get; set; }
        public bool Estado { get; set; }
        public bool Incognito { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Data.Common.Models.TipoDenuncia"/> class.
        /// </summary>
        /// <param name="Nombre">Nombre.</param>
        /// <param name="IdTipoDenuncia">Identifier tipo denuncia.</param>
        /// <param name="Estado">If set to <c>true</c> estado.</param>
        /// <param name="Incognito">If set to <c>true</c> incognito.</param>
        public TipoDenuncia(string Nombre,int IdTipoDenuncia,bool Estado,bool Incognito)
        {
            this.Nombre = Nombre;
            this.IdTipoDenuncia = IdTipoDenuncia;
            this.Incognito = Incognito;
            this.Estado = Estado;
        }
    }
}
