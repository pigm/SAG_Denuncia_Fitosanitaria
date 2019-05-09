using System;
using System.Threading.Tasks;
using DenunciaFitosanitaria.Utils;
using DenunciaFitosanitaria.Models.Realm;
using System.Linq;
using DenunciaFitosanitaria.Services.Common.Delegate;
using System.IO;
using DenunciaFitosanitaria.Data.Common.Models.Maps;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DenunciaFitosanitaria.Services
{
    /// <summary>
    /// Service helper.
    /// </summary>
    public static class ServiceHelper
    {
        /// <summary>
        /// Procesars the denuncias.
        /// </summary>
        /// <returns>The denuncias.</returns>
        public static async Task ProcesarDenuncias()
        {
            var denunciaProcesar = DataManager.RealmInstance.All<Denuncia>().ToList();
            if (ValidationUtils.GetNetworkStatus())
            {
                if (denunciaProcesar.Any())
                {
                    foreach (Denuncia denuncia in denunciaProcesar)
                    {
                        Data.Common.Models.Denuncia denunciaNew = new Data.Common.Models.Denuncia();
                        denunciaNew.descripcion = denuncia.descripcion;
                        denunciaNew.idestadodenuncia = 1;
                        denunciaNew.DenunciaRapida = denuncia.DenunciaRapida;
                        denunciaNew.idsubcategoria = denuncia.idsubcategoria;
                        denunciaNew.latitud = denuncia.latitud;
                        denunciaNew.longitud = denuncia.longitud;
                        denunciaNew.FechaEnvio = DateTime.Now;
                        denunciaNew.FechaAprobacion = new DateTime(2010, 1, 1);
                        denunciaNew.Comentario = string.Empty;
                        denunciaNew.CorreoContacto = denuncia.CorreoContacto;
                        denunciaNew.imagenurl = denuncia.imagenurl;
                        denunciaNew.MensajeVoz = denuncia.MensajeVoz;
                        denunciaNew.TelefonoContacto = denuncia.TelefonoContacto;
                        denunciaNew.UserAprobacion = string.Empty;

                        var key = DataManager.mapKey;
                        if (denuncia.step != 3)
                        {
                            if (!String.IsNullOrEmpty(denuncia.MensajeStream))
                            {
                                using (var fs = File.Open(denuncia.MensajeStream, FileMode.Open, FileAccess.Read, FileShare.None))
                                {
                                    var processAudio = await ServiceDelegate.Instance.InsertAudio(fs, denuncia.MensajeStream, denuncia.MensajeVoz, DataManager.token);
                                }
                            }


                            if (denuncia.imageData != null)
                            {
                                var processImg = await ServiceDelegate.Instance.InsertImageByte(denuncia.imageData, "", "fake", DataManager.token);
                                //if (processImg.Success)
                                //{
                                    denunciaNew.imagenurl = (string)processImg.Response;
                                //}
                            } 

                            var georeferencia = await ServiceDelegate.Instance.GetMapGeo(denuncia.latitud, denuncia.longitud, key);
                            if (georeferencia.Success)
                            {

                                var gereferenciaGeo = (MapGeocode)georeferencia.Response;
                                var address_components = gereferenciaGeo.results.Any() ? gereferenciaGeo.results[1].address_components : new List<AddressComponent>();
                                var regionVal = address_components.Find(g => g.types[0] == "administrative_area_level_1");
                                var comunaVal = address_components.Find(g => g.types[0] == "administrative_area_level_3");

                                denunciaNew.Georeferencia = JsonConvert.SerializeObject(gereferenciaGeo);
                                denunciaNew.Region = regionVal != null ? regionVal.long_name : "Sin región";
                                denunciaNew.Comuna = comunaVal != null ? comunaVal.long_name : "Sin comuna";
                            }
                        }
                        else
                        {
                            denunciaNew.Georeferencia = denuncia.Georeferencia;
                            denunciaNew.Region = denuncia.Region;
                            denunciaNew.Comuna = denuncia.Comuna;
                        }

                        var denunciaService = await ServiceDelegate.Instance.InsertDenuncia(denunciaNew, DataManager.token);
                        if (denunciaService.Success)
                        {
                            using (var trans = DataManager.RealmInstance.BeginWrite())
                            {
                                DataManager.RealmInstance.Remove(denuncia);
                                trans.Commit();
                            }
                            DataManager.mainActivity.UpdateBadge();
                        }
                    }
                }
            }
        }
    }
}