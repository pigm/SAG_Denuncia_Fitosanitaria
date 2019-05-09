using System.Linq;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Graphics;
using Android.Media;
using Android.Provider;
using Java.Lang;


namespace DenunciaFitosanitaria.Utils
{
    /// <summary>
    /// Bitmap utils.
    /// </summary>
    public static class BitmapUtils
    {

        /// <summary>
        /// Loads the and resize bitmap.
        /// </summary>
        /// <returns>The and resize bitmap.</returns>
        /// <param name="fileName">File name.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static Bitmap LoadAndResizeBitmap(this string fileName, int width, int height)
        {
            // First we get the the dimensions of the file on disk
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            BitmapFactory.DecodeFile(fileName, options);

            // Next we calculate the ratio that we need to resize the image by
            // to fit the requested dimensions.
            int outHeight = options.OutHeight;
            int outWidth = options.OutWidth;
            int inSampleSize = 1;

            if (outHeight > height || outWidth > width)
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }
            else
            {
                inSampleSize = outWidth > outHeight
                                   ? outHeight / height
                                   : outWidth / width;
            }

            // Now we will load the image and have BitmapFactory resize it for us.
            options.InSampleSize = inSampleSize;
            options.InJustDecodeBounds = false;
            Bitmap resizedBitmap = BitmapFactory.DecodeFile(fileName, options);

            return resizedBitmap;
        }

        /// <summary>
        /// Gets the bitmap.
        /// </summary>
        /// <returns>The bitmap.</returns>
        /// <param name="fileName">File name.</param>
        public static Bitmap GetBitmap(this string fileName)
        {
            BitmapFactory.Options options = new BitmapFactory.Options { InJustDecodeBounds = true };
            Bitmap readBitmap = BitmapFactory.DecodeFile(fileName, options);
            return readBitmap;
        }

        /// <summary>
        /// Gets the path to image.
        /// </summary>
        /// <returns>The path to image.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="activity">Activity.</param>
        public static string GetPathToImage(Android.Net.Uri uri, Activity activity)
        {
            string path = null;
            // The projection contains the columns we want to return in our query.
            string[] projection = new[] { Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data };
            using (ICursor cursor = activity.ContentResolver.Query(uri, projection, null, null, null))
            {
                if (cursor != null)
                {
                    int columnIndex = cursor.GetColumnIndexOrThrow(Android.Provider.MediaStore.Images.Media.InterfaceConsts.Data);
                    cursor.MoveToFirst();
                    path = cursor.GetString(columnIndex);
                }
            }

            return path;
        }


        /// <summary>
        /// Gets the browser hist.
        /// </summary>
        /// <param name="context">Context.</param>
        public static void getBrowserHist(Context context)
        {
            ICursor mCur = context.ContentResolver.Query(Browser.BookmarksUri,
                                                         Browser.HistoryProjection.ToArray(), null, null, null);
            mCur.MoveToFirst();
            if (mCur != null && mCur.MoveToFirst() && mCur.Count > 0)
            {
                while (mCur.IsAfterLast == false)
                {
                    mCur.MoveToNext();
                }
            }
        }

        /// <summary>
        /// Changes the orientation.
        /// </summary>
        /// <returns>The orientation.</returns>
        /// <param name="mediafile">Mediafile.</param>
        /// <param name="bitmap">Bitmap.</param>
        public static Bitmap changeOrientation(ExifInterface exifInterface, Bitmap bitmap)
        {
            int orientation = exifInterface.GetAttributeInt(ExifInterface.TagOrientation, 1);
            var matrix = new Matrix();
            switch (orientation)
            {
                case 2:
                    matrix.SetScale(-1, 1);
                    break;
                case 3:
                    matrix.SetRotate(180);
                    break;
                case 4:
                    matrix.SetRotate(180);
                    matrix.PostScale(-1, 1);
                    break;
                case 5:
                    matrix.SetRotate(90);
                    matrix.PostScale(-1, 1);
                    break;
                case 6:
                    matrix.SetRotate(90);
                    break;
                case 7:
                    matrix.SetRotate(-90);
                    matrix.PostScale(-1, 1);
                    break;
                case 8:
                    matrix.SetRotate(-90);
                    break;
                default:
                    return bitmap;
            }

            try
            {
                Bitmap oriented = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
                bitmap.Recycle();
                return oriented;
            }
            catch (OutOfMemoryError e)
            {
                e.PrintStackTrace();
                return bitmap;
            }
            catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return bitmap;
            }

        }
    }
}
