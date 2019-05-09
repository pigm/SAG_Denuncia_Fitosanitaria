using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using DenunciaFitosanitaria.Data.Common.Models;

namespace DenunciaFitosanitaria.Adapters
{
    /// <summary>
    /// Sub categorias adapter.
    /// </summary>
    public class SubCategoriasAdapter : BaseAdapter
    {
        Context adapterContext;
        List<SubCategoria> dataSubCategory;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Adapters.SubCategoriasAdapter"/> class.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="dataSubCategory">Data sub category.</param>
        public SubCategoriasAdapter(Context context, List<SubCategoria> dataSubCategory)
        {
            this.adapterContext = context;
            this.dataSubCategory = dataSubCategory;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public override int Count => dataSubCategory.Count();

        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <returns>The item.</returns>
        /// <param name="position">Position.</param>
        public override Java.Lang.Object GetItem(int position)
        {
            return 0;
        }

        /// <summary>
        /// Gets the item identifier.
        /// </summary>
        /// <returns>The item identifier.</returns>
        /// <param name="position">Position.</param>
        public override long GetItemId(int position)
        {
            return position;
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <returns>The view.</returns>
        /// <param name="position">Position.</param>
        /// <param name="convertView">Convert view.</param>
        /// <param name="parent">Parent.</param>
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)adapterContext.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.SubCategoryLayout, null);
            }

            TextView titulo = (TextView)convertView.FindViewById(Resource.Id.subcategoria_name);
            ImageView image = (ImageView)convertView.FindViewById(Resource.Id.image_subcategoria);
            titulo.Text = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dataSubCategory[position].Nombre);

            byte[] imageBytes = Convert.FromBase64String(dataSubCategory[position].ImagenEncrypt);
            image.SetImageBitmap(BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length));
            return convertView;
        }

    }
}
