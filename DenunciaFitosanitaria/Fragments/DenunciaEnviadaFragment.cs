using System.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using DenunciaFitosanitaria.Models.Realm;
using DenunciaFitosanitaria.Services;
using DenunciaFitosanitaria.Utils;
using Newtonsoft.Json;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Denuncia enviada fragment.
    /// </summary>
    public class DenunciaEnviadaFragment : DialogFragment
    {
        Button buttonClose, buttonCancel, buttonSend;
        TextView titulo,subtitulo;
        AppCompatEditText correoDenuncia, telefonoDenuncia;
        Android.App.ProgressDialog _progressDialog;
        AlphaAnimation buttonAnimation = new AlphaAnimation(1F, 0.4F);

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _progressDialog = new Android.App.ProgressDialog(Activity);
            _progressDialog.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            _progressDialog.SetMessage(Resources.GetString(Resource.String.insertandoDenunciaContacto));
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
            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.DenunciaEnviadaFragment, container, false);
            buttonClose = (Button)v.FindViewById(Resource.Id.buttonClose);
            buttonCancel = (Button)v.FindViewById(Resource.Id.buttonCancelAnonymous);
            buttonSend = (Button)v.FindViewById(Resource.Id.buttonSendExtraData);
            correoDenuncia = (AppCompatEditText)v.FindViewById(Resource.Id.email);
            telefonoDenuncia = (AppCompatEditText)v.FindViewById(Resource.Id.telefono);
            titulo = (TextView)v.FindViewById(Resource.Id.tituloDenuncia);
            subtitulo = (TextView)v.FindViewById(Resource.Id.subtituloDenuncia);
            var mail = DataManager.cuentasMail.Any() ? DataManager.cuentasMail.FirstOrDefault().Name : string.Empty;
            correoDenuncia.Text = mail;

            var permiteAnonimo = DataManager.tiposDenuncia.Find(x => x.IdTipoDenuncia == DataManager.categoriaSelected.IdTipoDenuncia).Incognito;
            if(!permiteAnonimo){
                buttonCancel.Visibility = ViewStates.Gone;
            }

            if(DataManager.denunciaId.Equals("0")){
                _progressDialog.SetMessage(Resources.GetString(Resource.String.guardandoAviso));
                titulo.Text = "Haz completado tu denuncia";
                subtitulo.Text = subtitulo.Text;
                buttonSend.Text = "ENVIAR";
            }

            buttonSend.Animation = buttonAnimation;
            buttonSend.Click += async delegate {
                var validaEmail = ValidationUtils.EmailIsValid(correoDenuncia.Text);
                var validaTelefono = (telefonoDenuncia.Text.Length > 7 && telefonoDenuncia.Text.Length < 10)  ? true : false;
                if(validaEmail){
                    if(validaTelefono){
                        _progressDialog.Show();
                        _progressDialog.SetCancelable(false);
                        if (DataManager.denunciaId.Equals("0"))
                        {
                            DataManager.RealmInstance.Write(() =>
                            {
                                var datoUpdate = DataManager.RealmInstance.All<DenunciaTemp>().Where(p => p.denunciaTmpId == DataManager.denunciaTmpId).FirstOrDefault();
                                datoUpdate.TelefonoContacto = telefonoDenuncia.Text;
                                datoUpdate.CorreoContacto = correoDenuncia.Text;
                                var jsonObj = JsonConvert.SerializeObject(datoUpdate);
                                Models.Realm.Denuncia denunciaRealm = JsonConvert.DeserializeObject<Models.Realm.Denuncia>(jsonObj);

                                DataManager.RealmInstance.Add<Denuncia>(denunciaRealm);
                            });
                            FinalizarDenuncia();                         
                        }
                    }else{
                        telefonoDenuncia.Error = "Debe ingresar teléfono de contacto";
                    }
                }else
                {
                    correoDenuncia.Error = "Debe ingresar email de contacto";
                }
                using (var transDenunciaTemp = DataManager.RealmInstance.BeginWrite())
                {
                    DataManager.RealmInstance.RemoveAll("DenunciaTemp");
                    transDenunciaTemp.Commit();
                }
            };

            buttonCancel.Animation = buttonAnimation;
            buttonCancel.Click += delegate {
                _progressDialog.Show();
                _progressDialog.SetCancelable(false);
                DataManager.RealmInstance.Write(() =>
                {
                    var datoUpdate = DataManager.RealmInstance.All<DenunciaTemp>().Where(p => p.denunciaTmpId == DataManager.denunciaTmpId).FirstOrDefault();
                    var jsonObj = JsonConvert.SerializeObject(datoUpdate);
                    Models.Realm.Denuncia denunciaRealm = JsonConvert.DeserializeObject<Models.Realm.Denuncia>(jsonObj);
                    DataManager.RealmInstance.Add<Denuncia>(denunciaRealm);
                });
                FinalizarDenuncia();
                using (var transDenunciaTemp = DataManager.RealmInstance.BeginWrite())
                {
                    DataManager.RealmInstance.RemoveAll("DenunciaTemp");
                    transDenunciaTemp.Commit();
                }
            };

            return v;
        }

        /// <summary>
        /// Finalizars the denuncia.
        /// </summary>
        async void FinalizarDenuncia()
        {
            DataManager.backParent = true;
            var parentExist = FragmentManager.Fragments.OfType<DenunciaDialogFragment>();
            if (parentExist.Any())
            {
                parentExist.ElementAt(0).Dismiss();
            }
            await ServiceHelper.ProcesarDenuncias();
            Activity.Finish();
        }

        /// <summary>
        /// Msjs the validacion.
        /// </summary>
        void MsjValidacion()
        {
            var msj = Resources.GetString(Resource.String.validaDenunciaContacto);
            string title = Resources.GetString(Resource.String.validaTitDesc);
            Dialogs.Dialog(Activity, title, msj, "Aceptar");
        }

        /// <summary>
        /// Procesas the error.
        /// </summary>
        void ProcesaError()
        {
            _progressDialog.Hide();
            var msj = Resources.GetString(Resource.String.errorMsj); ;
            string title = Resources.GetString(Resource.String.ups);
            Dialogs.Dialog(Activity, title, msj, "Aceptar");
        }
    }
}
