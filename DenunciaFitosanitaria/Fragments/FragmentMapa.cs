using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using DenunciaFitosanitaria.Utils;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Fragment mapa.
    /// </summary>
    public class FragmentMapa : Android.Support.V4.App.DialogFragment, IOnMapReadyCallback, GoogleMap.IOnMarkerClickListener, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener,GoogleMap.IOnCameraIdleListener,
    GoogleMap.IOnCameraMoveStartedListener,GoogleMap.IOnCameraMoveListener,GoogleMap.IOnCameraMoveCanceledListener, Android.Locations.ILocationListener
    {
        GoogleApiClient client;
        SupportMapFragment mapFragment;
        GoogleMap map;
        LatLng center;
        Button buttonClose,buttonCancelMap,buttonSendMap, btnMapViewNormal, btnMapViewSatelite;
        ImageView mapCenter;
        LocationManager locMgr;
        AlphaAnimation buttonAnimation = new AlphaAnimation(1F, 0.4F);
        readonly string[] PermissionsLocation =
        {
              Manifest.Permission.AccessCoarseLocation,
              Manifest.Permission.AccessFineLocation
        };
        const string permission = Manifest.Permission.AccessFineLocation;
        const int RequestLocationId = 0;

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
            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.MapFragment, container, false);
            btnMapViewNormal = (Button)v.FindViewById(Resource.Id.btnMapViewNormal);
            btnMapViewSatelite = (Button)v.FindViewById(Resource.Id.btnMapViewSatelite);
            buttonClose = (Button)v.FindViewById(Resource.Id.buttonCloseMap);
            buttonCancelMap = (Button)v.FindViewById(Resource.Id.buttonCancelMap);
            buttonSendMap = (Button)v.FindViewById(Resource.Id.buttonSendMap);
            mapCenter = (ImageView)v.FindViewById(Resource.Id.mapCenter);

            btnMapViewNormal.Click += delegate {
                if (map != null){
                    btnMapViewNormal.SetTextColor(Resources.GetColor(Resource.Color.tab_selected));
                    btnMapViewSatelite.SetTextColor(Resources.GetColor(Resource.Color.black));
                    map.MapType = GoogleMap.MapTypeNormal;
                }
            };
            btnMapViewSatelite.Click += delegate {
                if (map != null){
                    btnMapViewNormal.SetTextColor(Resources.GetColor(Resource.Color.black));
                    btnMapViewSatelite.SetTextColor(Resources.GetColor(Resource.Color.tab_selected));
                    map.MapType = GoogleMap.MapTypeSatellite;
                }
            };
            buttonClose.Animation = buttonAnimation;
            buttonClose.Click += delegate{
                DataManager.mapCenter = null;
                DismissAllowingStateLoss();
            };

            buttonCancelMap.Animation = buttonAnimation;
            buttonCancelMap.Click += delegate {
                DataManager.mapCenter = null;
                DismissAllowingStateLoss();
            };

            buttonSendMap.Animation = buttonAnimation;
            buttonSendMap.Click += delegate
            {
                DataManager.mapCenter = center;
                DismissAllowingStateLoss();
            };

            mapFragment = (SupportMapFragment)FragmentManager.FindFragmentById(Resource.Id.mapViewDenuncia);
            mapFragment.GetMapAsync(this);
            mapCenter.BringToFront();
            return v;
        }

        /// <summary>
        /// Ons the map ready.
        /// </summary>
        /// <param name="googleMap">Google map.</param>
        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            ApliClientBuild();
            if (map != null)
            {
                LatLng location;
                map.MyLocationEnabled = true;

                const string permission = Manifest.Permission.AccessFineLocation;
                Location loc;
                loc = GetLocation(permission);

                if (loc != null)
                {
                    location = new LatLng(loc.Latitude, loc.Longitude);

                    map.MyLocationButtonClick += delegate
                    {
                        var myLoc = GetLocation(permission);
                        var mylocation = new LatLng(myLoc.Latitude, myLoc.Longitude);
                        map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(mylocation, 14.0f));

                    };
                }
                else
                {
                    //si no puede obtener mis corrdenadas gps
                    location = new LatLng(-33.53058, -70.674187);
                    map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(location, 14.0f));

                }
                location = DataManager.mapCenter ?? location;
                map.MoveCamera(CameraUpdateFactory.NewLatLng(location));
                CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                builder.Target(location);
                builder.Zoom(10);
                builder.Bearing(0);
                CameraPosition cameraPosition = builder.Build();
                CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
                map.MapType = GoogleMap.MapTypeNormal;
                map.MoveCamera(cameraUpdate);
                map.SetOnMarkerClickListener(this);
                map.SetOnCameraIdleListener(this);
                map.SetOnCameraMoveStartedListener(this);
                map.SetOnCameraMoveListener(this);
                map.SetOnCameraMoveCanceledListener(this);


            }

            else
            {
                Toast.MakeText(Activity, "No se ha podido cargar el mapa", ToastLength.Short).Show();
            }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <returns>The location.</returns>
        /// <param name="permission">Permission.</param>
        private Location GetLocation(string permission)
        {
            Location loc;
            if (ContextCompat.CheckSelfPermission(this.Activity, permission) == (int)Permission.Granted)
            {
                locMgr = Activity.GetSystemService(Context.LocationService) as LocationManager;
                locMgr.RequestLocationUpdates(LocationManager.NetworkProvider, 0, 1, this);
                loc = locMgr.GetLastKnownLocation(LocationManager.NetworkProvider);
            }
            else
            {
                loc = LocationServices.FusedLocationApi.GetLastLocation(client);
            }

            return loc;
        }


        /// <summary>
        /// Aplis the client build.
        /// </summary>
        public void ApliClientBuild()
        {
            if(client== null){
                client = new GoogleApiClient.Builder(Activity)
                                                        .AddConnectionCallbacks(this)
                                                        .AddOnConnectionFailedListener(this)
                                                        .AddApi(LocationServices.API)
                                                        .EnableAutoManage((Android.Support.V4.App.FragmentActivity)Activity, this)
                                                        .Build();
            }
        }

        /// <summary>
        /// Ons the camera idle.
        /// </summary>
        /******MOVIMIENTO CAMARA HANDLERS****/
        public void OnCameraIdle()
        {
            center = map.CameraPosition.Target;
        }

        /// <summary>
        /// Ons the camera move started.
        /// </summary>
        /// <param name="reason">Reason.</param>
        public void OnCameraMoveStarted(int reason)
        {
          
        }

        /// <summary>
        /// Ons the camera move.
        /// </summary>
        public void OnCameraMove()
        {
          
        }

        /// <summary>
        /// Ons the camera move canceled.
        /// </summary>
        public void OnCameraMoveCanceled()
        {
            
        }

        /// <summary>
        /// Ons the marker click.
        /// </summary>
        /// <returns><c>true</c>, if marker click was oned, <c>false</c> otherwise.</returns>
        /// <param name="marker">Marker.</param>
        /***CONEXION MAPA HANDLERS***/
        public bool OnMarkerClick(Marker marker)
        {
            return true;
        }

        /// <summary>
        /// Ons the connected.
        /// </summary>
        /// <param name="connectionHint">Connection hint.</param>
        public void OnConnected(Bundle connectionHint)
        {
            return;
        }

        /// <summary>
        /// Ons the connection failed.
        /// </summary>
        /// <param name="result">Result.</param>
        public void OnConnectionFailed(ConnectionResult result)
        {
            return;
        }

        /// <summary>
        /// Ons the connection suspended.
        /// </summary>
        /// <param name="cause">Cause.</param>
        public void OnConnectionSuspended(int cause)
        {
            return;
        }

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
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
        /// Ons the destroy view.
        /// </summary>
        public override void OnDestroyView()
        {
            RemoveFrg();

            base.OnDestroyView();
        }


        /// <summary>
        /// Ons the destroy.
        /// </summary>
        public override void OnDestroy()
        {
            RemoveFrg();
            base.OnDestroy();
        }

        /// <summary>
        /// Removes the frg.
        /// </summary>
        void RemoveFrg()
        {
            
            mapFragment = (SupportMapFragment)FragmentManager.FindFragmentById(Resource.Id.mapViewDenuncia);
            if (mapFragment != null)
            {
                FragmentManager.BeginTransaction().Remove(mapFragment).Commit();
            }
        }

        /// <summary>
        /// Ons the pause.
        /// </summary>
        public override void OnPause()
        {
            if (client != null && client.IsConnected)
            {
                client.StopAutoManage(this.Activity);
                client.Disconnect();
            }
            base.OnPause();
        }

        /// <summary>
        /// Ons the dismiss.
        /// </summary>
        /// <param name="dialog">Dialog.</param>
        public override void OnDismiss(IDialogInterface dialog)
        {
            var parentExist = FragmentManager.Fragments.OfType<DenunciaDialogFragment>();
            if (parentExist.Any())
            {
                parentExist.ElementAt(0).OnResume();
            }

            base.OnDismiss(dialog);
        }

        /// <summary>
        /// Ons the location changed.
        /// </summary>
        /// <param name="location">Location.</param>
        public void OnLocationChanged(Location location)
        {
           // throw new System.NotImplementedException();
        }

        /// <summary>
        /// Ons the provider disabled.
        /// </summary>
        /// <param name="provider">Provider.</param>
        public void OnProviderDisabled(string provider)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Ons the provider enabled.
        /// </summary>
        /// <param name="provider">Provider.</param>
        public void OnProviderEnabled(string provider)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Ons the status changed.
        /// </summary>
        /// <param name="provider">Provider.</param>
        /// <param name="status">Status.</param>
        /// <param name="extras">Extras.</param>
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
            //throw new System.NotImplementedException();
        }
    }
}
