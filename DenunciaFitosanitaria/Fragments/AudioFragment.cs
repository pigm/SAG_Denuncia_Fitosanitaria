using System;
using Android;
using Android.Content.PM;
using Android.OS;
using Android.Media;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using DenunciaFitosanitaria.Utils;
using Android.Content;
using System.Linq;
using Android.Views.Animations;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Audio fragment.
    /// </summary>
    public class AudioFragment : DialogFragment, MediaRecorder.IOnInfoListener
    {
        MediaRecorder mediarecorder;
        MediaPlayer mediaplayer;
        Button btnRecord, btnStopRecord, btnStart, btnStop, btnCancelar,btnAceptar,buttonClose, buttonEliminarAudio;
        RelativeLayout rightTopPanelEliminarBtn;
        string pathSave = "";
        const int REQUEST_PERMISSION_CODE = 1000;
        const string TITLE = "SAG";
        const string MESSAGE = "Estas seguro que deseas eliminar el audio";
        const string POSITIVE_BUTTON = "Aceptar";
        const string NEGATIVE_BUTTON = "Cancelar";
        bool isGrantedPermission = false;
        string fileName = "";
        bool statusRecording;
        AlphaAnimation buttonAnimation = new AlphaAnimation(1F, 0.4F);

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNoTitle, 0);
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
            if (ContextCompat.CheckSelfPermission(this.Activity,Manifest.Permission.WriteExternalStorage) != Permission.Granted
                && ContextCompat.CheckSelfPermission(this.Activity,Manifest.Permission.RecordAudio) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this.Activity, new string[]
                {
                    Manifest.Permission.WriteExternalStorage,
                    Manifest.Permission.RecordAudio },
                    REQUEST_PERMISSION_CODE);

            }
            else
            {
                isGrantedPermission = true;
            }

            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.AudioFragment, container, false);
            rightTopPanelEliminarBtn = (RelativeLayout)v.FindViewById(Resource.Id.rightTopPanelEliminarBtn);
            btnRecord = (Button)v.FindViewById(Resource.Id.btnRecord);
            btnStopRecord = (Button)v.FindViewById(Resource.Id.btnStopRecord);
            btnStart = (Button)v.FindViewById(Resource.Id.btnPlay);
            btnStop = (Button)v.FindViewById(Resource.Id.btnStop);
            btnCancelar = (Button)v.FindViewById(Resource.Id.buttonCancelAudio);
            btnAceptar = (Button)v.FindViewById(Resource.Id.buttonSendAudio);
            buttonEliminarAudio = (Button)v.FindViewById(Resource.Id.buttonEliminarAudio);
            buttonClose = (Button)v.FindViewById(Resource.Id.buttonCloseAudio);

            btnAceptar.Animation = buttonAnimation;
            btnAceptar.Click += delegate {
                if (!statusRecording)
                {
                    if (mediaplayer != null)
                    {
                        try{
                            if (mediaplayer.IsPlaying)
                            {
                                mediaplayer.Stop();
                                mediaplayer.Release();
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }

                    if (pathSave != string.Empty)
                    {
                        DataManager.audioPath = pathSave;
                        DataManager.audioName = fileName;
                    }

                    Dismiss();
                }
            };

            btnCancelar.Animation = buttonAnimation;
            btnCancelar.Click += delegate {
                /*DataManager.audioPath = null;
                DataManager.audioName = null;
                if (!statusRecording)
                {
                    //StopRecorder();
                    if (mediaplayer != null)
                    {
                        if (mediaplayer.IsPlaying)
                        {
                            mediaplayer.Stop();
                            mediaplayer.Release();
                        }
                    }*/
                    Dismiss();
                //}
            };
            buttonEliminarAudio.Animation = buttonAnimation;
            buttonEliminarAudio.Click += delegate {
                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this.Activity);
                builder.SetTitle(TITLE);
                builder.SetIcon(Resource.Drawable.logo_sag);
                builder.SetMessage(MESSAGE);
                builder.SetCancelable(false);
                builder.SetPositiveButton(POSITIVE_BUTTON, delegate {
                    DataManager.audioPath = null;
                    DataManager.audioName = null;
                    if (!statusRecording)
                    {
                        //StopRecorder();
                        if (mediaplayer != null)
                        {
                            try{
                                if (mediaplayer.IsPlaying)
                                {
                                    mediaplayer.Stop();
                                    mediaplayer.Release();
                                }
                            }catch(Exception e){}
                        }
                        Dismiss();
                    }
                });
                builder.SetNegativeButton(NEGATIVE_BUTTON, delegate{});
                builder.Show();
            };

            buttonClose.Animation = buttonAnimation;
            buttonClose.Click += delegate {               
                Dismiss();         
            };
            btnStopRecord.Clickable = false;
            btnStopRecord.Enabled = false;
            btnStart.Clickable = false;
            btnStart.Enabled = false;
            btnStop.Clickable = false;
            btnStop.Enabled = false;

            btnRecord.Animation = buttonAnimation;
            btnRecord.Click += delegate
            {
                RecordAudio();
            };

            btnStopRecord.Animation = buttonAnimation;
            btnStopRecord.Click += delegate
            {
                StopRecorder();
            };

            btnStart.Animation = buttonAnimation;
            btnStart.Click += delegate
            {
                StarLastRecord();
            };

            btnStop.Animation = buttonAnimation;
            btnStop.Click += delegate
            {
                StopLastRecord();
            };

            if(DataManager.audioPath!=null){
                SetupMediaRecorder();
                PreparePlayer();
                rightTopPanelEliminarBtn.Visibility = ViewStates.Visible;
            }


            return v;
        }

        /// <summary>
        /// Ons the request permissions result.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="permissions">Permissions.</param>
        /// <param name="grantResults">Grant results.</param>
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case REQUEST_PERMISSION_CODE:
                    {
                        if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                        {
                            Toast.MakeText(Activity, "Permiso Otorgados", ToastLength.Short).Show();
                            isGrantedPermission = true;
                        }
                        else
                        {
                            Toast.MakeText(Activity, "Necesitas asignar permisos para grabar", ToastLength.Short).Show();
                            isGrantedPermission = false;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Records the audio.
        /// </summary>
        void RecordAudio()
        {
            if (!statusRecording)
            {
                if (isGrantedPermission)
                {
                    DateTime d = DateTime.Now;
                    string dateString = d.ToString("yyyyMMddHHmmss");
                    pathSave = Android.OS.Environment.ExternalStorageDirectory
                                      .AbsolutePath + "/" + dateString + "_audio.aac";
                    fileName = dateString + "_audio.aac";

                    SetupMediaRecorder();
                    try
                    {
                        mediarecorder.Prepare();
                        mediarecorder.Start();
                        statusRecording = true;
                        btnStart.Enabled = false;
                        btnStart.Clickable = false;
                        btnStart.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
                        btnStart.SetTextColor(Resources.GetColor(Resource.Color.hint));
                        btnStopRecord.Enabled = true;
                        btnStopRecord.Clickable = true;
                        btnStopRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
                        btnRecord.SetTextColor(Resources.GetColor(Resource.Color.white));
                        btnRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.colorRed));
                        btnStopRecord.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("DEBUG", ex.Message);
                    }
                }
            }

        }

        /// <summary>
        /// Stops the last record.
        /// </summary>
        void StopLastRecord()
        {
            btnStop.Enabled = false;
            btnStop.Clickable = false;
            btnStop.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnStop.SetTextColor(Resources.GetColor(Resource.Color.hint));
            btnStopRecord.Enabled = false;
            btnStopRecord.Clickable = false;
            btnStopRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnStopRecord.SetTextColor(Resources.GetColor(Resource.Color.hint));
            btnStart.Enabled = true;
            btnStart.Clickable = true;
            btnStart.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
            btnStart.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));
            btnRecord.Enabled = true;
            btnRecord.Clickable = true;
            btnRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
            btnRecord.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));

            if (mediaplayer != null)
            {
                mediaplayer.Stop();
                mediaplayer.Release();
                SetupMediaRecorder();
            }
        }

        /// <summary>
        /// Stars the last record.
        /// </summary>
        void StarLastRecord()
        {

            btnStopRecord.Enabled = false;
            btnStopRecord.Clickable = false;
            btnStopRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnStopRecord.SetTextColor(Resources.GetColor(Resource.Color.hint));
            btnStop.Enabled = true;
            btnStop.Clickable = true;
            btnStop.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
            btnStop.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));
            btnRecord.Enabled = false;
            btnRecord.Clickable = false;
            btnRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnRecord.SetTextColor(Resources.GetColor(Resource.Color.hint));

            mediaplayer = new MediaPlayer();
            try
            {
                mediaplayer.SetDataSource(pathSave);
                mediaplayer.Prepare();
            }
            catch (Exception ex)
            {

                Log.Debug("DEBUG", ex.Message);
            }

            mediaplayer.Start();
            if (!mediaplayer.IsPlaying)
            {
                btnStop.Enabled = false;
                btnStop.Clickable = false;
                btnStop.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
                btnStop.SetTextColor(Resources.GetColor(Resource.Color.hint));
            }
        }


        /// <summary>
        /// Stops the recorder.
        /// </summary>
        void StopRecorder()
        {
            mediarecorder.Stop();
            statusRecording = false;
            btnStart.Enabled = true;
            btnStart.Clickable = true;
            btnStart.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
            btnStart.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));
            btnStop.Enabled = false;
            btnStop.Clickable = false;
            btnStop.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnStop.SetTextColor(Resources.GetColor(Resource.Color.hint));
            btnStopRecord.Enabled = false;
            btnStopRecord.Clickable = false;
            btnStopRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnStopRecord.SetTextColor(Resources.GetColor(Resource.Color.hint));
            btnRecord.Enabled = true;
            btnRecord.Clickable = true;
            btnRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
            btnRecord.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));
        }


        /// <summary>
        /// Prepares the player.
        /// </summary>
        void PreparePlayer()
        {
            pathSave = DataManager.audioPath;
            btnStart.Enabled = true;
            btnStart.Clickable = true;
            btnStart.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
            btnStart.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));
            btnStop.Enabled = false;
            btnStop.Clickable = false;
            btnStop.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnStop.SetTextColor(Resources.GetColor(Resource.Color.hint));
            btnStopRecord.Enabled = false;
            btnStopRecord.Clickable = false;
            btnStopRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.colorPrimary));
            btnStopRecord.SetTextColor(Resources.GetColor(Resource.Color.hint));
            btnRecord.Enabled = true;
            btnRecord.Clickable = true;
            btnRecord.SetBackgroundColor(Resources.GetColor(Resource.Color.tab_selected));
            btnRecord.SetTextColor(Resources.GetColor(Resource.Color.button_txt_color));
        }


        /// <summary>
        /// Setups the media recorder.
        /// </summary>
        void SetupMediaRecorder()
        {
            mediarecorder = new MediaRecorder();
            mediarecorder.SetMaxDuration(60000); //fix time grabación
            mediarecorder.SetAudioSource(AudioSource.Mic);
            mediarecorder.SetOutputFormat(OutputFormat.AacAdts);
            mediarecorder.SetAudioEncoder(AudioEncoder.Aac);
            mediarecorder.SetOutputFile(pathSave);
            mediarecorder.SetOnInfoListener(this);

        }

        /// <summary>
        /// Ons the start.
        /// </summary>
        public override void OnStart()
        {
            if (Dialog == null)
            {
                return;
            }

            Dialog.Window.SetWindowAnimations(Resource.Style.DlgAnimationMap);
            base.OnStart();
        }

        /// <summary>
        /// Ons the dismiss.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        public override void OnDismiss(IDialogInterface dialog)
        {
            var parentExist = FragmentManager.Fragments.OfType<DenunciaDialogFragment>();
            if(parentExist.Any())
            {
                parentExist.ElementAt(0).OnResume();
            }

            base.OnDismiss(dialog);
        }

        /// <summary>
        /// Ons the info.
        /// </summary>
        /// <param name="mr">Mr.</param>
        /// <param name="what">What.</param>
        /// <param name="extra">Extra.</param>
        public void OnInfo(MediaRecorder mr, [GeneratedEnum] MediaRecorderInfo what, int extra)
        {
            if (what == MediaRecorderInfo.MaxDurationReached)
            {
                var msj = Resources.GetString(Resource.String.audioTime);
                string title = Resources.GetString(Resource.String.audioTimeTit);
                Dialogs.Dialog(Activity, title, msj, "Aceptar");
                StopRecorder();
            }
        }
    }
}
