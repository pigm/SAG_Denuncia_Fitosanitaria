using System;
using System.IO;
using Realms;

namespace DenunciaFitosanitaria.Models.Realm
{
    /// <summary>
    /// Denuncia.
    /// </summary>
    public class Denuncia: RealmObject
    {
        public int idestadodenuncia { get; set; }
        public string descripcion { get; set; }
        public string CorreoContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string MensajeVoz { get; set; }
        public string MensajeStream { get; set; }
        public string imagenurl { get; set; }
        public byte[] imageData { get; set; }
        public string latitud { get; set; }
        public string longitud { get; set; }
        public int idsubcategoria { get; set; }
        public string Comuna { get; set; }
        public string Region { get; set; }
        public string Georeferencia { get; set; }
        public string Comentario { get; set; }
        public string UserAprobacion { get; set; }
        public bool DenunciaRapida { get; set; }
        public int step { get; set; }
        public bool permiteAnonimo { get; set; }
        public string denunciaTmpId { get; set; }
    }
}
