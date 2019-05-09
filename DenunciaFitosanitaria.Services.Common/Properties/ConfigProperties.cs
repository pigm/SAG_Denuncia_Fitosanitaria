using System;
namespace DenunciaFitosanitaria.Services.Common.Properties
{
    /// <summary>
    /// Config properties. Endpoints of Service.
    /// </summary>
    public static class ConfigProperties
    {
        public static string GOOGLE_API_ENDPOINT = "https://maps.googleapis.com/maps/api/geocode/json?";
#if DEBUG
        public static string API_ENDPOINT = "https://apidfs.sag.gob.cl/";//SAG Prod
        //public static string API_ENDPOINT = "http://192.168.51.28:8080/";//SAG QA
        //public static string API_ENDPOINT = "http://192.168.1.40/WebApiSag/"; // DSAC
#else
        public static string API_ENDPOINT = "https://apidfs.sag.gob.cl/";//SAG
        //public static string API_ENDPOINT = "http://192.168.1.40/WebApiSag/"; // DSAC
#endif
        //public static string API_ENDPOINT = "http://192.168.151.33:8080/"; //DESARROLLO 
        //public static string API_ENDPOINT = "http://192.168.51.28:8080/";
        public static string PATH_TOKEN = "api/User/Login";
        public static string PATH_TIPO_DENUNCIA = "PostTipoDenuncia";
        public static string PATH_CATEGORIA = "api/Categoria/AllCategories";
        public static string PATH_SUBCATEGORIA = "api/SubCategorias/GetSubcategorias";
        public static string PATH_SUBCATEGORIA_DETALLE = "api/SubCategorias/GetSubCategoria_Imagenes";
        public static string PATH_INSERT_DENUNCIA = "api/Denuncia/InsertDenuncia";
        public static string PATH_UPDATE_DENUNCIA = "api/Denuncia/UpdatetDenuncia";
        public static string PATH_IMAGE = "api/Image/GetImage";
        public static string USER_TOKEN = "123";
        public static string PASS_TOKEN = "123456";
        public static string PATH_INSERT_AUDIO = "api/Audio/UploadAudio";
        public static string PATH_INSERT_IMAGE = "api/Image/UploadImage";
    }
}
