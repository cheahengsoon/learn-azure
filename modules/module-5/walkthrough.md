# Module 5. Add blob storage with Azure Storage.

### Prerequisites
* Download the [starter code](http://www.google.com) for this module.

### Instructions
#### Backend
**Objective**: Add blob storage to our Yammerly clone.

1. Navigate to the Azure Portal and create a new `Storage Account`. Click `Create`.

  ![](/modules/module-5/images/create_new_storage_account.png)

2. Click `Blobs`, then add a new container named `images`. Set the `Access Type` to `Blob`. Click `Create`.

  ![](/modules/module-5/images/create_new_container.png)

3. In the `Settings` for your Azure Mobile App, click `Data Connections->Add`. Change the type to `Storage` and select your `Azure Storage` account.
 
  ![](/modules/module-5/images/add_data_connection.png)

4. Add the `WindowsAzure.Storage` NuGet to the `Yammerly.Service` project.

5. Add a new `Web API 2 Controller - Empty` and name it `StorageController`. Copy the following code into the class:

 ```csharp
 using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using Microsoft.Azure;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text;
using System.Diagnostics;
using Yammerly.Service.DataObjects;

namespace Yammerly.Service.Controllers
{
    [Authorize]
    [MobileAppController]
    public class StorageController : ApiController
    {
        public StorageController()
        {
            ConnectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_MS_AzureStorageAccountConnectionString", EnvironmentVariableTarget.Process);
            StorageAccount = CloudStorageAccount.Parse(ConnectionString);
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public string ConnectionString { get; set; }

        public CloudStorageAccount StorageAccount { get; set; }

        public CloudBlobClient BlobClient { get; set; }

        public async Task<StorageToken> Get()
        {
            var container = BlobClient.GetContainerReference("images");

            try
            {
                await container.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                Debug.WriteLine($"[GetStorageTokenController] Cannot create container: {ex.Message}");
            }

            // Create a blob URI - based on a GUID
            var blobName = $"{Guid.NewGuid().ToString("N")}.jpg";
            var blob = container.GetBlockBlobReference(blobName);

            // Create a policy for the blob access
            var blobPolicy = new SharedAccessBlobPolicy
            {
                // Set start time to five minutes before now to avoid clock skew.
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-5),
                // Allow Access for the next 60 minutes (according to Azure)
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(60),
                // Allow read, write and create permissions
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Create
            };

            return new StorageToken
            {
                Name = blobName,
                Uri = blob.Uri,
                SasToken = blob.GetSharedAccessSignature(blobPolicy)
            };

        }
    }
}
 ```

 6. Redeploy the backend.

#### Mobile Apps

1. Next, we need to call our `StorageController` from our mobile app to grab storage tokens, and finally store files off to Azure Storage. We can use this endpoint to store files.

 ```csharp
         public async Task<string> StoreBlob(Stream file)
        {
            string url;

            try
            {
                // Get the storage token from the custom API
                var storageToken = await MobileService.InvokeApiAsync<StorageToken>("Storage", HttpMethod.Get, null);
                var storageUri = new Uri($"{storageToken.Uri}{storageToken.SasToken}");

                var blob = new CloudBlockBlob(storageUri);
                await blob.UploadFromStreamAsync(file);

                url = blob.Uri.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"An error occurred breakage: {ex.Message}");

                throw ex;
            }

            return url;
        }
 ```

2. Let's add code to allow users to change their profile photo. Jump over to `ProfileViewModel` and add the following to your `ExecuteChangeProfilePhotoCommandAsync` method.

 ```csharp
                 await CrossMedia.Current.Initialize();

                MediaFile file;
                if (CrossMedia.Current.IsCameraAvailable)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Photos",
                        Name = "photo.jpg"
                    });
                }
                else
                {
                    file = await CrossMedia.Current.PickPhotoAsync();
                }

                var client = DependencyService.Get<IDataService>() as AzureService;
                var url = await client.StoreBlob(file.GetStream());
                Settings.PhotoUrl = url;
                PhotoUrl = url;

                var user = await client.GetItem<Employee>(Settings.UserId);
                user.PhotoUrl = url;
                await client.UpdateItem(user);
 ```

3. Let's add code to allow users to post to our corporate timeline. Open up `TimelineViewController` and add the following code to `ExecutePostTimelineItemCommandAsync`.

 ```csharp
                 await CrossMedia.Current.Initialize();

                MediaFile file;
                if (CrossMedia.Current.IsCameraAvailable)
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "TimelineItems",
                        Name = "photo.jpg"
                    });
                }
                else
                {
                    file = await CrossMedia.Current.PickPhotoAsync();
                }

                var client = DependencyService.Get<IDataService>() as AzureService;
                var author = await client.GetItem<Employee>(Settings.UserId);
                var url = await client.StoreBlob(file.GetStream());
                var text = "Having a blast at this Azure workshop!";

                var timelineItem = new TimelineItem
                {
                    Author = author,
                    Text = text,
                    PhotoUrl = url
                };

                TimelineItems.Add(timelineItem);
                await client.AddItem<TimelineItem>(timelineItem);
 ```

 4. Run the app, and post to the feed. We now have a working app with Azure Storage!