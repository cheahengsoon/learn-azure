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
using Gcm;
using WindowsAzure.Messaging;

namespace Yammerly.Droid.Helpers
{
    [BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })] // Allow GCM on boot and when app is closed   
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
        Categories = new string[] { "@PACKAGE_NAME@" })]
    public class SampleGcmBroadcastReceiver : GcmBroadcastReceiverBase<PushService>
    {
        //IMPORTANT: Change this to your own Sender ID!
        //The SENDER_ID is your Google API Console App Project Number
        public static string[] SENDER_IDS = { PushConstants.SenderId };
    }

    [Service] //Must use the service tag
    public class PushService : GcmServiceBase
    {
        static NotificationHub hub;

        // Call from MainActivity
        public static void Initialize(Context context)
        {
            hub = new NotificationHub(PushConstants.NotificationHubName, PushConstants.ListenConnectionString, context);
        }

        public static void Register(Context Context)
        {
            GcmClient.Register(Context, SampleGcmBroadcastReceiver.SENDER_IDS);
        }

        public PushService() : base(SampleGcmBroadcastReceiver.SENDER_IDS)
        {
        }

        protected override void OnRegistered(Context context, string registrationId)
        {
            if (hub != null)
                hub.Register(registrationId, "TEST");
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            if (hub != null)
                hub.Unregister();
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            Console.WriteLine("Received Notification");

            //Push Notification arrived - print out the keys/values
            if (intent != null || intent.Extras != null)
            {

                var keyset = intent.Extras.KeySet();

                foreach (var key in intent.Extras.KeySet())
                    Console.WriteLine("Key: {0}, Value: {1}", key, intent.Extras.GetString(key));
            }
        }

        protected override bool OnRecoverableError(Context context, string errorId)
        {
            //Some recoverable error happened
            return true;
        }

        protected override void OnError(Context context, string errorId)
        {
            //Some more serious error happened
        }
    }
}