using System;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using DenunciaFitosanitaria.Data.Common.Models;
using DenunciaFitosanitaria.Utils;

namespace DenunciaFitosanitaria.Fragments
{
    /// <summary>
    /// Dynamic fragment subcategoria.
    /// </summary>
    public class DynamicFragmentSubcategoria : Fragment
    {
        SubCategoriaDetalle subCategoriaDetalle;
        FragmentManager SupportFragmentManager;
        ImageView imagenes_subcategoria;
        TextView subcategoriaDescripcionImg;
        int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Fragments.DynamicFragment"/> class.
        /// </summary>
        /// <param name="sfm">Sfm.</param>
        /// <param name="categorias">Categorias.</param>
        public DynamicFragmentSubcategoria(FragmentManager sfm,int position)
        {
            SupportFragmentManager = sfm;
            this.subCategoriaDetalle = DataManager.subcategoriaDetalle;
            this.position = position;
        }

        /// <summary>
        /// Ons the create.
        /// </summary>
        /// <param name="savedInstanceState">Saved instance state.</param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
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
            ViewGroup v = (ViewGroup)inflater.Inflate(Resource.Layout.DynamicFragmentSubcategory, container, false);
            imagenes_subcategoria = (ImageView)v.FindViewById(Resource.Id.imagenes_subcategoria);
            subcategoriaDescripcionImg = (TextView)v.FindViewById(Resource.Id.subcategoriaDescripcionImg);
            var imgEncr = string.Empty;
            var txtDcr = string.Empty;
            switch(position){
                case 1:
                    txtDcr = subCategoriaDetalle.Descripcion1;
                    imgEncr = subCategoriaDetalle.ImagenEncrypt1;
                    break;
                case 2:
                    txtDcr = subCategoriaDetalle.Descripcion2;
                    imgEncr = subCategoriaDetalle.ImagenEncrypt2;
                    break;
                case 3:
                    txtDcr = subCategoriaDetalle.Descripcion3;
                    imgEncr = subCategoriaDetalle.ImagenEncrypt3;
                    break;
                case 4:
                    txtDcr = subCategoriaDetalle.Descripcion4;
                    imgEncr = subCategoriaDetalle.ImagenEncrypt4;
                    break;
            }

            byte[] imageBytes = Convert.FromBase64String(imgEncr);
            imagenes_subcategoria.SetImageBitmap(BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length));
            subcategoriaDescripcionImg.Text = txtDcr;
            return v;
        }
    }
}