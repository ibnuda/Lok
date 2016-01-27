using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Android.Support.V4.Content;

namespace Lok
{
    public class LokTrackerAlarmReceiver: WakefulBroadcastReceiver
    {
        private static readonly string Tag = "LokTrackerAlarmReceiver";
        public override void OnReceive ( Context context, Intent intent )
        {
            context.StartService (new Intent (context, typeof (LocationService)));
        }
    }
}