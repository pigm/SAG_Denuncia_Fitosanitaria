using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using DenunciaFitosanitaria.Data.Common.Models.Maps;
using DenunciaFitosanitaria.Models.Realm;
using DenunciaFitosanitaria.Services.Common.Delegate;
using DenunciaFitosanitaria.Utils;
using Newtonsoft.Json;

namespace DenunciaFitosanitaria.Services
{
    /// <summary>
    /// Send periodic notice.
    /// </summary>
    [Service]
    public class SendPeriodicNotice : IntentService
    {
        Timer timer { get; set; }
        /// <summary>
        /// Ons the bind.
        /// </summary>
        /// <returns>The bind.</returns>
        /// <param name="intent">Intent.</param>
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        /// <summary>
        /// Ons the start command.
        /// </summary>
        /// <returns>The start command.</returns>
        /// <param name="intent">Intent.</param>
        /// <param name="flags">Flags.</param>
        /// <param name="startId">Start identifier.</param>
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return OnStartCommandAsync(intent, flags, startId).Result;
        }

        /// <summary>
        /// Ons the start command async.
        /// </summary>
        /// <returns>The start command async.</returns>
        /// <param name="intent">Intent.</param>
        /// <param name="flags">Flags.</param>
        /// <param name="startId">Start identifier.</param>
        public async Task<StartCommandResult> OnStartCommandAsync(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            StartCommandResult a = StartCommandResult.NotSticky;
            //await ProcesarDenuncias();
            return a;//base.OnStartCommand(intent, flags, startId);
        }


        /// <summary>
        /// Ons the create.
        /// </summary>
        public override async void OnCreate()
        {
            //await ProcesarDenuncias();
            await ServiceHelper.ProcesarDenuncias();
            /*timer = new Timer(20000);
            timer.Elapsed += async (sender, e) => await ProcesarDenuncias();
            timer.Start();*/
            base.OnCreate();
        }


        /// <summary>
        /// Procesars the denuncias.
        /// </summary>
        /// <returns>The denuncias.</returns>
        async Task ProcesarDenuncias()
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

                        if (denuncia.step != 3)
                        {
                            var georeferencia = await ServiceDelegate.Instance.GetMapGeo(denuncia.latitud, denuncia.longitud, Resources.GetString(Resource.String.google_maps_key));
                            if (georeferencia.Success)
                            {
                                if (denuncia.MensajeStream != null || denuncia.MensajeStream != string.Empty)
                                {
                                    using (var fs = File.Open(denuncia.MensajeStream, FileMode.Open, FileAccess.Read, FileShare.None))
                                    {
                                        var processAudio = await ServiceDelegate.Instance.InsertAudio(fs, denuncia.MensajeStream, denuncia.MensajeVoz, DataManager.token);
                                    }
                                }


                                if (denuncia.imageData != null)
                                {
                                    var processImg = await ServiceDelegate.Instance.InsertImageByte(denuncia.imageData, "", "fake", DataManager.token);
                                    if (processImg.Success)
                                    {
                                        denunciaNew.imagenurl = (string)processImg.Response;
                                    }
                                }

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
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ons the handle intent.
        /// </summary>
        /// <param name="intent">Intent.</param>
        protected override void OnHandleIntent(Intent intent)
        {   
        }
    }
}
