# azure-training

Learn how to build connected mobile apps with Xamarin and Azure. This training walks you through the process of creating a backend for a social network for employees, like Yammer.

### Overview

* Morning
   * High-level overview of Azure App Service
   * Competitive landscape
   * Build an MVP with Azure Easy Tables
* Afternoon
   * Scale this out to a full ASP.NET backend with data storage, identity, and push.
   * Tie in other services if we have time.

### Outline

1. Build an MVP with Azure Easy Tables.
2. Build a complete backend with Azure Mobile Apps and an ASP.NET Web API backend.
3. Add identity with Azure Active Directory (AAD) authentication.
4. Add push notifications with Azure Notification Hubs.
5. Add blob storage with Azure Storage.
6. Manage and secure our API with Azure API Management.

Optional:

1. Azure Functions
2. Azure Machine Learning
3. Microsoft Cognitive Services

Topics covered on a one-off basis are:

* Dev Ops with Visual Studio Team Services (VSTS) and HockeyApp
* Azure API Apps
* Hybrid connections

#### 1. Build an MVP with Azure Easy Tables
Objective: Build a backend for our app, and connect it with our frontend to have online/offline sync and conflict resolution.

1. Download starter code for app (uses local data).
2. Create a new Azure Mobile Apps instance and configure Easy Tables.
3. Consume our Easy Tables backend from our mobile application using online/offline sync.
4. Add a custom conflict resolution handler.
5. Investigate using Azure.Mobile, a helper library that makes connecting to an Azure backend and having online/offline sync four lines of code.
6. Look at the limitations of using Azure Easy Tables over full-fledged Azure Mobile Apps. Why would you even opt for using Easy Tables in the first place?

#### 2. Build a complete backend with Azure Mobile Apps (Web API Backend)
Objective: Build a more customizable backend with Azure Mobile Apps that we have full control over.

1. Create a new Azure Mobile Apps instance and configure our data source.
2. Download the starter code for our ASP.NET Web API backend.
3. Add basic data storage to our backend.
4. Add navigation properties to our model, and expand this data on our backend automatically.

#### 3. Add identity with Azure Active Directory.
Objective: Add authentication to our backend with Azure Active Directory

1. Create an Azure Active Directory instance.
2. Register our application to authenticate with Azure AD.
3. Configure our backend to authenticate API calls with Azure AD.
4. Add authentication to our mobile applications.

#### 4. Add push notifications with Azure Notification Hubs.
Objective: Add push notification support to our mobile app for Android and UWP.

1. Create a new Azure Notification Hub.
2. Add a web page for sending broadcast notifications.
3. Add an API for sending broadcast notifications.
4. Add push notification handling to our Android and UWP apps.
5. Add support for segmented, templated push notifications in our backend.

#### 5. Add blob storage with Azure Storage.
Objective: Create an interactive timeline with Azure Blob Storage.

1. Create a new Azure Storage account.
2. Create a new container.
3. Add blob storage directly from our mobile apps using Azure Storage.
4. Add blob storage to our backend, as a custom API.
5. Check the image for adult content, using Microsoft Cognitive Services.

#### 6. Manage and secure our API with Azure API Management.

TODO

#### Additional Topics
##### Dev Ops with Xamarin, VSTS, and HockeyApp

1. Create a Visual Studio Team Services page for our app.
2. Add continuous integration for our mobile app.
3. Add deployment with HockeyApp.

##### Hybrid connections

1. Connect our Yammer clone to a database on-prem with hybrid connections.


##### Azure API Apps

1. Build a simple API with API apps.

#### Optional Topics

##### Azure Functions
TODO

##### Azure Search
TODO

##### Azure Machine Learning
Show off cognitive services recommendations demo.

##### Microsoft Cognitive Services
Add facial recognition to employee photos uploaded.

### Pierce TODOS

* Create introductory deck.
* Name our mobile app
* Create localized app.
* Create connected app.
* Write tutorial steps / demo script.
* Create closing deck.
* Add code as "releases" to GitHub.
* Practice
* Turn into sample application.
* Record as a full workshop for PierceBoggan.com.
