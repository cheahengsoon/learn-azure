# Additional Module: Azure Functions
**Objective**: Create a time-triggered Azure Function to clear out soft deletions from our database on a regular interval.
**Estimated Effort**: 10 minutes

###### Prerequisites
* Xamarin
* Azure subscription
* Completion of up to [Module 2](/modules/module-2/)

### Instructions
##### Azure Function

1. Azure Functions allow us to run code in response to triggers - time- or event-based. Let's create a new Azure Function App that will run on a regular interval and clear out soft deletions from our database. In the [Azure Portal](portal.azure.com), click `Add -> Web + Mobile -> Function App`, or search for `Function App`.

 ![](/additional-modules/function-app/images/add_new_function_app.png)

2. Enter information for your function app. Note that an instance of Azure Storage will automatically be spun up for you.

 ![](/additional-modules/function-app/images/new_function_app.png)
 
3. Follow the quickstart guide to create a new Azure Function. We want a `Time-Based Trigger` and `C#` as the language. Click `Create this Function`.

 ![](/additional-modules/function-app/images/create_new_function.png)

4. We can now write code for our function in the editor! We can even bring in namespaces such as `System.Data` using `#r`. Paste the [function code](/additional-modules/function-app/function_code.cs) into the editor, and click `Save`. This code will connect to our database and clear out the rows marked for deletion.

 ![](/additional-modules/function-app/images/add_function_code.png)

5. Click the `Integrate` tab. We can add triggers, inputs, and outputs on this pane. In our case, we want to configure the schedule for our job. We can paste in `Cron` values to define execution times. This function uses `0 0 3 * 0`.

 ![](/additional-modules/function-app/images/set_time_parameters.png)

6. Next, we need to configure a connection string to our database for our function to run. Click `Function App Settings` then `Go to App Service Settings`. Just like any App Service, there are application-specific settings we can add, including connection strings.

 ![](/additional-modules/function-app/images/open_app_settings.png)

7. Navigate to `Application Settings` and paste in a new connection string with name `sqldb` and value from your `Azure Mobile App` you created in `Module 2`. Click `Save`.

 ![](/additional-modules/function-app/images/add_connection_string.png)

8. OK! Now we are ready to run our function. It will run on the set time interval automatically, but we can force it to run by clicking the `Run` button on the `Develop` tab. Make sure we get back a `202 Accepted`, and our function succeeded!

 ![](/additional-modules/function-app/images/run_function.png)

Functions are great for numerous things! Unlike a WebJob, which runs continuously, Functions are smart enough to scale up and down at the correct time, saving you money. We took a look at a time-based function, but there's awesome event-based functions you can do as well. For example, when a new blob is added to our `images` container (such as a new photo posted to the timeline), we can use a function to scale down that image to a lesser resolution and perform an adult content check with Microsoft Cognitive Services to make sure the photo is OK for the whole company to see.
