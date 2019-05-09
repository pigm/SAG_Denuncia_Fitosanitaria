using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.Accounts;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using DenunciaFitosanitaria.Fragments;
using DenunciaFitosanitaria.Models.Realm;
using DenunciaFitosanitaria.Services;
using DenunciaFitosanitaria.Services.Broadcast;
using DenunciaFitosanitaria.Utils;
using Realms;

namespace DenunciaFitosanitaria.Activities
{
    /// <summary>
    /// Main activity.
    /// </summary>
    [Activity(Name = "sag.denunciafitosanitaria.cl.MainActivity", Label = "Denuncia Fitosanitaria", ScreenOrientation = ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.AdjustResize, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class MainActivity : AppCompatActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        public NavigationView navigationView;
        public DrawerLayout drawerLayout;
        TextView version;
        RelativeLayout denunciasPendientesLayout;
        TextView badgeText;
        Context contex;
        LinearLayout parentLayout;
        ImageView denunciasPendientes;
        const int RequestLocationId = 0;
         static MainActivity ma;
        readonly string[] PermissionsLocation =
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission.AccessFineLocation,
                Manifest.Permission.Camera
            };

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="bundle">Bundle.</param>
        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            BroadcastNotice();
            ma = this;
            await GetLocationCompatAsync();
            getEmailId(contex);
            SetContentView(Resource.Layout.Main);
            parentLayout = FindViewById<LinearLayout>(Resource.Id.parentLayout);
            navigationView = FindViewById<NavigationView>(Resource.Id.navView);
            navigationView.InflateMenu(Resource.Menu.navmenu);
            navigationView.ItemIconTintList = null;
            Title = "Denuncias y Alertas SAG";

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarMenu);

            PackageInfo pInfo = PackageManager.GetPackageInfo(PackageName, 0);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Window.SetStatusBarColor(Resources.GetColor(Resource.Color.colorPrimary));
                toolbar.Elevation = 4;
            }
            version = FindViewById<TextView>(Resource.Id.version);
            //version.Text = Resources.GetString(Resource.String.version) + " " + pInfo.VersionName;

            contex = this;
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            SupportActionBar.SetDisplayHomeAsUpEnabled(false);
           /* drawerToggle = new Android.Support.V7.App.ActionBarDrawerToggle(this, drawerLayout, toolbar, Resource.String.menu_open_drawer, Resource.String.menu_close_drawer);
            drawerLayout.AddDrawerListener(drawerToggle);
            drawerToggle.SyncState();*/
            drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);

            if(bundle==null){
                var ft = SupportFragmentManager.BeginTransaction();
                ft.Add(Resource.Id.defaultFrame, new TiposDenunciasFragment(SupportFragmentManager), Resources.GetString(Resource.String.home));
                ft.Commit();
            }
            navigationView.NavigationItemSelected += NavigationView_NavigationItemSelected;


        }

        /// <summary>
        /// Navigations the view navigation item selected.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        void NavigationView_NavigationItemSelected(object sender, NavigationView.NavigationItemSelectedEventArgs e) //async #todo
        {
            switch (e.MenuItem.ItemId)
            {
                case (Resource.Id.menuitem1):
                    break;
            }

            drawerLayout.CloseDrawers();
        }

        /// <summary>
        /// Changes the main fragment.
        /// </summary>
        /// <param name="fragment">Fragment.</param>
        /// <param name="idFragment">Identifier fragment.</param>
        public void changeMainFragment(Android.Support.V4.App.Fragment fragment, string idFragment)
        {
            var ft = SupportFragmentManager.BeginTransaction();
            ft.Replace(Resource.Id.defaultFrame, fragment, idFragment);
            ft.Commit();
        }

        /// <summary>
        /// Gets the location compat async.
        /// </summary>
        /// <returns>The location compat async.</returns>
        async Task GetLocationCompatAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;
            if (ContextCompat.CheckSelfPermission(this, permission) == (int)Permission.Granted)
            {
                return;
            }

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, permission))
            {
                var dialog = new Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle("Acción requerida")
                                            .SetMessage("Esta aplicación necesita acceso a la localización del dispositivo")
                .SetPositiveButton("Aceptar", (senderAlert, args) =>
                {
                    RequestPermissions(PermissionsLocation, RequestLocationId);
                })
                .Show();
                return;
            }

            RequestPermissions(PermissionsLocation, RequestLocationId);
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
                case RequestLocationId:
                    {
                        if (grantResults[0] == (int)Permission.Granted)
                        {
                            var toast = Toast.MakeText(this, "Permisos asignados",
                                                       ToastLength.Short);
                            toast.Show();
                        }
                        else
                        {
                            var toast = Toast.MakeText(this, "Los permisos no fueron otorgados, la aplicación no se puede utilizar",
                                                      ToastLength.Long);
                            toast.Show();
                            closeApplication();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Closes the application.
        /// </summary>
        public void closeApplication()
        {
            var activity = this;
            activity.FinishAffinity();
        }

        public void OnLocationChanged(Location location)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderDisabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnProviderEnabled(string provider)
        {
            //throw new NotImplementedException();
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the email identifier.
        /// </summary>
        /// <returns>The email identifier.</returns>
        /// <param name="context">Context.</param>
         void getEmailId(Context context)
        {
            AccountManager account = AccountManager.Get(this);
            Account[] tabAccount = account.GetAccountsByType("com.google");

            //
            List<Account> list = new List<Android.Accounts.Account>();

            //
            foreach (Android.Accounts.Account act in tabAccount)
            {
                string name = act.Name;
                list.Add(act);
            }

            DataManager.cuentasMail = list;
        }

        /// <summary>
        /// Ons the resume.
        /// </summary>
        protected override void OnResume()
        {

            DataManager.backParent = false;
            if (!ValidationUtils.GetNetworkStatus())
            {
                Dialogs.ErrorService(parentLayout, Resources.GetString(Resource.String.sinInternet));
            }
            else
            {
                
                //var intent = new Intent(this, typeof(SendPeriodicNotice));
                //StartService(intent);
            }
            UpdateBadge();
            base.OnResume();
        }

        /// <summary>
        /// Sends the notice.
        /// </summary>
        private void SendNotice()
        {
            Intent alarmIntent = new Intent(this, typeof(SendPeriodicNotice));
            PendingIntent pendingIntent = PendingIntent.GetService(this, 0, alarmIntent, 0);
            AlarmManager manager = (AlarmManager)GetSystemService(Context.AlarmService);
            manager.SetRepeating(AlarmType.RtcWakeup, 60000, 60000, pendingIntent);
        }

        /// <summary>
        /// Broadcasts the notice.
        /// </summary>
        private void BroadcastNotice()
        {
            Intent i = new Intent(this, typeof(NoticeReceiver));
            PendingIntent pi = PendingIntent.GetBroadcast(this, 0, i, 0);
            AlarmManager alarmManager = (AlarmManager)GetSystemService(AlarmService);
            DataManager.mainActivity = this;
            alarmManager.SetInexactRepeating(AlarmType.ElapsedRealtime, SystemClock.CurrentThreadTimeMillis(),
             30000, pi);
        }

        /// <summary>
        /// Updates the badge.
        /// </summary>
        public void UpdateBadge()
        {
            denunciasPendientesLayout = FindViewById<RelativeLayout>(Resource.Id.denunciasPendientesLayout);
            denunciasPendientes = FindViewById<ImageView>(Resource.Id.denunciasPendientes);
            badgeText = FindViewById<TextView>(Resource.Id.badgeText);

            //denunciasPendientesLayout.SetOnClickListener(this);
            Realm instanceRealm = DataManager.RealmInstance;
            var pending = instanceRealm.All<Denuncia>().ToList();

            badgeText.Text = pending.Any() ? pending.Count().ToString() : "0";
            if (pending.Any())
            {
                denunciasPendientesLayout.Visibility = ViewStates.Visible;
                denunciasPendientes.SetColorFilter(Android.Graphics.Color.Orange);
            }
            else
            {
                denunciasPendientesLayout.Visibility = ViewStates.Gone;
                denunciasPendientes.SetColorFilter(Android.Graphics.Color.White);
            }
        }
    }
}
