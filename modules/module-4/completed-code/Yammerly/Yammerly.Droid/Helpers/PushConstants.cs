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
        public const string SenderId = "257308349779";
        public const string ListenConnectionString = "Endpoint=sb://azuretrainingrunthroughnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=Y1oas1E4aQb3jcmfhcvU1ftPEPdTeSugMRklI3jKvek=";
        public const string NotificationHubName = "azuretrainingrunthrough";
    }
}