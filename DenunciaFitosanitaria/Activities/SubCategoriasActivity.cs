using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using DenunciaFitosanitaria.Adapters;
using DenunciaFitosanitaria.Data.Common.Models;
using DenunciaFitosanitaria.Fragments;
using DenunciaFitosanitaria.Services.Common.Delegate;
using DenunciaFitosanitaria.Utils;

namespace DenunciaFitosanitaria.Activities
{
    /// <summary>
    /// Sub categorias activity.
    /// </summary>
    [Activity(Name = "sag.denunciafitosanitaria.cl.SubCategoriasActivity", Label = "Denuncia Fitosanitaria", ScreenOrientation = ScreenOrientation.Portrait|ScreenOrientation.Landscape, WindowSoftInputMode = SoftInput.AdjustResize, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class SubCategoriasActivity : AppCompatActivity
    {
        GridView subCategoriasView;
        Intent intent;
        Button btnForm;
        RelativeLayout relLayoutParent;
        ScrollView scrollViewSubCat;
        Android.App.ProgressDialog _progressDialog;
        AlphaAnimation buttonAnimation = new AlphaAnimation(1F, 0.4F);

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SubCategoryActivity);
            relLayoutParent = FindViewById<RelativeLayout>(Resource.Id.relLayoutParent);
            scrollViewSubCat = FindViewById<ScrollView>(Resource.Id.scrollViewSubCat);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMenu);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            _progressDialog = new Android.App.ProgressDialog(this);
            _progressDialog.SetProgressStyle(Android.App.ProgressDialogStyle.Spinner);
            _progressDialog.SetMessage(Resources.GetString(Resource.String.obteniendo));

            toolbar.Click += delegate {
                var activity = this;
                activity.Finish();
            };
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Resources.GetColor(Resource.Color.colorPrimary));
                toolbar.Elevation = 4;
            } 

            btnForm = FindViewById<Button>(Resource.Id.btndenunciarapid);
            TextView titleView = FindViewById<TextView>(Resource.Id.tittleBar);
            titleView.Text = DataManager.categoriaSelected.Nombre;
            Title = DataManager.categoriaSelected.Nombre;
            subCategoriasView = FindViewById<GridView>(Resource.Id.gridViewSubCat);

            var i = (int)TypedValue.ApplyDimension(ComplexUnitType.Px, (250 * (DataManager.subcategorias.Count())) + 50, this.Resources.DisplayMetrics);
            if (DataManager.subcategorias.Count() == 1)
            {
                i = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, 180 * (DataManager.subcategorias.Count()), this.Resources.DisplayMetrics);
            }
            //subCategoriasView.LayoutParameters.Height = i;
            //subCategoriasView.RequestLayout();
            SubCategoriasAdapter categoriasAdapter = new SubCategoriasAdapter(this, DataManager.subcategorias);
            subCategoriasView.Adapter = categoriasAdapter;
            subCategoriasView.ItemClick += async delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                if (ValidationUtils.GetNetworkStatus())
                { 
                    _progressDialog.Show();
                    _progressDialog.SetCancelable(false);
                    var subcategorySelected = DataManager.subcategorias[args.Position];
                    DataManager.subcategoriaSelected = subcategorySelected;
                    var subcategorias = await ServiceDelegate.Instance.GetSubcategoriaDetalle(DataManager.token, subcategorySelected.IdSubCategoria);
                    if (subcategorias.Success)
                    {
                        _progressDialog.Dismiss();
                        DataManager.subcategoriaDetalle = (SubCategoriaDetalle)subcategorias.Response;
                        if (DataManager.subcategoriaDetalle.IdSubCategoria != 0)
                        {
                            intent = new Intent(this, typeof(SubCategoriaDetalleActivity));
                            StartActivityForResult(intent, 2);
                        }
                    }
                    else
                    {
                        _progressDialog.Dismiss();
                        Dialogs.ErrorService(relLayoutParent);
                    }
                }else{
                    Dialogs.ErrorService(relLayoutParent,Resources.GetString(Resource.String.sinInternet));
                }
               
            };

            btnForm.Animation = buttonAnimation;
            btnForm.Click += delegate {
                var subcategorySelected = DataManager.subcategorias[0];
                DataManager.subcategoriaSelected = subcategorySelected;
                DataManager.denunciaRapida = true;
                DenunciaDialogFragment denunciaDialogFragment = new DenunciaDialogFragment();
                var transcation = SupportFragmentManager.BeginTransaction();
                denunciaDialogFragment.SetStyle(0, Resource.Style.AlertDialog_AppCompat);
                denunciaDialogFragment.Show(transcation, denunciaDialogFragment.Tag);
                denunciaDialogFragment.Cancelable = true;

            };

            // Create your application here
        }

        /// <summary>
        /// Ons the back pressed.
        /// </summary>
        public override void OnBackPressed()
        {
            var activity = this;
            try
            {
                activity.Finish();
            }
            catch
            {
                activity.OnBackPressed();
            }
        }

        /// <summary>
        /// Ons the support navigate up.
        /// </summary>
        /// <returns><c>true</c>, if support navigate up was oned, <c>false</c> otherwise.</returns>
        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        /// <summary>
        /// Ons the resume.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            ViewGroup.LayoutParams paramsLayoutCat = scrollViewSubCat.LayoutParameters;
            if (paramsLayoutCat is ViewGroup.MarginLayoutParams)
            {
                ((ViewGroup.MarginLayoutParams)paramsLayoutCat).Height = ((ViewGroup.MarginLayoutParams)paramsLayoutCat).Height + 100;
            }

            scrollViewSubCat.LayoutParameters = paramsLayoutCat;

            if (!ValidationUtils.GetNetworkStatus())
            {
                Dialogs.ErrorService(relLayoutParent,Resources.GetString(Resource.String.sinInternet)); 
            }
            if(DataManager.backParent){
                Finish();
            }
        }
    }
}
