# Module 2: Create an ASP.NET Web API mobile app backend.
**Objective**: Use ASP.NET Web API to build a backend for our mobile apps. Learn to debug and deploy Azure Mobile Apps backends.

to quickly create a no-code backend for our mobile app. Pull down data from our backend in the Yammerly mobile apps, and even configure online/offline synchronization, as well as sync conflict handling.
**Estimated Effort**: 30 minutes

###### Prerequisites
* Visual Studio 2015 Community Edition (or higher)
* Xamarin
* Azure subscription
* Postman
* Azure SDK for .NET
* Download the [starter code](/modules/module-2/starter-code/) for this module.

### Instructions
##### Backend

1. Just like any app, we can get started building Azure Mobile Apps backends with Visual Studio. Right-click the `Yammerly` solution, and select `Add->New Project`. Navigate to `Visual C#`-> `Cloud` -> `Azure Mobile App`. Name it `Yammerly.Service` and click `OK`. If you do not see this template, ensure that you have installed the Azure SDK for .NET.

  ![](/modules/module-2/images/new_mobile_app_backend.png)

2. You will be prompted to configure some settings for the solution. Uncheck `Host in the cloud`, and click `OK`.

  ![](/modules/module-2/images/project_options.png)
 
3. Let's bring over our data objects from the mobile solution to our backend. Right-click the `Data Objects` folder and click `Add->Existing Item`. Select the `Employee`, `TimelineItem`, and `StorageToken` objects, and click `Add`. Typically, you may use a `Shared Project` or other code sharing strategy to share models between your frontend and backend, as done in the [Xamarin Evolve conference app](https://github.com/xamarinhq/app-evolve). 

4. In each data model we just added, update the namespace to `Yammerly.Service.DataObjects`. The `Microsoft.Azure.Mobile.Server` SDK comes with an `EntityData` namespace we can use, so we need to bring that namespace in as well. Additionally, remove the `Name` field from the `Employee` data model.

5. Because we have nested object navigation (the `Author` property of `TimelineItem`), we need to define this property as `virtual`, which will create a JOIN for us on the table and manage that relationship for us. In SQL speak, this essentially creates a backing field behind the scenes named `AuthorId` and will fetch that object for us when we request it, without having to make a separate request. `EntityFramework` handles all of this for us, so we never have to think about it.

 ```csharp
     public class TimelineItem : EntityData
    {
        public virtual Employee Author { get; set; }
        public string Text { get; set; }
        public string PhotoUrl { get; set; }
    }
 ```

5. Time to add some `TableController`s! These will serve as the RESTful endpoints that our mobile app hits in order to receive data or other information from the backend. Right-click the `Controllers` folder and select `Add->Controller`. Select the `Azure Mobile Apps Table Controller`, as seen below, then click `Add`.

  ![](/modules/module-2/images/scaffold_controller.png)

6. Set the `Model Class` to `Employee` and the `Data context class` to `MobileServiceContext`. Click `Add`. This will "scaffold" out a new `TableController` for us and configure some additional settings. Repeat this process for `TimelineItem`.

  ![](/modules/module-2/images/add_controller.png)

7. Next, we want to enable "soft delete" for our `Employee` and `TimelineItem` tables. Soft delete is an awesome feature that allows a user to "delete" data without actually deleting it. This is especially useful in contexts where the user may want to "undo" a deletion. When we query data from our backend, the `MobileServiceClient` will not return any of our soft deletions, unless explictly prompted to do so.  To enable soft delete, open each controller and set the `enableSoftDelete` parameter of `EntityDomainManager` to `true`.

 ```csharp
DomainManager = new EntityDomainManager<TimelineItem>(context, Request, enableSoftDelete: true);
 ```
 
8. Now that our API is complete, it's time to seed our database with some initial data. If you want initial data in your database, this is how you go about doing it. Open `App_Start/Startup.MobileApp`. Scroll down to the `MobileServiceInitializer`. This is where we can "seed" some initial data into our database. Because this is using EntityFramework behind the scenes, we need to be cognizant of how we are adding data, especially relationally. Copy and paste the following into the `Seed` method:

 ```csharp
         protected override void Seed(MobileServiceContext context)
        {
            var employees = new List<Employee>
            {
                new Employee { Id = Guid.NewGuid().ToString(), FirstName = "Nat", LastName = "Friedman", Title = "CEO", PhotoUrl = "http://static4.businessinsider.com/image/559d359decad04574c42a3c4-480/xamarin-nat-friedman.jpg" },
                new Employee { Id = Guid.NewGuid().ToString(), FirstName = "Miguel", LastName = "de Icaza", Title = "CTO", PhotoUrl = "http://images.techhive.com/images/idge/imported/article/nww/2011/03/031111-deicaza-100272676-orig.jpg" },
                new Employee { Id = Guid.NewGuid().ToString(), FirstName = "Joseph", LastName = "Hill", Title = "VP of Developer Relations", PhotoUrl = "https://www.gravatar.com/avatar/f763ec6935726b7f7715808828e52223.jpg?s=256" },
                new Employee { Id = Guid.NewGuid().ToString(), FirstName = "James", LastName = "Montemagno", Title = "Developer Evangelist", PhotoUrl = "http://www.gravatar.com/avatar/7d1f32b86a6076963e7beab73dddf7ca?s=256" },
                new Employee { Id = Guid.NewGuid().ToString(), FirstName = "Pierce", LastName = "Boggan", Title = "Software Engineer", PhotoUrl = "https://avatars3.githubusercontent.com/u/1091304?v=3&s=460" },
            };

            var timelineItems = new List<TimelineItem>
            {
                new TimelineItem { Id = Guid.NewGuid().ToString(), Author = employees[0], Text = "Had a great time talking about the Mobile DevOps lifecycle today!",PhotoUrl="https://lh3.googleusercontent.com/proxy/cvqTGBR-t3YY9dNwF0IN00xs_B7WDGF2h-klJThcBL40sdoI_piW8c33SsU3J3uGbM9go0_5ZWjAtr_MKqJFHRp6i8HdSWMBN_Iud9DtGw7EblrTSXiVgk06K2YpVjW8C95yptzDI3zwEPnXGw=w506-h284" },
                new TimelineItem { Id = Guid.NewGuid().ToString(), Author = employees[2], Text = "First full day of the conference and it's packed! Over 2,000 people here.", PhotoUrl = "http://img.youtube.com/vi/jgXCB51e4ak/hqdefault.jpg" },
                new TimelineItem { Id = Guid.NewGuid().ToString(), Author = employees[3], Text = "Learning all about building connected apps with Azure thanks to Mike James!", PhotoUrl = "https://sec.ch9.ms/ch9/2d33/ee514b78-c024-49bf-8341-87fd2a492d33/XE16AzureXamarin_Custom.jpg" },
                new TimelineItem { Id = Guid.NewGuid().ToString(), Author = employees[4], Text = "Thought I showed up to see Miguel de Icaza?", PhotoUrl = "https://pbs.twimg.com/media/ChKQOQRWgAEB7hu.jpg" },
                new TimelineItem { Id = Guid.NewGuid().ToString(), Author = employees[1], Text= "We finally did it! .NET native, open source and on all platforms.", PhotoUrl = "https://encrypted-tbn3.gstatic.com/images?q=tbn:ANd9GcTgTyHJtGi8odJpb1chJC2e8oCKCBevfft-PJueaCcO4V7O3DFv" }
            };

            employees.ForEach(employee => context.Employees.Add(employee));
            timelineItems.ForEach(timelineItem =>
            {
                var employee = context.Employees.Find(timelineItem.Author.Id);
                timelineItem.Author = employee;
                context.TimelineItems.Add(timelineItem);
            });

            try
            {
                context.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Trace.TraceInformation(
                              "Class: {0}, Property: {1}, Error: {2}",
                              validationErrors.Entry.Entity.GetType().FullName,
                              validationError.PropertyName,
                              validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message + "\n" + ex.InnerException.Message + "\n" + ex.StackTrace);
            }

            base.Seed(context);
        }
 ```

9. That's it! Now that we have basic data access down, let's deploy our solution. We want to use best practices though, so we don't want to ship to production right away. Azure Mobile Apps has a great feature called deployment slots that allow us to test our backend, without pushing to production. Navigate to your Azure Mobile App in the Azure Portal. Click `Settings`, then `Deployment Slots`.

  ![](/modules/module-2/images/new_deployment_slot.png)

10. Click `Add Slot`, name the deployment slot, and click `OK`.

  ![](/modules/module-2/images/add_deployment_slot.png)
 
11. In Visual Studio, right-click our Web API solution and click `Publish`. In the `Select a publish target` section, select `Microsoft Azure App Service`.

  ![](/modules/module-2/images/publish_web.png)
  
12. You may have to log into the account you are using for Azure. Expand down your resource groups until you get to the Azure Mobile App you created. Expand the `Deployment Slots` folder and select the deployment slot you created. Click `OK`, followed by `Publish`. A build will begin, and your site will be deployed.

  ![](/modules/module-2/images/publish_pane.png)

13. Open Postman. Postman is an awesome app that we can use to debug our API. Form a request for `http://{YOUR-APP-SERVICE-NAME}.azurewebsites.net/tables/Employee?ZUMO-API-VERSION=2.0.0` and click `Send`. The first request will take a considerable amount of time, as a new database is being created and populated with our seed data. After the request returns, you should see a JSON response with employee data.

  ![](/modules/module-2/images/postman.png)

14. If we try another request for `http://{YOUR-APP-SERVICE-NAME}.azurewebsites.net/tables/TimelineItem?ZUMO-API-VERSION=2.0.0`, we should see another 200 OK response, along with JSON data. But it looks like there is an issue - our `Author` property is not returning. This is because data pertaining to navigation properties is not automatically "expanded" for us. Imagine we have a mobile app with dozens of navigation properties, of which have navigation properties, and we request fifty records. We would be pulling down tons of data, which would make our app seem slow. In this case, we can use lazy loading to fetch these requests on demand, or selectively expand objects. In this case, let's just automatically expand all navigation properties. We can adjust our request to include `$expand=Author` (`http://azure-training-development.azurewebsites.net/tables/TimelineItem?ZUMO-API-VERSION=2.0.0&$expand=Author`). Try the request again in Postman with the altered query, and you should see it working:

  ![](/modules/module-2/images/postman_expansion.png)

15. `MobileServiceClient` doesn't know we should do this expansion, so we need to handle this on the backend. We can add a custom attribute to our service that, when applied, will automatically perform expansion without the client even knowing. Add a new class named `ExpandPropertyAttribute` to a new folder named `Helpers` in the `Yammerly.Service` solution.

 ```csharp
using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Yammerly.Service.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ExpandPropertyAttribute : ActionFilterAttribute
    {
        string propertyName;

        public ExpandPropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            var uriBuilder = new UriBuilder(actionContext.Request.RequestUri);
            var queryParams = uriBuilder.Query.TrimStart('?').Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            int expandIndex = -1;
            for (var i = 0; i < queryParams.Count; i++)
            {
                if (queryParams[i].StartsWith("$expand", StringComparison.Ordinal))
                {
                    expandIndex = i;
                    break;
                }
            }

            if (expandIndex < 0)
            {
                queryParams.Add("$expand=" + this.propertyName);
            }
            else
            {
                queryParams[expandIndex] = queryParams[expandIndex] + "," + propertyName;
            }

            uriBuilder.Query = string.Join("&", queryParams);
            actionContext.Request.RequestUri = uriBuilder.Uri;
        }
    }
}
 ```

16. Hop back over to the `TimelineItemController` and apply the attribute `ExpandProperty` to the `Get` and `GetAll` endpoints:

 ```csharp
         [ExpandProperty("Author")]
        public IQueryable<TimelineItem> GetAllTimelineItem()
        {
            return Query(); 
        }

        [ExpandProperty("Author")]
        public SingleResult<TimelineItem> GetTimelineItem(string id)
        {
            return Lookup(id);
        }
 ```
 
17. Redeploy the backend to our deployment slot. Open up Postman and perform the query, without using the `$expand` parameter. The `Author` property should now autoexpand for you.
 
18. Awesome! It looks like our backend checks out. Time to move this from our development deployment slot to production. Visit the `Deployment Slots` pane of your Azure Mobile App and select the deployment slot you created earlier. Click `Swap` and ensure there are no warnings and that we are deploying into production, then press `OK`.
 
  ![](/modules/module-2/images/swap_deployment_slot.png)
  
19. Open up Postman and query our production site for `TimelineItem`s. We should see a HTTP 200 OK response and JSON data, with our `Author` property autoexpanded.
 
##### Mobile App

1. Now that our .NET backend is configured and deployed, it's time to connect to it. Because we already are setup to handle data from our Mobile App, no changes are needed to pull down employees. Run the app, and you will see employees load fine. The client is completely agnostic of the backend, so whether you have a no-code backend with Easy Tables or an ASP.NET backend - the `MobileServiceClient` is agnostic!
 
2. Now that we have a functioning backend for `TimelineItem`s, let's take our "MVP" up to a full-blown app. Open up `App.xaml.cs` and change `MainPage` to be a new `RootPage`.
 
3. Next, we need to setup our `AzureService` to handle `TimelineItem`s. Add a line of code after initializing the `MobileSQLiteStore` to define the `TimelineItem` table:
 
  ```csharp
  store.DefineTable<TimelineItem>();
  ```

Run the mobile app, and you will see that we now have a flyout that contains pages for our employee timeline, directory, as well as a profile page. In the next module, we will [explore how to authenticate users with Azure Active Directory](/modules/module-3/).
