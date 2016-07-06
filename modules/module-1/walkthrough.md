# Module 1: Create a no-code backend with Azure Easy Tables.

### Prerequisites
* Download the [starter code](http://www.google.com) for this module.

### Instructions
#### Backend
**Objective**:

1. Head over to the Azure Portal at [portal.azure.com](portal.azure.com).
2. Create a new `Resource Group`.

 ![](/modules/module-1/images/create_resource_group.png)

3. Select a location and name for your `Resource Group`. Click `Create`. 
4. Now that we have a `Resource Group`, it's time to start adding items to it! Click the `Add` button and search the Azure Marketplace for `Mobile App`. Click the `Mobile App` cell, and click `Create` on the next blade.

 ![](/modules/module-1/images/create_new_mobile_app.png)

5. Enter a name for your `Mobile App`. This will serve as your mobile backend url, unless you configure a custom domain for it. Additionally, create a pricing plan or use one of the defaults. You will probably want to go `Standard (S1)` for all services. Click `Create` and a new mobile backend will be deployed. This may take anywhere from a minute to five minutes to deploy.

 ![](/modules/module-1/images/create_mobile_app.png)

6. After the Azure Mobile App deploys, click `Settings` and wait for the blade to fully load. Click the `Easy Tables` setting. There will be a banner prompting you to configure Easy Tables - click it. 

7. To use our no-code backend, we need to configure a place for the data to lie. Click the prompt, and then click `Add` on the `Data Connections` blade.

 ![](/modules/module-1/images/connect_database.png)

8. Configure a new `Data Connection`. The data connection may take anywhere from 2-5 minutes to deploy.

 ![](/modules/module-1/images/configure_data_connection.png)

9. After the data connection has been created, you should be able to create an Easy Tables backend by clicking `Initialize App`. Note that this _will overwrite_ all site contents, so don't configure Easy Tables on an existing app.

 ![](/modules/module-1/images/initialize_app.png)

10. Now that our no-code backend is created, it's time to add a table and some data. Easy Tables has an awesome feature in preview that allows you to create a table and populate it with existing data from a CSV file. This is great for situations with existing data. Click `Add from CSV` and upload the `employee_data.csv` file in the `/module-1/` directory. Be sure to change the table name to `Employee`, and click `Start Upload`.

 ![](/modules/module-1/images/populated_easy_table.png)

11. Once the data is uploaded, you should be able to click the `Employee` table and see populated data. From this dialog, you can alter schema, manage permissions, and delete data (very similar to how Azure Mobile Services) used to function.

### Mobile App
**Objective**: Communicate with an Azure Mobile Apps backend, including implementing online/offline synchronization and automatic conflict handling.

1. To pull down data from our Azure Mobile App backend, we need to use the [Microsoft.Azure.Mobile.Client SDK](), which has already been added as a NuGet reference for you. Open `Yammerly/Services/AzureService`. Let's add the code here to do online/offline sync and communicate with our backend.

2. `Initialize` will be used to configure the `MobileServiceClient`, which handles communication with our backend, as well as online/offline sync for us.

 ```csharp
        public async Task Initialize()
        {
            if (isInitialized)
                return;

            // MobileServiceClient handles communication with our backend, auth, and more for us.
            MobileService = new MobileServiceClient("https://azure-training.azurewebsites.net", null)
            {
                // Saves us from having to name things camel-case, or use custom JsonProperty attributes.
                SerializerSettings = new MobileServiceJsonSerializerSettings
                {
                    CamelCasePropertyNames = true
                }
            };

            // Configure online/offline sync.
            var store = new MobileServiceSQLiteStore("app.db");
            store.DefineTable<Employee>();
            await MobileService.SyncContext.InitializeAsync(store);

            isInitialized = true;
        }
 ```

3. Now that our `MobileServiceClient` is initialized, let's write some helper methods for handling CRUD operations, as well as syncing.

 ```csharp
        public async Task<IEnumerable<T>> GetItems<T>() where T : EntityData
        {
            await Initialize();

            await SyncItems<T>();
            return await MobileService.GetSyncTable<T>().ToEnumerableAsync();
        }

        public async Task<T> GetItem<T>(string id) where T : EntityData
        {
            await Initialize();

            await SyncItems<T>();

            return await MobileService.GetSyncTable<T>().LookupAsync(id);
        }

        public async Task AddItem<T>(T item) where T : EntityData
        {
            await Initialize();

            await MobileService.GetSyncTable<T>().InsertAsync(item);
            await SyncItems<T>();
        }

        public async Task UpdateItem<T>(T item) where T : EntityData
        {
            await Initialize();

            await MobileService.GetSyncTable<T>().UpdateAsync(item);
            await SyncItems<T>();
        }

        public async Task RemoveItem<T>(T item) where T : EntityData
        {
            await Initialize();

            await MobileService.GetSyncTable<T>().DeleteAsync(item);
            await SyncItems<T>();
        }

        public async Task SyncItems<T>() where T : EntityData
        {
            await Initialize();

            try
            {
                await MobileService.SyncContext.PushAsync();
                await MobileService.GetSyncTable<T>().PullAsync($"all{typeof(T).Name}", MobileService.GetSyncTable<T>().CreateQuery());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during Sync occurred: {ex.Message}");
            }
        }
 ```

4. We want to handle sync conflicts gracefully. By default, "last write wins" is the conflict resolutions strategy. If you look at our `EntityData` model, you will notice there is a `Version` property; this is updated everytime there is a POST, PATCH, or DELETE operation performed on the server. If we have an issue now, the server will return a `412 Precondition Failed` response. We have several choices when it comes to conflict resolution: "client wins", "server wins", merge results, or let the the user select. In this case, we will elect to let the user decide, though it depends on the particular situation. Let's update our `AzureService` to handle this. Add a new class named `SyncHandler` to the `Helpers` folder (code below), which will prompt the user to select which version is correct in the event of a conflict.

 ```csharp
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yammerly.Helpers
{
    public class SyncHandler : IMobileServiceSyncHandler
    {
        MobileServiceClient client;

        public SyncHandler(MobileServiceClient client)
        {
            this.client = client;
        }

        public async Task<JObject> ExecuteTableOperationAsync(IMobileServiceTableOperation operation)
        {
            MobileServicePreconditionFailedException ex;
            JObject result = null;

            do
            {
                ex = null;
                try
                {
                    result = await operation.ExecuteAsync();
                }
                catch (MobileServicePreconditionFailedException e)
                {
                    ex = e;
                }

                // There was a conflict in the server
                if (ex != null)
                {
                    // Grabs the server item from the exception. If not available, fetch it.
                    var serverItem = ex.Value;
                    if (serverItem == null)
                    {
                        serverItem = (JObject)(await operation.Table.LookupAsync((string)operation.Item["id"]));
                    }

                    // Prompt user action
                    var userAction = await App.Current.MainPage.DisplayAlert("Conflict Occurred", "Select which version to keep.", "Server", "Client");

                    if (userAction)
                    {
                        return serverItem;
                    }
                    else
                    {
                        // Overwrite the server version and try the operation again by continuing the loop.
                        operation.Item[MobileServiceSystemColumns.Version] = serverItem[MobileServiceSystemColumns.Version];
                    }
                }
            } while (ex != null);

            return result;
        }
        
        public Task OnPushCompleteAsync(MobileServicePushCompletionResult result)
        {
            return Task.FromResult(0);
        }
    }
}
 ```

5. Update `AzureService`'s `SyncContext` to use our `SyncHandler` for processing sync.

 ```csharp
 await MobileService.SyncContext.InitializeAsync(store, new SyncHandler(MobileService));
 ```
6. Update `App.xaml.cs`to use our `AzureService` instead of a `MockService`.

 ```csharp
 PresentMainPage(useMock: false);
 ```

7. Run the app. We are now pulling down data from our no-code Azure Easy Tables backend in just a few lines of (mostly) boilerplate code. If you turn off the internet on your machine and restart the app, you will notice that the app continues to run and load data properly.

Easy Tables are a great way to get started with building an Azure backend. For many apps, Easy Tables works great, but it has one key limitation: relationships should not be done using Easy Tables. For that, we need to opt for using the "full-fledged" Azure Mobile Apps. In the next module, we will take a look at building a fully-customizable backend with ASP.NET Web API.