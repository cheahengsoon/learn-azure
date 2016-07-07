# Module 4: Add push notifications with Azure Notification Hubs.
**Objective**: Add push notifications to our Android Yammerly application using Azure Notification Hubs, and learn how to send both broadcast and templated push notifications.

**Estimated Effort**: 40 minutes

###### Prerequisites
* Visual Studio 2015 Community Edition (or higher)
* Xamarin
* Azure subscription
* Google account
* Postman
* Azure SDK for .NET
* Completion of [Module 3](/modules/module-3/).
* Download the [starter code](/modules/module-4/starter-code/) for this module.

### Instructions
##### Google Cloud Messenging Service
Push notifications are a tricky subject. Every platform implements push differently, and offers different services for doing so. Luckily, Azure Notification Hubs makes it super easy to send push notifications to iOS, Android, Windows, and even Blackberry devices. We will start by completing the Android configuration, then configure our backend and mobile app to send and receive push notifications.

1. Navigate to the [Google Cloud Console](https://console.cloud.google.com/), and sign in with your Google credentials. Click `Create New Project`.
 
 ![](/modules/module-4/images/create_new_project.png)

2. Name the project. Click `Create`.

 ![](/modules/module-4/images/name_project.png)

3. Copy your `Project Number` in the app's dashboard. This will be used later as the `SenderId`.
 
 ![](/modules/module-4/images/project_number.png)

4. Click `Enable and Manage API`. Select `Google Cloud Messaging`, then click `Enable`.

5. Navigate to `Crendentials`, click `Create Credentials->API key`. Select `Server key`, give your key a name, and then click `Create`. Copy your API key generated.

 ![](/modules/module-4/images/create_credentials.png)

#### Backend

1. Navigate to your resource group in the Azure Portal. Search for `Notification Hub`, select the row, and click `Create`. Notification Hubs manage the complex relationships between all the different push services and platforms, and give us an easy-to-use API and portal for adding push to our applications.

2. Enter a name, namespace, and region for your Notification Hub to be hosted in, then select `Create`.

 ![](/modules/module-4/images/create_notification_hub.png)

3. Click `Settings`, `Notification Services`, `Google (GCM)`, and paste in the API key you copied earlier. Press the `Save` button.

 ![](/modules/module-4/images/notification_hub_configure.png)

Backend configuration is now complete for the Notification Hub. It's time to consume our notification hub from the Yammerly Android app!

#### Mobile Apps

1. Open up the Yammerly.Droid solution, and a new class named `PushConstants` to the `Helpers` directory. Add three fields: `SenderId`, `ListenConnectionString`, and `NotificationHubName`. `SenderId` is the Google project number you saved earlier. `ListenConnectionString` can be found in the `Access Policies` section of your notification hub's settings.`NotificationHubName` is just your notification hub name.

 ```csharp
    public class PushConstants
    {
        public const string SenderId = "257308349779";
        public const string ListenConnectionString = "Endpoint=sb://azuretrainingrunthroughnamespace.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=Y1oas1E4aQb3jcmfhcvU1ftPEPdTeSugMRklI3jKvek=";
        public const string NotificationHubName = "azuretrainingrunthrough";
    }
 ```

2. Now that we have our required values for push notifications, let's add some code to handle them when they arrive. Add a new class to the `Helpers` directory named `PushService` that will register the device to receive push notifications, and handle incoming push notifications. The majority of this is boilerplate code that can be added every time and doesn't need to be rewritten. You can use `OnRegistered` to register the device for additional tags (for targeted push), and `OnMessage` to define what happens when you receive a push notification (such as deep link into a Xamarin.Forms page).

 ```csharp
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
```

3. Add a new method to `MainActivity` named `ConfigurePushNotifications` and call it from `OnCreate`. This will handle initialization of the push service as well as register the device to receive push notifications from Google Cloud Messaging/Notification Hubs.

 ```csharp
        public void ConfigurePushNotifications()
        {
            PushService.Initialize(this);
            PushService.Register(this);
        }
 ```

 4. Run your app in an emulator that supports the Google APIs. Your device will be registered with Google Cloud Messaging and the Azure Notification Hub.

 5. Visit the Azure Notification Hub portal and click `Test Send`. Select the `Android` platform, then press `Send`. You should notice the notification appear on the device.

  ![](/modules/module-4/images/notification_hub_configure.png)

In this module, we added push notifications to our Yammerly Android app in just a few minutes with Azure Notification Hubs. In the next module, we wrap up our Yammer clone by [adding blob storage with Azure Storage](/modules/module-5).
