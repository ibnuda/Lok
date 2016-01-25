using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Location;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Locations;
using Android.Util;
using Java.Text;
using Java.Util;
using ILocationListener = Android.Gms.Location.ILocationListener;

using ModernHttpClient;

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
            if (location == null) return;
            Log.Error(TAG, "Posisi :" + location.Altitude + ", " + location.Longitude + ", akurasi : " + location.Accuracy);

            if (!(location.Accuracy < 500.0f)) return;
            StopLocationUpdates();
            SendLocationDataToWebsite(location);
        }

        protected async void SendLocationDataToWebsite(Location location)
        {
            var dateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            var date = new Date(location.Time);
            var prefs = this.GetSharedPreferences("lok", 0);
            var editor = prefs.Edit();
            var totalDistance = prefs.GetFloat("totalDistance", 0f);
            var firstTimePosition = prefs.GetBoolean("firstTimePosition", true);
            var distance = 0;
            if (firstTimePosition)
                editor.PutBoolean("firstTimePosition", false);
            else
            {
                var prevLocation = new Location("")
                {
                    Latitude = prefs.GetFloat("prevLat", 0f),
                    Longitude = prefs.GetFloat("prevLong", 0f)
                };

                distance = (int) location.DistanceTo(prevLocation);
                totalDistance += distance;
                editor.PutFloat("totalDistance", totalDistance);
            }
            editor.PutFloat("prevLat", (float) location.Latitude);
            editor.PutFloat("prevLong", (float) location.Longitude);
            editor.Apply();

            // TO DO : add parameters for post requests.
            // Hint : using HttpClient();
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string,string>()
                {
                    { "latitude", location.Latitude.ToString() },
                    { "longitude", location.Longitude.ToString() },
                    { "speed", location.Speed.ToString() },
                    { "date", System.Uri.EscapeDataString(DateTime.Now.ToString()) },
                    { "locationmethod", location.Provider },
                    { "distance", distance.ToString() },
                    { "username", prefs.GetString("username", "") },
                    { "sessionid", prefs.GetString("sessionId", "") },
                    { "accuracy", location.Accuracy.ToString() }
                };
                var content = new FormUrlEncodedContent(values);
                await client.PostAsync(_defaultUploadWebsite, content);
            }
        }
    }
}