using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using DenunciaFitosanitaria.Data.Common.Models;

namespace DenunciaFitosanitaria.Adapters
{
    /// <summary>
    /// Categorias adapter.
    /// </summary>
    public class CategoriasAdapter : BaseAdapter
    {
        Context adapterContext;
        List<Categoria> dataCategory;
        public CategoriasAdapter(Context context, List<Categoria> dataCategory){
            this.adapterContext = context;
            this.dataCategory = dataCategory;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public override int Count => dataCategory.Count();

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
                convertView = inflater.Inflate(Resource.Layout.CategoryLayout, null);
            }

            TextView titulo = (TextView)convertView.FindViewById(Resource.Id.categoria_name);
            ImageView image = (ImageView)convertView.FindViewById(Resource.Id.image_categoria);
            titulo.Text = dataCategory[position].Nombre.ToUpper();

            byte[] imageBytes = Convert.FromBase64String(dataCategory[position].ImagenEncrypt);
            image.SetImageBitmap(BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length));
            return convertView;
        }

    }
}
