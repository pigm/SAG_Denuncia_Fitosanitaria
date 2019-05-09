using System;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Speech;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using DenunciaFitosanitaria.Data.Common.Models;
using DenunciaFitosanitaria.Utils;
using Java.Util;
using Newtonsoft.Json;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Denuncia dialog fragment.
    /// </summary>
    public class DenunciaDialogFragment : Android.Support.V4.App.DialogFragment, View.IOnClickListener
    {
        Button buttonClose,buttonCancel,buttonSend;
        AppCompatEditText descripcionDenuncia,direccionDenuncia;
        ImageView mapaImg,audioImg,photoImg;
        LinearLayout mapaLyt, audioLyt, photoLyt,layoutDenuncia;
        TextView txtMap,txtAudio,txtImg;
        ProgressDialog _progressDialog;
        byte[] bitmapData;
        int stepProcess;
        AlphaAnimation buttonAnimation = new AlphaAnimation(1F, 0.4F);

        /// <summary>
        /// Ons the click.
        /// </summary>
        /// <param name="v">V.</param>
        public void OnClick(View v)
        {
            v.StartAnimation(buttonAnimation);

            if (v.ToString().ToLower().Contains("map"))
            {
                FragmentMapa fragmentMapa = new FragmentMapa();
                var transcation = FragmentManager.BeginTransaction();
                fragmentMapa.SetStyle(0, Resource.Style.AlertDialog_AppCompat);
                fragmentMapa.Show(transcation, fragmentMapa.Tag);
                fragmentMapa.Cancelable = false;
            }

            if (v.ToString().Contains("audio"))
            {
                AudioFragment fragmentAudio = new AudioFragment();
                var transcation = FragmentManager.BeginTransaction();

                fragmentAudio.Show(transcation, fragmentAudio.Tag);
                fragmentAudio.Cancelable = false;
            }

            if (v.ToString().Contains("photo"))
            {
                GaleriaFragment galeriaFragment = new GaleriaFragment();
                var transcation = FragmentManager.BeginTransaction();
                galeriaFragment.Show(transcation, galeriaFragment.Tag);
                galeriaFragment.Cancelable = false;
            }
        }


        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _progressDialog = new Android.App.ProgressDialog(Activity);
            _progressDialog.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            _progressDialog.SetMessage(Resources.GetString(Resource.String.insertandoDenuncia));
        }


        /// <summary>
        /// Ons the create view.
        /// </summary>
        /// <returns>The create view.</returns>
        /// <param name="inflater">Inflater.</param>
        /// <param name="container">Container.</param>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.DenunciaFragment, container, false);
            buttonClose = (Button)v.FindViewById(Resource.Id.buttonClose);
            buttonCancel = (Button)v.FindViewById(Resource.Id.buttonCancel);
            buttonSend = (Button)v.FindViewById(Resource.Id.buttonSend);
            descripcionDenuncia = (AppCompatEditText)v.FindViewById(Resource.Id.descripcionDenuncia);
            direccionDenuncia = (AppCompatEditText)v.FindViewById(Resource.Id.direccionDenuncia);
            mapaImg = (ImageView)v.FindViewById(Resource.Id.mapBtn);
            audioImg = (ImageView)v.FindViewById(Resource.Id.audioBtn);
            photoImg = (ImageView)v.FindViewById(Resource.Id.photoBtn);
            mapaLyt = (LinearLayout)v.FindViewById(Resource.Id.mapLyt);
            audioLyt = (LinearLayout)v.FindViewById(Resource.Id.audioLyt);
            photoLyt = (LinearLayout)v.FindViewById(Resource.Id.photoLyt);
            txtMap= (TextView)v.FindViewById(Resource.Id.txtMap);
            txtAudio = (TextView)v.FindViewById(Resource.Id.txtAudio);
            txtImg = (TextView)v.FindViewById(Resource.Id.txtImg);
            layoutDenuncia = (LinearLayout)v.FindViewById(Resource.Id.layoutDenuncia);

            mapaImg.SetOnClickListener(this);
            txtMap.SetOnClickListener(this);
            audioImg.SetOnClickListener(this);
            photoImg.SetOnClickListener(this);
            audioLyt.SetOnClickListener(this);
            photoLyt.SetOnClickListener(this);
            mapaLyt.SetOnClickListener(this);

            buttonClose.Animation = buttonAnimation;
            buttonClose.Click += delegate {
                DataManager.audioPath = null;
                DataManager.mapCenter = null;
                DataManager.imageData = null;
                DataManager.imageFrom = 0;
                DataManager.imagePath = null;
                Dismiss();
               };

            buttonCancel.Animation = buttonAnimation;
            buttonCancel.Click += delegate {
                DataManager.audioPath = null;
                DataManager.mapCenter = null;
                DataManager.imageData = null;
                DataManager.imageFrom = 0;
                DataManager.imagePath = null;
                Dismiss();
            };

            buttonSend.Animation = buttonAnimation;
            buttonSend.Click += async delegate {
                if(descripcionDenuncia.Text.Count() < 1 || direccionDenuncia.Text.Count() < 7){ // FIX HECTOR
                    descripcionDenuncia.Error = "Debe ingresar descripción";
                    direccionDenuncia.Error = "Verifique si el texto ingresado es de al menos 7 caracteres";
                    var msj = Resources.GetString(Resource.String.validaDenunciaDesc);
                    string title = Resources.GetString(Resource.String.validaTitDesc);
                    Dialogs.Dialog(Activity, title, msj, "Aceptar");
                }else if(DataManager.mapCenter==null){
                    var msj = Resources.GetString(Resource.String.validaDenunciaGps);
                    string title = Resources.GetString(Resource.String.validaTitGps);
                    Dialogs.Dialog(Activity, title, msj, "Aceptar");
                }else
                {
                    if (DataManager.imageData == null)
                    {
                        Snackbar.Make(layoutDenuncia, "Aviso sin imagen, ¿Desea continuar?", Snackbar.LengthLong)
                            .SetAction("CONTINUAR", async (view) =>
                            {
                                var denuncia = await ProcesarAviso();

                            })
                                .Show();
                    }else{
                        var denuncia = await ProcesarAviso();
                    }
                }

                GC.Collect();
            };

            return v;
        }

        /// <summary>
        /// Procesars the aviso.
        /// </summary>
        /// <returns>The aviso.</returns>
        public async System.Threading.Tasks.Task<Denuncia> ProcesarAviso()
        {
            //_progressDialog.Show();
            //_progressDialog.SetCancelable(false);
            DateTime d = DateTime.Now;
            var lat = DataManager.mapCenter.Latitude.ToString().Replace(",", ".");
            var lon = DataManager.mapCenter.Longitude.ToString().Replace(",", ".");
            var mail = string.Empty;
            //var mail = DataManager.cuentasMail.Any() ? DataManager.cuentasMail.FirstOrDefault().Name : string.Empty;
            /*****/

            var imageName = string.Empty;

            if (DataManager.imageData != null)
            {
                Bitmap newImg = MediaStore.Images.Media.GetBitmap(Activity.ContentResolver, DataManager.imageData);

                var streamImg = new MemoryStream();
                newImg.Compress(Bitmap.CompressFormat.Jpeg, 15, streamImg);//calidad al 15%
                bitmapData = streamImg.ToArray();

            }



            Denuncia denuncia = new Denuncia();
            denuncia.descripcion = descripcionDenuncia.Text + " .\n Referencia: " + direccionDenuncia.Text;
            denuncia.idestadodenuncia = 1;
            denuncia.DenunciaRapida = DataManager.denunciaRapida;
            denuncia.idsubcategoria = DataManager.subcategoriaSelected.IdSubCategoria;
            denuncia.latitud = lat;
            denuncia.longitud = lon;
            denuncia.FechaEnvio = d;
            denuncia.FechaAprobacion = new DateTime(2010, 1, 1);
            denuncia.Comentario = string.Empty;
            denuncia.CorreoContacto = mail;
            denuncia.imagenurl = imageName;
            denuncia.MensajeVoz = DataManager.audioName;
            denuncia.TelefonoContacto = string.Empty;
            denuncia.UserAprobacion = string.Empty;
            /*****/

           /* if (ValidationUtils.GetNetworkStatus())
            {
                var georeferencia = await ServiceDelegate.Instance.GetMapGeo(lat, lon, Resources.GetString(Resource.String.google_maps_key));
                if (georeferencia.Success)
                {
                    if (DataManager.audioPath != null)
                    {
                        using (var fs = File.Open(DataManager.audioPath, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            var processAudio = await ServiceDelegate.Instance.InsertAudio(fs, DataManager.audioPath, DataManager.audioName, DataManager.token);
                        }
                    }


                    if (DataManager.imageData != null)
                    {
                        var processImg = await ServiceDelegate.Instance.InsertImageByte(bitmapData, DataManager.fotoPath, DataManager.imageData.LastPathSegment, DataManager.token);
                        if (processImg.Success)
                        {
                            imageName = (string)processImg.Response;
                        }
                    }
                    denuncia.imagenurl = imageName;
                    var gereferenciaGeo = (MapGeocode)georeferencia.Response;
                    var address_components = gereferenciaGeo.results.Any() ? gereferenciaGeo.results[1].address_components : new List<AddressComponent>();
                    var regionVal = address_components.Find(g => g.types[0] == "administrative_area_level_1");
                    var comunaVal = address_components.Find(g => g.types[0] == "administrative_area_level_3");

                    denuncia.Georeferencia = JsonConvert.SerializeObject(gereferenciaGeo);
                    denuncia.Region = regionVal != null ? regionVal.long_name : "Sin región";
                    denuncia.Comuna = comunaVal != null ? comunaVal.long_name : "Sin comuna";

                    /*var denunciaService = await ServiceDelegate.Instance.InsertDenuncia(denuncia, DataManager.token);
                    if (denunciaService.Success)
                    {
                        DataManager.denunciaId = (string)denunciaService.Response;
                        DenunciaEnviadaFragment denunciaEnviadaFragment = new DenunciaEnviadaFragment();
                        _progressDialog.Hide();
                        initVars();
                        var transcation = FragmentManager.BeginTransaction();
                        denunciaEnviadaFragment.SetStyle(0, Resource.Style.AlertDialog_AppCompat);
                        denunciaEnviadaFragment.Show(transcation, denunciaEnviadaFragment.Tag);
                        denunciaEnviadaFragment.Cancelable = false;
                    }
                    else
                    {
                        _progressDialog.Hide();
                        procesaError();
                        stepProcess = 3;
                        GuardarBorradorMsje(layoutDenuncia, denuncia, Resources.GetString(Resource.String.guardarBorrardorMsj), Resources.GetString(Resource.String.guardarBorrardorOK));
                    }
                }
                else
                {
                    _progressDialog.Hide();
                    procesaError();
                    stepProcess = 2;
                    GuardarBorradorMsje(layoutDenuncia, denuncia, Resources.GetString(Resource.String.guardarBorrardorMsj), Resources.GetString(Resource.String.guardarBorrardorOK));
                }
            }
            else
            {
                _progressDialog.Hide();
                procesaError();*/
                    stepProcess = 1;
                    GuardarBorradorMsje(layoutDenuncia, denuncia, Resources.GetString(Resource.String.noInternet), Resources.GetString(Resource.String.guardarBorrardor));
          //      }
            
            return denuncia;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Activities.ActivitySplash"/> class.
        /// </summary>
        void GuardarBorradorMsje(View context, Denuncia denuncia, string message = "", string button = "")
        {
            Snackbar snackbar = Snackbar.Make(context, message, Snackbar.LengthIndefinite);
                    snackbar.SetAction(button, (view) => {
                            snackbar.Dismiss();
                            var idTemp = Guid.NewGuid().ToString();
                            DataManager.denunciaTmpId = idTemp;

                            var existenDenuncias = DataManager.RealmInstance.All<Models.Realm.DenunciaTemp>();
                            if (existenDenuncias.Any())
                            {
                                using (var transDenunciaTemp = DataManager.RealmInstance.BeginWrite())
                                {
                                    DataManager.RealmInstance.RemoveAll("DenunciaTemp");
                                    transDenunciaTemp.Commit();
                                }
                            }
                            DataManager.RealmInstance.Write(() =>
                            {
                                var jsonObj = JsonConvert.SerializeObject(denuncia);
                                Models.Realm.DenunciaTemp denunciaRealm = JsonConvert.DeserializeObject<Models.Realm.DenunciaTemp>(jsonObj);

                                denunciaRealm.MensajeStream = DataManager.audioPath;
                                denunciaRealm.imageData = bitmapData;
                                denunciaRealm.denunciaTmpId = idTemp;
                                denunciaRealm.step = stepProcess;
                                DataManager.RealmInstance.Add(denunciaRealm);
                            });

                            DataManager.audioPath = null;
                            DataManager.mapCenter = null;
                            DataManager.imageData = null;
                            DataManager.imageFrom = 0;
                            DataManager.imagePath = null;
                            DataManager.denunciaId = "0";
                            DenunciaEnviadaFragment denunciaEnviadaFragment = new DenunciaEnviadaFragment();
                            _progressDialog.Hide();
                            initVars();
                            var transcation = FragmentManager.BeginTransaction();
                            denunciaEnviadaFragment.SetStyle(0, Resource.Style.AlertDialog_AppCompat);
                            denunciaEnviadaFragment.Show(transcation, denunciaEnviadaFragment.Tag);
                            denunciaEnviadaFragment.Cancelable = false;

                        })
                        .Show();
        }


        /// <summary>
        /// Inits the variables.
        /// </summary>
        private static void initVars()
        {
            DataManager.audioPath = null;
            DataManager.audioName = null;
            DataManager.fotoPath = null;
            DataManager.imageData = null;
            DataManager.imageFrom = 0;
            DataManager.imagePath = null;
            DataManager.mapCenter = null;
            DataManager.backParent = true;
        }


        /// <summary>
        /// Procesas the error.
        /// </summary>
        private void procesaError()
        {
            _progressDialog.Hide();
            var msj = Resources.GetString(Resource.String.errorMsj); ;
            string title = Resources.GetString(Resource.String.ups);
            Dialogs.Dialog(Activity, title, msj, "Aceptar");
        }




        /// <summary>
        /// Ons the start.
        /// </summary>
        public override void OnStart()
        {
            if(Dialog == null){
                return;
            }

            Dialog.Window.SetWindowAnimations(Resource.Style.DlgAnimation);
            base.OnStart();
        }


        /// <summary>
        /// Ons the resume.
        /// </summary>
        public override void OnResume()
        {
            if(DataManager.audioPath != null){
                audioImg.SetColorFilter(Resources.GetColor(Resource.Color.tab_selected));
            }else{
                audioImg.ClearColorFilter();
            }

            if(DataManager.mapCenter != null){
                mapaImg.SetColorFilter(Resources.GetColor(Resource.Color.tab_selected));
            }else{
                mapaImg.ClearColorFilter();
            }

            if (DataManager.imagePath != null)
            {
                photoImg.SetColorFilter(Resources.GetColor(Resource.Color.tab_selected));
            }else{
                photoImg.ClearColorFilter();
            }

            base.OnResume();
        }

        /// <summary>
        /// Starts the voice input.
        /// </summary>
        private void startVoiceInput()
        {
            Intent intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraLanguage, Locale.Default);
            intent.PutExtra(RecognizerIntent.ExtraPrompt, "Hola, ¿En que podemos ayudarte?");
            try
            {
                StartActivityForResult(intent, 10);
            }
            catch (ActivityNotFoundException a)
            {

            }
        }

        /// <summary>
        /// Ons the activity result.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            foreach(Android.Support.V4.App.DialogFragment fragment in FragmentManager.Fragments){
                fragment.OnActivityResult(requestCode, resultCode, data);
            }
        }



    }
}
