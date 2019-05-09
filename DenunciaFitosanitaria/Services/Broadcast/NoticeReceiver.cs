using System.Linq;
using Android.App;
using Android.Content;

namespace DenunciaFitosanitaria.Services.Broadcast
{
    [BroadcastReceiver]
    public class NoticeReceiver : BroadcastReceiver
    {
        /// <summary>
        /// Ons the receive.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <param name="intent">Intent.</param>
        public override async void OnReceive(Context context, Intent intent)
        {
            ActivityManager am = (ActivityManager)context.GetSystemService(Context.ActivityService);
            ComponentName cn = am.GetRunningTasks(1).ElementAt(0).TopActivity;
            if(cn.ToString().Contains("MainActivity")){
                await ServiceHelper.ProcesarDenuncias();
            }
        }
    }
}
