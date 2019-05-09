using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DenunciaFitosanitaria.Adapters;
using DenunciaFitosanitaria.Fragments;
using DenunciaFitosanitaria.Utils;
using Android.Content;
using Android.Runtime;
using Android.Views.Animations;

namespace DenunciaFitosanitaria.Activities
{
    /// <summary>
    /// Sub categoria detalle activity.
    /// </summary>
    [Activity(Name = "sag.denunciafitosanitaria.cl.SubCategoriaDetalleActivity", Label = "Denuncia Fitosanitaria", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class SubCategoriaDetalleActivity : AppCompatActivity
    {
        ViewPager viewPager;
        TabLayout mTopNavigation;
        TextView subcategoriaDescripcion;
        Button btnForm;
        RelativeLayout relativeLayoutDetail;
        AlphaAnimation buttonAnimation = new AlphaAnimation(1F, 0.4F);

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SubCategoryDetailLayout);
            relativeLayoutDetail = FindViewById<RelativeLayout>(Resource.Id.relativeLayoutDetail);
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMenu);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            TextView titleView = FindViewById<TextView>(Resource.Id.tittleBar);
            titleView.Text = DataManager.subcategoriaSelected.Nombre;
            Title = DataManager.subcategoriaSelected.Nombre;

            toolbar.Click += delegate {
                var activity = this;
                activity.Finish();
            };

            viewPager = (ViewPager)FindViewById(Resource.Id.viewPagerSub);
            subcategoriaDescripcion = (TextView)FindViewById(Resource.Id.subcategoriaDescripcion);
            btnForm = (Button)FindViewById(Resource.Id.btnForm);

            subcategoriaDescripcion.Text = DataManager.subcategoriaSelected.Descripcion;
            setupViewPager(viewPager);
            mTopNavigation = (TabLayout)FindViewById(Resource.Id.tabsFrgSub);
            mTopNavigation.SetupWithViewPager(viewPager);

            btnForm.Animation = buttonAnimation;
            btnForm.Click += delegate {
                DataManager.denunciaRapida = false;
                DenunciaDialogFragment denunciaDialogFragment = new DenunciaDialogFragment();
                var transcation = SupportFragmentManager.BeginTransaction();
                denunciaDialogFragment.SetStyle(0, Resource.Style.AlertDialog_AppCompat);
                denunciaDialogFragment.Show(transcation, denunciaDialogFragment.Tag);
                denunciaDialogFragment.Cancelable = true;
                    
            };
            // Create your application here
        }

        /// <summary>
        /// Setups the view pager.
        /// </summary>
        /// <param name="viewPager">View pager.</param>
        public void setupViewPager(ViewPager viewPager)
        {
            GenericFragmentPagerAdapter adapter = new GenericFragmentPagerAdapter(SupportFragmentManager);
            var total = (DataManager.subcategoriaDetalle.ImagenEncrypt1 == null || DataManager.subcategoriaDetalle.ImagenEncrypt1 == string.Empty ? 0 : 1) +
                (DataManager.subcategoriaDetalle.ImagenEncrypt2 == null || DataManager.subcategoriaDetalle.ImagenEncrypt2 == string.Empty ? 0 : 1) +
                (DataManager.subcategoriaDetalle.ImagenEncrypt3 == null || DataManager.subcategoriaDetalle.ImagenEncrypt3 == string.Empty ? 0 : 1) +
                (DataManager.subcategoriaDetalle.ImagenEncrypt4 == null || DataManager.subcategoriaDetalle.ImagenEncrypt4 == string.Empty ? 0 : 1) +
                1;
            
            for (var x = 1; x < total; x++)
            {
                var dynamic = new DynamicFragmentSubcategoria(SupportFragmentManager, x);
                adapter.addFragment(dynamic, "");
            }

            viewPager.Adapter = adapter;
        }

        /// <summary>
        /// Ons the back pressed.
        /// </summary>
        public override void OnBackPressed()
        {
            var activity = this;
            try{
                activity.Finish();
            }catch{
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
        /// Ons the activity result.
        /// </summary>
        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
        }

        /// <summary>
        /// Ons the resume.
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();
            if (!ValidationUtils.GetNetworkStatus())
            {
                Dialogs.ErrorService(relativeLayoutDetail,Resources.GetString(Resource.String.sinInternet)); 
            }
        }
    }
}
