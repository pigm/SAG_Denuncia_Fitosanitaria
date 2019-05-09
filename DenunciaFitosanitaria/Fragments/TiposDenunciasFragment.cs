using System.Linq;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using DenunciaFitosanitaria.Adapters;
using DenunciaFitosanitaria.Data.Common.Models;
using DenunciaFitosanitaria.Utils;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Tipos denuncias fragment.
    /// </summary>
    public class TiposDenunciasFragment : Fragment,TabLayout.IOnTabSelectedListener
    {
        TabLayout mTopNavigation;
        ViewPager viewPager;
        FragmentManager SupportFragmentManager;

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
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Fragments.TiposDenunciasFragment"/> class.
        /// </summary>
        /// <param name="sfm">Sfm.</param>
        public TiposDenunciasFragment(FragmentManager sfm)
        {
            SupportFragmentManager = sfm;
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
            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.TiposFragment, container, false);
            return v;
        }

        /// <summary>
        /// Ons the view created.
        /// </summary>
        /// <param name="view">View.</param>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            viewPager = (ViewPager)view.FindViewById(Resource.Id.viewPager);
            setupViewPager(viewPager);
            mTopNavigation = (TabLayout)view.FindViewById(Resource.Id.tabsFrg);
            mTopNavigation.SetTabTextColors(Color.White, Color.Gold);
            mTopNavigation.SetupWithViewPager(viewPager);

        }

        /// <summary>
        /// Setups the view pager.
        /// </summary>
        /// <param name="viewPager">View pager.</param>
        public void setupViewPager(ViewPager viewPager)
        {
            GenericFragmentPagerAdapter adapter = new GenericFragmentPagerAdapter(SupportFragmentManager);
            foreach(TipoDenuncia tipo in DataManager.tiposDenuncia){
                var categorias = DataManager.categorias.Where(cat => cat.IdTipoDenuncia == tipo.IdTipoDenuncia).ToList();
                var dynamic = new DynamicFragment(SupportFragmentManager, categorias,tipo.Incognito);
                adapter.addFragment(dynamic,tipo.Nombre.ToUpper());
            }

                /*var dynamic = new FragmentOfertas(SupportFragmentManager);
                var dynamic2 = new FragmentPasillos(SupportFragmentManager);
                adapter.addFragment(dynamic, "Ofertas");
                adapter.addFragment(dynamic2, "Pasillo");*/

            viewPager.Adapter = adapter;
        }

        /// <summary>
        /// Ons the tab reselected.
        /// </summary>
        /// <param name="tab">Tab.</param>
        public void OnTabReselected(TabLayout.Tab tab)
        {
        }

        /// <summary>
        /// Ons the tab selected.
        /// </summary>
        /// <param name="tab">Tab.</param>
        public void OnTabSelected(TabLayout.Tab tab)
        {
            var pos = tab.Position;
        }

        /// <summary>
        /// Ons the tab unselected.
        /// </summary>
        /// <param name="tab">Tab.</param>
        public void OnTabUnselected(TabLayout.Tab tab)
        {
            
        }
    }
}
