
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Lok
{
	[Activity (Label = "LokTrackerBoot")]			
	public class LokTrackerBootReceiver : BroadcastReceiver
	{
		private static readonly string TAG = "LokTrackerBoot";
		public override void OnReceive(Context context, Intent intent)
		{
			var alarmManager = (AlarmManager)context.GetSystemService (Context.AlarmService);
			var intentTracker = new Intent (context, typeof(LokTrackerAlarmReceiver));
			var intentPending = PendingIntent.GetBroadcast (context, 0, intentTracker, 0);

			var prefs = context.GetSharedPreferences ("lok", 0);
			var intervalMinute = prefs.GetInt ("interval", 1);
			var currentlyTracking = prefs.GetBoolean ("currentlyTracking", false);

			if (currentlyTracking)
				alarmManager.SetRepeating (
					AlarmType.ElapsedRealtimeWakeup, 
					Android.OS.SystemClock.ElapsedRealtime (), 
					intervalMinute * 60000, 
					intentPending);
			else
				alarmManager.Cancel (intentPending);
		}
			
	}
}

