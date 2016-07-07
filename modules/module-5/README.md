# Module 5: Add blob storage with Azure Storage.
**Objective**: Add blob storage to our Yammerly clone to store unstructured data, such as large text and images. For Yammerly, we will add support for custom profile photos, as well as the ability to post photos to the company social timeline.

**Estimated Effort**: 45 minutes

###### Prerequisites
* Visual Studio 2015 Community Edition (or higher)
* Xamarin
* Azure subscription
* Postman
* Azure SDK for .NET
* Completion of [Module 2](/modules/module-2/), or [deployment of the Module 3 starter service] code(/modules/module-3/starter-code/).
* Download the [starter code](/modules/module-5/starter-code/) for this module.

### Instructions
##### Backend

1. Azure Storage is a service that allows us to store blobs, files, tables (NoSQL), and queue storage to our applications. Blob storage is especially relevant to mobile developers, as almost all applications need to store an image or some form of unstructured data. Navigate to the Azure Portal and create a new `Storage Account`. Click `Create`.

  ![](/modules/module-5/images/create_new_storage_account.png)

2. Click `Blobs`, then add a new container named `images`. You can think of containers as folders for storage. Just like folders, each container can have different permissions. In this case, set the `Access Type` to `Blob`. Click `Create`.

  ![](/modules/module-5/images/create_new_container.png)

3. In the `Settings` for your Azure Mobile App, click `Data Connections->Add`. Change the type to `Storage` and select your `Azure Storage` account. Azure Mobile Apps will automatically manage a connection string for us that we can access from our mobile backend.
 
  ![](/modules/module-5/images/add_data_connection.png)

4. Add the `WindowsAzure.Storage` NuGet to the `Yammerly.Service` project.

5. Access control is done using `Shared Access Signatures`. You can think of them as fancy API keys that also contain additional information, such as permissions. Although this can be generated ahead of time, it would be _highly_ unsecure to ship this with a mobile application, and always should be generated on a backend on demand. If a `Shared Access Signature` was to become compromised, the culprit (who would also have had to successfully authenticated against our Azure AD instance) would have a limited amount of time (in this case sixty minutes) before the signature expired. Add a new `Web API 2 Controller - Empty` and name it `StorageController`. Copy the following code into the class, which will generate a `Shared Access Signature` for a user to upload blobs to the `images` container on demand:

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

 6. Redeploy the backend to include our `StorageController` API.

#### Mobile Apps

1. Next, we need to call our `StorageController` from our mobile app to grab storage tokens, and finally store files off to Azure Storage. We can use the `InvokeApiAsync` method to call our `ApiController` and fetch the token. We can then upload directly to our blob. Add the following `StoreBlob` method to our `AzureService` class.

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

2. Now that all the groundwork is in place for cloud storage, let's add code to allow users to change their profile photo after logging in, instead of using the default. Jump over to `ProfileViewModel`, and add the following to your `ExecuteChangeProfilePhotoCommandAsync` method. We will use the `Media Plugin for Xamarin and Windows` to take a photo, then use our `AzureService` to upload it.

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

3. Let's add code to allow users to post to our corporate timeline, just like Facebook. Open up `TimelineViewController` and add the following code to `ExecutePostTimelineItemCommandAsync`.

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

 4. Run the app, and post to the feed. We now have a working Yammer clone written with Xamarin.Forms for iOS, Android, and Windows that uses a .NET backend, authenticates with Azure Active Directory, handles push, and allows us to push to a corporate timeline with Azure Storage! That's it for the required modules. If you learned something, please feel free to link back to this training online. I'll work on improving it and adding more content. :) Speaking of more content, check out our additional modules to see how you can make even cooler apps with Microsoft Azure and Xamarin.
