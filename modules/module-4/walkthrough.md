# Module 4: Add push with Azure Notification Hubs.

### Prerequisites
* Download the [starter code](http://www.google.com) for this module.

### Instructions
#### Google Cloud Messaging
**Objective**: Add push notifications to our Yammer clone.

1. Navigate to the [Google Cloud Console](https://console.cloud.google.com/), and sign in with your Google credentials. Click `Create New Project`.
 
 ![](/modules/module-3/images/create_new_project.png)

2. Name the project. Click `Create`.

 ![](/modules/module-3/images/name_project.png)

3. Copy your `Project Number` in the app's dashboard. This will be used later as the `SenderId`.
 
 ![](/modules/module-3/images/project_number.png)

4. Click `Enable and Manage API`. Select `Google Cloud Messaging`, then click `Enable`.

5. Navigate to `Crendentials`, click `Create Credentials->API key`. Select `Server key`, give your key a name, and then click `Create`. Copy your API key generated.

 ![](/modules/module-3/images/create_credentials.png)

#### Backend

1. Navigate to your resource group in the Azure Portal. Search for `Notification Hub`, select the row, and click `Create`.

2. Enter a name, namespace, and region for your Notification Hub to be hosted in, then select `Create`.

 ![](/modules/module-3/images/create_notification_hub.png)

3. Click `Settings`, `Notification Services`, `Google (GCM)`, and paste in the API key you copied earlier. Press the `Save` button.

 ![](/modules/module-3/images/notification_hub_configure.png)

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

2. Now that we have our required values for push notifications, let's add some code to handle them when they arrive.