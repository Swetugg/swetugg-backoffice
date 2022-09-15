Swetugg Web
===========

This is the source code for the Swetugg website. See anything broken? Why not send a PR? :)

A possibly working sample of this is published to http://swetuggpreview.azurewebsites.net/start. If you want admin privileges to try it out, create an account and ping me [@CodingInsomnia](https://twitter.com/CodingInsomnia) on Twitter.

Getting started
---------------
You should be able to start up an empty version of the swetugg site simply by hitting F5 in Visual Studio. This will create a local database using Entity Framework Code First Migrations, if this fails with the error message "failed with the operating system error 2(The system cannot find the file specified.).", then try removing your `App_Data` folder and recreate it from Visual Studio. If this fails with the exception `SqlException: Cannot attach the file 'C:\projects\swetugg-web\src\Swetugg.Web\App_Data\aspnet-Swetugg.Web-20150406030259.mdf' as database 'aspnet-Swetugg.Web-20150406030259'.` then you may create it by opening `TOOLS > Connect o Database` and navigate to `{project location}\src\Swetugg.Web\App_Data\aspnet-Swetugg.Web-20150406030259.mdf` and click `yes` when it asks if you want to create the file. However, since the database will be empty you won't be able to actually browse to the frontpage. The first thing you should do is to make sure the database is seeded with a default admin account:

### Seed the database
In Visual Studio, open up the Package Manager Console and run:

    Update-Database

### Admin page
The entry point to the admin backend is found at `/start`. In order to do anything you have to have an account, the default admin account is `info@swetugg.se` with password `ChangeMe.123`. 

### Creating a new conference
The first thing you need to do is to create a conference. You do this by selecting Admin->Conferences on the top menu, then hitting the New conference button and filling out the form. Be careful when you choose the "slug", as this will need to be matched with the name of an area route in your app. 

Once the conference is created, you'll end up on the main admin page for it. This is where you'll find links to manage speakers and sessions as well as setting up rooms and finally the schedule.

### Call For Papers
Anyone can create an account on the conference site and get access to the Call For Papers (found at `/cfp`). There they can add a bio, contact information and any number of sessions.

### Reviewing CFP
A conference manager can (via the main conference admin page) review all submissions to the CFP and promote any speaker and sessions to the conference. Promoting a speaker/session creates a link between the CFP entry and the conference entry. If the CFP entry is changed, the conference entry can be updated by hitting the Update button.

Configuring
-----------
There are a few things that needs to be set up in order for the web site to work completely.

### Connection strings
The connection string named `DefaultConnection` is used as the main database for the Swetugg site. By default this is a local SQL Express file placed in the `App_Data` folder.
If running SQL Server 2014 then change `Data Source=(LocalDb)\v11.0;` to `Data Source=(LocalDb)\v12.0;` in the `DefaultConnection` connection string. SQL2014 doesn't create a local db instance some cases, you can resolve that by running `sqllocaldb create "v12.0"` from administrator command prompt.

Speaker images are uploaded to Azure Blob storage if the StorageConnection connection string is set correctly

    <!-- Connection to Blob storage for images etc -->
    <add name="StorageConnection" connectionString="DefaultEndpointsProtocol=https;AccountName={account-name};AccountKey={account-key}" />

### Login providers
There are entries in `web.config` to enable login via 3rd party providers such as Facebook and Google. To enable each provider, set its corresponding Enabled-setting to true and fill out the different keys. For more information on how to get the keys, see [this article](http://go.microsoft.com/fwlink/?LinkId=403804)

    <!-- Social media logins -->
    <add key="Facebook_Api_Enabled" value="false" />
    <add key="Facebook_Api_AppId" value="{AppId}" />
    <add key="Facebook_Api_AppSecret" value="{AppSecret}" />

    <add key="Google_Api_Enabled" value="false" />
    <add key="Google_Api_ClientId" value="{ClientId}" />
    <add key="Google_Api_ClientSecret" value="{AppSecret}" />

    <add key="Twitter_Api_Enabled" value="false" />
    <add key="Twitter_Api_ConsumerKey" value="{ConsumerKey}" />
    <add key="Twitter_Api_ConsumerSecret" value="{ConsumerSecret}" />

    <add key="Microsoft_Api_Enabled" value="false" />
    <add key="Microsoft_Api_ClientId" value="{ClientId}" />
    <add key="Microsoft_Api_ClientSecret" value="{AppSecret}" />
    <!-- / Social media logins -->

### Sendgrid
In order to be able to confirm email addresses and send password reset emails, create a Sendgrid account and fill out these fields in `web.config`

    <!-- Messaging -->
    <add key="SendGrid_Messaging_Enabled" value="false"/>
    <add key="SendGrid_Messaging_MailAccount" value="{Sendgrid Account}"/>
    <add key="SendGrid_Messaging_MailPassword" value="{Sendgrid Password}"/>
    <!-- / Messaging -->

