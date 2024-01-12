Swetugg Web
===========

This is the source code for the Swetugg website. See anything broken? Why not send a PR? :)

A possibly working sample of this is published to http://swetuggpreview.azurewebsites.net/start. If you want admin privileges to try it out, create an account and ping me [@CodingInsomnia](https://twitter.com/CodingInsomnia) on Twitter.

Getting started
---------------

### Running the site locally
There are a few steps necessary before you can run the site locally.
First of all, you need to copy the `Web.Example.config` file to `web.config` and fill out the necessary settings. See the section on [Configuring](#configuring) for more information.
The web.config file is included in the .gitignore file, so you don't have to worry about accidentally checking in your settings.

### Admin page
The entry point to the admin backend is found at `/start`. In order to do anything you have to have an account, the default admin account is `info@swetugg.se` with password `ChangeMe.123`. 

### Creating a new conference
The first thing you need to do is to create a conference. You do this by selecting Admin->Conferences on the top menu, then hitting the New conference button and filling out the form. Be careful when you choose the "slug", as this will need to be matched with the name of an area route in your app. 

Once the conference is created, you'll end up on the main admin page for it. This is where you'll find links to manage speakers and sessions as well as setting up rooms and finally the schedule.


Configuring
-----------
There are a few things that needs to be set up in order for the web site to work completely.

### Connection strings
The connection string for the database is named `DefaultConnection`. The required value can be found in 1pass.
This database is copied from live and replaced once per day, so it will always be fresh. Note that this means that you may need to reapply your migrations and reseed the database between days.

It is also possible to run the site with a local database. In order to do this, you need to change the connection string to point to a local database. Once you've set the connection string to an existing database, you will need to run the migration and the seed method.

There is a second connection string called `StorageConnection` which is used for storing images and other files. This is not necessary for the site to work, but it will not be possible to upload images for speakers etc if this is not set up correctly.

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

