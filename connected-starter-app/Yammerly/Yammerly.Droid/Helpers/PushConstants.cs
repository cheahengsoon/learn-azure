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

namespace Yammerly.Droid.Helpers
{
    public class PushConstants
    {
        public const string SenderId = "yammerly-production";
        public const string ListenConnectionString = "Endpoint=sb://yammerlyproduction.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=5sUmGqCzxJy6dTDwxds2qmbcuPkeYDPK8U9fR1WGtOc=";
        public const string NotificationHubName = "yammerlyproduction";
    }
}