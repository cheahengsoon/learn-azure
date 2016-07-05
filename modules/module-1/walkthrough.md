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
