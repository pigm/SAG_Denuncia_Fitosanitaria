using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using DenunciaFitosanitaria.Activities;
using DenunciaFitosanitaria.Adapters;
using DenunciaFitosanitaria.Data.Common.Models;
using DenunciaFitosanitaria.Services.Common.Delegate;
using DenunciaFitosanitaria.Utils;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Dynamic fragment.
    /// </summary>
    public class DynamicFragment : Fragment
    {
        List<Categoria> categorias;
        FragmentManager SupportFragmentManager;
        GridView categoriasView;
        Intent intent;
        LinearLayout fila_anonimo, linearLayoutCat, dynamicFragmentLinearLayout;
        bool incognito;
        Android.App.ProgressDialog _progressDialog;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Fragments.DynamicFragment"/> class.
        /// </summary>
        /// <param name="sfm">Sfm.</param>
        /// <param name="categorias">Categorias.</param>
        public DynamicFragment(FragmentManager sfm, List<Categoria> categorias,bool incognito)
        {
            SupportFragmentManager = sfm;
            this.categorias = categorias;
            this.incognito = incognito;
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
            _progressDialog.SetMessage(Resources.GetString(Resource.String.obteniendo));
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
            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.DynamicFragment, container, false);
            categoriasView = (GridView)v.FindViewById(Resource.Id.gridViewCat);
            dynamicFragmentLinearLayout = (LinearLayout)v.FindViewById(Resource.Id.dynamicFragmentLinearLayout);
            linearLayoutCat = (LinearLayout)v.FindViewById(Resource.Id.linearLayoutCat);
            fila_anonimo = (LinearLayout)v.FindViewById(Resource.Id.fila_anonimo);


            if(!incognito){
                fila_anonimo.Visibility = ViewStates.Gone;
                ViewGroup.LayoutParams paramsLayoutCat = linearLayoutCat.LayoutParameters;
                if(paramsLayoutCat is ViewGroup.MarginLayoutParams){
                    ((ViewGroup.MarginLayoutParams)paramsLayoutCat).TopMargin = 0;
                }

                linearLayoutCat.LayoutParameters = paramsLayoutCat;
            }

            CategoriasAdapter categoriasAdapter = new CategoriasAdapter(this.Activity,categorias);
            categoriasView.Adapter = categoriasAdapter;
            categoriasView.ItemClick += async delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                if(ValidationUtils.GetNetworkStatus()){
                    _progressDialog.Show();
                    _progressDialog.SetCancelable(false);
                    var categorySelected = categorias[args.Position];
                    DataManager.categoriaSelected = categorySelected;
                    var subcategorias = await ServiceDelegate.Instance.GetSubcategorias(DataManager.token, categorySelected.IdCategoria);
                    if (subcategorias.Success)
                    {
                        _progressDialog.Dismiss();
                        DataManager.subcategorias = (List<SubCategoria>)subcategorias.Response;
                        if (DataManager.subcategorias.Any())
                        {
                            intent = new Intent(this.Activity, typeof(SubCategoriasActivity));
                            StartActivityForResult(intent, 2);
                        }
                    }
                    else
                    {
                        _progressDialog.Dismiss();
                        Dialogs.ErrorService(dynamicFragmentLinearLayout);
                    }
                }else{
                    Dialogs.ErrorService(dynamicFragmentLinearLayout,Resources.GetString(Resource.String.sinInternet)); 
                }

            };
            return v;
        }

        /// <summary>
        /// Ons the resume.
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
        }

        /// <summary>
        /// Setea dinamicamente el alto de un gridview.
        /// </summary>
        /// <param name="gridView">Grid view.</param>
        void SetDynamicHeight(GridView gridView)
        {
            IListAdapter gridViewAdapter = gridView.Adapter;
            if (gridViewAdapter == null)
            {
                return;
            }

            int totalHeight = 0;
            int items = gridViewAdapter.Count;
            int rows = 0;

            View listItem = gridViewAdapter.GetView(0, null, gridView);
            listItem.Measure(0, 0);
            totalHeight = listItem.MeasuredHeight;

            float x = 1;
            if (items > 5)
            {
                x = items / 5;
                rows = (int)(x + 1);
                totalHeight *= rows;
                totalHeight = totalHeight + 1000;
                ViewGroup.LayoutParams layoutParams = gridView.LayoutParameters;
                layoutParams.Height = totalHeight;
                gridView.LayoutParameters = layoutParams;
            }
        }
    }
}
