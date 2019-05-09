using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using DenunciaFitosanitaria.Activities;

namespace DenunciaFitosanitaria.Utils
{
    /// <summary>
    /// Dialogs.
    /// </summary>
    public static class Dialogs
    {
        /// <summary>
        /// Dialog the specified context, tittle, message and positiveBtn.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="tittle">Tittle.</param>
        /// <param name="message">Message.</param>
        /// <param name="positiveBtn">Positive button.</param>
        public static void Dialog(Context context, string tittle, string message, string positiveBtn)
        {
            var dialog = new AlertDialog.Builder(context)
            .SetTitle(tittle)
            .SetMessage(message)
            .SetPositiveButton(positiveBtn, (senderAlert, args) =>
            {
            })
            .Show();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Activities.ActivitySplash"/> class.
        /// </summary>
        public static void ErrorService(View context,string message="Error al procesar solicitud",string button="ok")
        {
            Snackbar.Make(context, message, Snackbar.LengthIndefinite)
                        .SetAction(button, (view) => { })
                        .Show();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DenunciaFitosanitaria.Activities.ActivitySplash"/> class.
        /// </summary>
        public static void ErrorServiceFinish(View context, string message = "Error al procesar solicitud", string button = "ok")
        {
            Snackbar.Make(context, message, Snackbar.LengthIndefinite)
                        .SetAction(button, (view) => { })
                        .Show();
        }


        /// <summary>
        /// Errors the service finish.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="activitySplash">Activity splash.</param>
        /// <param name="message">Message.</param>
        /// <param name="button">Button.</param>
        public static void ErrorServiceFinish(View context, ActivitySplash activitySplash, string message = "Error al procesar solicitud", string button = "ok")
        {
            Snackbar.Make(context, message, Snackbar.LengthIndefinite)
                    .SetAction(button, (view) => { activitySplash.Finish(); })
                        .Show();
        }
    }
}
