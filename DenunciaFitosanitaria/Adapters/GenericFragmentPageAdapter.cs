using System.Collections.Generic;
using Android.Support.V4.App;
using Java.Lang;

namespace DenunciaFitosanitaria.Adapters
{
    /// <summary>
    /// Generic fragment pager adapter.
    /// </summary>
    public class GenericFragmentPagerAdapter : FragmentStatePagerAdapter
    {
        List<Fragment> fragmentList = new List<Fragment>();
        List<string> titleList = new List<string>();
        FragmentManager mFragmentManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Adapters.GenericFragmentPagerAdapter"/> class.
        /// </summary>
        /// <param name="fm">Fm.</param>
        public GenericFragmentPagerAdapter(FragmentManager fm) : base(fm)
        {
            mFragmentManager = fm;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public override int Count
        {
            get
            {
                return fragmentList.Count;
            }
        }

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="position">Position.</param>
        public override Fragment GetItem(int position)
        {
            return fragmentList[position];
        }

        /// <summary>
        /// Gets the page title formatted.
        /// </summary>
        /// <returns>The page title formatted.</returns>
        /// <param name="position">Position.</param>
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(titleList[position].ToLower());
        }

        /// <summary>
        /// Adds the fragment.
        /// </summary>
        /// <param name="fragment">Fragment.</param>
        /// <param name="title">Title.</param>
        public void addFragment(Fragment fragment, string title)
        {
            fragmentList.Add(fragment);
            titleList.Add(title);
        }
    }
}
