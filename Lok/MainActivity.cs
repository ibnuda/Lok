﻿using System;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;

using Android.Support.V7.App;
using Android.Views.InputMethods;
using Java.Lang;
using Java.Security;
using Java.Util;
using static Android.Gms.Common.GooglePlayServicesUtil;
using static Android.Gms.Common.ConnectionResult;

namespace Lok
{
    [Activity (Label = "Lok", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity: Activity
    {
        private static readonly string Tag = "LokActivity";

        private string _defaultUploadWebsite;
        private static EditText _usernameEditText;
        private static EditText _websiteEditText;
        private static Button _trackingButton;

        private bool _currentlyTracking;
        private RadioGroup _intervalRadioGroup;
        private int _intervalMinute = 1;
        private AlarmManager _alarmManager;
        private Intent _gpsIntent;
        private PendingIntent _pendingIntent;


        protected override void OnCreate ( Bundle bundle )
        {
            base.OnCreate (bundle);
            SetContentView (Resource.Layout.Main);

            _usernameEditText = FindViewById<EditText> (Resource.Id.editTextUsername);
            _websiteEditText = FindViewById<EditText> (Resource.Id.editTextUploadWebsite);
            _intervalRadioGroup = FindViewById<RadioGroup> (Resource.Id.radioGroup1);
            _trackingButton = FindViewById<Button> (Resource.Id.buttonTracking);

            var radioButton1Menit = FindViewById<RadioButton> (Resource.Id.radioButton1Minute);
            var radioButton3Menit = FindViewById<RadioButton> (Resource.Id.radioButton3Minute);
            var radioButton5Menit = FindViewById<RadioButton> (Resource.Id.radioButton5Minute);

            var prefs = GetSharedPreferences ("lok", 0);
            _currentlyTracking = prefs.GetBoolean ("currentlyTracking", false);
            var firstTime = prefs.GetBoolean ("firstTime", true);
            if (firstTime)
            {
                var editor = prefs.Edit ();
                editor.PutBoolean ("firstTime", false);
                editor.PutString ("appId", UUID.RandomUUID ().ToString ());
                editor.Apply ();
            }
            // _intervalRadioGroup.CheckedChange += delegate { };

            // Untuk event onchange pada radiobutton interval.
            radioButton1Menit.Click += SaveIntervalRadioButtonClick;
            radioButton3Menit.Click += SaveIntervalRadioButtonClick;
            radioButton5Menit.Click += SaveIntervalRadioButtonClick;

            // _trackingButton.Click += SaveuserSettings();
            _trackingButton.Click += ( sender, e ) => { TrackLocation (); };
        }

        private void SaveIntervalRadioButtonClick ( object sender, EventArgs e )
        {
            var radioButton = (RadioButton) sender;
            if (_currentlyTracking)
                Toast.MakeText (ApplicationContext, Resource.String.Hello, ToastLength.Long).Show ();
            var prefs = GetSharedPreferences ("lok", 0);
            var editor = prefs.Edit ();

            if (radioButton.Id == Resource.Id.radioButton1Minute)
                editor.PutInt ("interval", 1);
            else if (radioButton.Id == Resource.Id.radioButton3Minute)
                editor.PutInt ("interval", 3);
            else if (radioButton.Id == Resource.Id.radioButton5Minute)
                editor.PutInt ("interval", 5);
            editor.Apply ();
        }

        private static bool KosongAtauPakaiSpasi ()
        {
            var tempUsername = _usernameEditText.Text.Trim ();
            var tempUploadSite = _websiteEditText.Text.Trim ();
            return tempUploadSite.Length == 0 || tempUsername.Length == 0
                || PakaiSpasi (tempUsername) || PakaiSpasi (tempUploadSite);
        }

        private static bool PakaiSpasi ( string s )
        {
            return s.Split (' ').Length > 1;
        }

        private bool CheckGooglePlayEnable ()
        {
            if (IsGooglePlayServicesAvailable (this) == Success)
                return true;
            else
            {
                Toast.MakeText (ApplicationContext, Resource.String.GooglePlayUnavailable, ToastLength.Long).Show ();
                return false;
            }
        }

        private void DisplayUserSettings ()
        {
            var prefs = this.GetSharedPreferences ("lok", 0);
            _intervalMinute = prefs.GetInt ("interval", 1);

            if (_intervalMinute == 1)
                _intervalRadioGroup.Check (Resource.Id.radioButton1Minute);
            else if (_intervalMinute == 3)
                _intervalRadioGroup.Check (Resource.Id.radioButton3Minute);
            else if (_intervalMinute == 5)
                _intervalRadioGroup.Check (Resource.Id.radioButton5Minute);

            _websiteEditText.Text = prefs.GetString ("uploadSite", _defaultUploadWebsite);
            _usernameEditText.Text = prefs.GetString ("username", "");
        }

        private bool SaveUserSettings ()
        {
            if (KosongAtauPakaiSpasi ())
                return false;

            var prefs = this.GetSharedPreferences ("lok", 0);
            var editor = prefs.Edit ();

            if (_intervalRadioGroup.CheckedRadioButtonId == Resource.Id.radioButton1Minute)
                editor.PutInt ("interval", 1);
            else if (_intervalRadioGroup.CheckedRadioButtonId == Resource.Id.radioButton3Minute)
                editor.PutInt ("interval", 3);
            else if (_intervalRadioGroup.CheckedRadioButtonId == Resource.Id.radioButton5Minute)
                editor.PutInt ("interval", 5);

            editor.PutString ("username", _usernameEditText.Text.Trim ());
            editor.PutString ("uploadSite", _websiteEditText.Text.Trim ());

            return true;
        }

        private void StartAlarmManager ()
        {
            Log.Debug (Tag, "StartAlarmManager");

            var context = BaseContext;
            _alarmManager = (AlarmManager) context.GetSystemService (Context.AlarmService);
            _gpsIntent = new Intent (context, typeof (LokTrackerAlarmReceiver));
            _pendingIntent = PendingIntent.GetBroadcast (context, 0, _gpsIntent, 0);

            var prefs = this.GetSharedPreferences ("lok", 0);
            _intervalMinute = prefs.GetInt ("intervalMenit", 1);
            _alarmManager.SetRepeating (AlarmType.ElapsedRealtimeWakeup,
                Android.OS.SystemClock.ElapsedRealtime (),
                _intervalMinute * 60000,
                _pendingIntent);
        }

        private void CancelAlarmManager ()
        {
            Log.Debug (Tag, "StopAlarmManager");
            var context = BaseContext;
            _gpsIntent = new Intent (context, typeof (LokTrackerAlarmReceiver));
            _pendingIntent = PendingIntent.GetBroadcast (context, 0, _gpsIntent, 0);
            _alarmManager = (AlarmManager) context.GetSystemService (Context.AlarmService);
            _alarmManager.Cancel (_pendingIntent);
        }

        private void TrackLocation ()
        {
            var prefs = this.GetSharedPreferences ("lok", 0);
            var editor = prefs.Edit ();

            if (!SaveUserSettings ())
                return;
            if (!CheckGooglePlayEnable ())
                return;

            if (_currentlyTracking)
            {
                CancelAlarmManager ();
                _currentlyTracking = false;
                editor.PutBoolean ("currentlyTracking", false);
                editor.PutString ("sessionId", "");
            }
            else
            {
                StartAlarmManager ();
                editor.PutBoolean ("currentlyTracking", true);
                editor.PutFloat ("totalDistance", 0f);
                editor.PutBoolean ("firstTimePosition", false);
                editor.PutString ("sessionId", UUID.RandomUUID ().ToString ());
            }
            editor.Apply ();
            SetTrackingButton ();
        }

        private void SetTrackingButton ()
        {
            if (_currentlyTracking)
            {
                _trackingButton.SetTextColor (Android.Graphics.Color.Pink);
                _trackingButton.SetText (Resource.String.TrackingOn);
            }
            else
            {
                _trackingButton.SetTextColor (Android.Graphics.Color.Azure);
                _trackingButton.SetText (Resource.String.TrackingOff);
            }
        }

        protected override void OnResume ()
        {
            Log.Debug (Tag, "OnResume");
            base.OnResume ();
            DisplayUserSettings ();
            SetTrackingButton ();
        }

        protected override void OnStop ()
        {
            base.OnStop ();
        }
    }
}

