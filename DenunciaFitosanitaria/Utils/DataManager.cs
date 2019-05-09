using System.Collections.Generic;
using Android.Accounts;
using Android.Gms.Maps.Model;
using Android.Graphics;
using DenunciaFitosanitaria.Activities;
using DenunciaFitosanitaria.Data.Common.Models;
using Realms;

namespace DenunciaFitosanitaria.Utils
{
    /// <summary>
    /// Data manager.
    /// </summary>
    public class DataManager
    {
        public static Bitmap thumbnail{ get; set; } 
        public static bool backParent { get; set; } 
        public static bool denunciaRapida { get; set; } 
        public static string audioName{ get; set; }
        public static string denunciaId { get; set; }
        public static int imageFrom { get; set; }
        public static string imagePath { get; set; }
        public static Android.Net.Uri imageData { get; set; }
        public static string fotoPath { get; set; }
        public static string audioPath { get; set; }
        public static List<Account> cuentasMail{ get; set; }
        public static Account cuentaMail{ get; set; }
        public static SubCategoriaDetalle subcategoriaDetalle { get; set; }
        public static SubCategoria subcategoriaSelected { get; set; }
        public static List<SubCategoria> subcategorias { get; set; }
        public static Categoria categoriaSelected { get; set; }
        public static string token { get; set; }
        public static List<TipoDenuncia> tiposDenuncia { get; set; }
        public static List<Categoria> categorias { get; set; }
        public static LatLng mapCenter { get; set; }
        static Realm realm;
        public static MainActivity mainActivity{ get; set; }
        public static string denunciaTmpId { get; set; }
        public static string mapKey { get; set; }

        /// <summary>
        /// Gets the realm instance.
        /// </summary>
        /// <value>The realm instance.</value>
        public static Realm RealmInstance
        {
            get
            {
                if (realm == null)
                {
                    try
                    {
                        RealmConfiguration config = new RealmConfiguration
                        {
                            SchemaVersion = 3,
                            ShouldDeleteIfMigrationNeeded = true
                        };
                        realm = Realm.GetInstance(config);
                    }
                    catch
                    {
                        realm = Realm.GetInstance();
                    }
                }
                return realm;
            }
        }

    }
}
