using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using DenunciaFitosanitaria.Utils;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Galeria fragment.
    /// </summary>
    public class GaleriaFragment : Android.Support.V4.App.DialogFragment, View.IOnClickListener
    {
        ImageView fotoImg, galeriaImg,photoView;
        Button buttonClose, buttonCancel, buttonAceptar;
        AlphaAnimation buttonAnimation = new AlphaAnimation(1F, 0.4F);

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetStyle(StyleNoTitle, 0);
            // Create your fragment here
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
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.GalleryFragment, container, false);
            photoView = (ImageView)v.FindViewById(Resource.Id.photoView);
            fotoImg = (ImageView)v.FindViewById(Resource.Id.fotoBtn);
            galeriaImg = (ImageView)v.FindViewById(Resource.Id.galeryBtn);
            buttonClose = (Button)v.FindViewById(Resource.Id.buttonCloseFoto);
            buttonCancel = (Button)v.FindViewById(Resource.Id.buttonCancelFoto);
            buttonAceptar = (Button)v.FindViewById(Resource.Id.buttonSendFoto);
            fotoImg.SetOnClickListener(this);
            galeriaImg.SetOnClickListener(this);

            buttonAceptar.Animation = buttonAnimation;
            buttonAceptar.Click += delegate {
                Dismiss();
            };

            buttonClose.Animation = buttonAnimation;
            buttonClose.Click += delegate {
                DataManager.imagePath = null;
                DataManager.imageData = null;
                DataManager.thumbnail = null;
                Dismiss();
            };

            buttonCancel.Animation = buttonAnimation;
            buttonCancel.Click += delegate {
                DataManager.imagePath = null;
                DataManager.imageData = null;
                DataManager.thumbnail = null;
                Dismiss();
            };

            if(DataManager.imagePath!=null){
                try{
                    if (DataManager.imageData != null)
                    {
                        photoView.SetImageBitmap(DataManager.thumbnail);
                    }
                }catch(Exception e){
                    
                }
            }

            return v;
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
            try{
                var parentExist = FragmentManager.Fragments.OfType<DenunciaDialogFragment>();
                if (parentExist.Any())
                {
                    parentExist.ElementAt(0).OnResume();
                }
            }catch{
                
            }


            base.OnDismiss(dialog);
        }

        /// <summary>
        /// Ons the click.
        /// </summary>
        /// <param name="v">V.</param>
        public void OnClick(View v)
        {
            CreateDirectoryForPictures();
            v.StartAnimation(buttonAnimation);
            if (v.ToString().Contains("galery"))
            {
                var imageIntent = new Intent();
                imageIntent.SetType("image/*");
                imageIntent.SetAction(Intent.ActionGetContent);
                var parentExist = FragmentManager.Fragments.OfType<GaleriaFragment>();
                if (parentExist.Any())
                {
                    parentExist.ElementAt(0).StartActivityForResult(Intent.CreateChooser(imageIntent, "Selecciona foto"), 0);
                }
            }

                
                if (v.ToString().Contains("foto"))
                {
                    try
                    {
                        CreateDirectoryForPictures();
                        var intent = new Intent(MediaStore.ActionImageCapture);
                        AppUtils._file = new Java.IO.File(AppUtils._dir, System.String.Format("denunciaPhoto_{0}.jpg", Guid.NewGuid()));
                        intent.AddFlags(ActivityFlags.GrantReadUriPermission);//FLAG_GRANT_READ_URI_PERMISSION);
                        //intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(AppUtils._file));
                        intent.PutExtra(MediaStore.ExtraOutput, FileProvider.GetUriForFile(Activity, Activity.ApplicationContext.PackageName + ".sag.denunciafitosanitaria.cl", AppUtils._file));           
                        StartActivityForResult(intent, 1);
                    }
                    catch (Exception ex)
                    {
                        Log.Info("metodo onclick", ex.Message);
                    }
                }
        }  

        /// <summary>
        /// Ons the activity result.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        public override void OnActivityResult(int requestCode, int resultCode, Intent data)//async #todo
        {
            base.OnActivityResult(requestCode, resultCode, data);
            int height = Resources.DisplayMetrics.HeightPixels;
            int width = photoView.Width;

            if(resultCode == -1){
                if (requestCode == 0)
                {
                    DataManager.imagePath = data.Data.Path;
                    DataManager.imageData = data.Data;
                    DataManager.imageFrom = 1;
                    try
                    {
                        Android.Net.Uri uri = data.Data;
                        var contentResolver = this.Activity.ContentResolver.OpenInputStream(uri);
                        Bitmap bmp = BitmapFactory.DecodeStream(contentResolver);
                        Matrix matrix = new Matrix();
                        var finalBmp = Bitmap.CreateBitmap(bmp, 0, 0, bmp.Width, bmp.Height, matrix, true);
                        AppUtils.bitmap = finalBmp;

                        if (AppUtils.bitmap != null)
                        {
                            DataManager.thumbnail = AppUtils.bitmap;
                            photoView.SetImageBitmap(AppUtils.bitmap);
                            AppUtils.bitmap = null;
                        }

                        GC.Collect();
                    }
                    catch(Exception e)
                    {
                    }


                }else if (requestCode == 1){

                    Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                    Android.Net.Uri contentUri = FileProvider.GetUriForFile(Activity, Activity.ApplicationContext.PackageName + ".sag.denunciafitosanitaria.cl", AppUtils._file);
                    mediaScanIntent.SetData(contentUri);
                    var parentExist = FragmentManager.Fragments.OfType<GaleriaFragment>();
                    parentExist.ElementAt(0).Context.SendBroadcast(mediaScanIntent);

                    DataManager.imagePath = AppUtils._file.Path;
                    DataManager.imageData = contentUri;
                    DataManager.imageFrom = 2;

                    try{
                        AppUtils.bitmap = AppUtils._file.Path.LoadAndResizeBitmap(width, height);

                        if (AppUtils.bitmap != null)
                        {
                            DataManager.thumbnail = AppUtils.bitmap;
                            photoView.SetImageBitmap(AppUtils.bitmap);
                            AppUtils.bitmap = null;
                            GC.Collect();
                        }
                    }catch (Exception e)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Creates the directory for pictures.
        /// </summary>
        private void CreateDirectoryForPictures()
        {
            AppUtils._dir = new Java.IO.File(
                Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures), "DenunciaFitosanitariaPhotos");
            if (!AppUtils._dir.Exists())
            {
                AppUtils._dir.Mkdirs();
            }
        }

    }
}
