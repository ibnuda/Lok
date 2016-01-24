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

using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Locations;
using Android.Util;
using Java.Text;
using Java.Util;
using ILocationListener = Android.Gms.Location.ILocationListener;

// using Android.Gms.Location;

namespace Lok
{
    public class LocationService : Service, GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener, ILocationListener
    {
        private static readonly string TAG = "LocationService";

        private string _defaultUploadWebsite;
        private bool _currentlyProcessLocation = false;
        private LocationRequest _locationRequest;
        private GoogleApiClient _googleApiClient;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public void OnConnected(Bundle connectionHint)
        {
            Log.Debug(TAG, "OnConnected");

            _locationRequest = LocationRequest.Create();
            _locationRequest.SetInterval(1000);
            _locationRequest.SetFastestInterval(1000);
            _locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, _locationRequest, this);
        }

        public void OnConnectionSuspended(int cause)
        {
            Log.Debug(TAG, "OnConnectionSuspended");
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Log.Error(TAG, "OnConnectionFailed");
            StopLocationUpdates();
            StopSelf();
        }

        private void StopLocationUpdates()
        {
            if (_googleApiClient != null && _googleApiClient.IsConnected)
                _googleApiClient.Disconnect();
        }

        public void OnLocationChanged(Location location)
        {
            if (location != null)
            {
                Log.Error(TAG, "Posisi :" + location.Altitude + ", " + location.Longitude + ", akurasi : " + location.Accuracy);

                if (location.Accuracy < 500.0f)
                {
                    StopLocationUpdates();
                }
            }
        }

        protected void SendLocationDataToWebsite(Location location)
        {
            var dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            var date = new Date(location.Time);
            var prefs = this.GetSharedPreferences("Lok", 0);
        }
    }
}