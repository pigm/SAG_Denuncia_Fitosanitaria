using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using DenunciaFitosanitaria.Data.Common.Models;
using DenunciaFitosanitaria.Services.Common.Delegate;
using DenunciaFitosanitaria.Utils;

namespace DenunciaFitosanitaria.Activities
{
    /// <summary>
    /// Activity splash.
    /// </summary>
    [Activity(Name = "sag.denunciafitosanitaria.cl.ActivitySplash", Label = "Denuncia Fitosanitaria", NoHistory = true, MainLauncher = true, Theme = "@style/DenunciaFitosanitaria.Splash", Icon = "@drawable/logo_sag", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class ActivitySplash : Activity
    {
        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            DataManager.mapKey = Resources.GetString(Resource.String.google_maps_key);
        }

        /// <summary>
        /// Ons the resume.
        /// </summary>
        protected override async void OnResume()
        {
            base.OnResume();
            //var nn = DataManager.RealmInstance.Config.DatabasePath;
            if (ValidationUtils.GetNetworkStatus())
            {
                await GetAuthorizationAsync();
            }
            else
            {
                Snackbar.Make(Window.DecorView.RootView, "La aplicación requiere internet para su uso", Snackbar.LengthIndefinite)
                        .SetAction("OK", (view) => { Finish(); })
                        .Show();
            }
        }


        /// <summary>
        /// Gets the authorization.
        /// </summary>
        public async Task GetAuthorizationAsync()
        {
            try
            {
                var authorization = await ServiceDelegate.Instance.GetToken();
                if (authorization.Success)
                {
                    DataManager.token = authorization.TokenResponse as string;
                    var tiposDenuncia = await ServiceDelegate.Instance.GetTiposDenuncia(DataManager.token);
                    if (tiposDenuncia.Success)
                    {
                        DataManager.tiposDenuncia = tiposDenuncia.Response as List<TipoDenuncia>;
                        var categorias = await ServiceDelegate.Instance.GetCategorias(DataManager.token);
                        if (categorias.Success)
                        {
                            DataManager.categorias = categorias.Response as List<Categoria>;
                        }
                        else
                        {
                            Dialogs.ErrorServiceFinish(Window.DecorView.RootView,this);
                        }
                    }
                    else
                    {
                        Dialogs.ErrorServiceFinish(Window.DecorView.RootView,this);
                    }
                    Task startupwork = new Task(() =>
                    {
                        Task.Delay(300).Wait();

                    });

                    startupwork.ContinueWith(t =>
                    {
                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                    startupwork.Start();
                }else{
                    Dialogs.ErrorServiceFinish(Window.DecorView.RootView,this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}