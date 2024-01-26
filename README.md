Swetugg Web
===========

This is the source code for the Swetugg website. See anything broken? Why not send a PR? :)

Getting started
---------------

### Running the site locally
There are a few steps necessary before you can run the site locally.
Update the Connectionstring DefaultConnection in `web.config` from the value in 1pass for the dev database. Most development only need this but in case something else is needed see the section on [Configuring](#configuring) for more information.

### Admin page
The entry point to the admin backend is found at `/start`. Use the same login as for the main site.

### Configuring
-----------
There are a few things that need to be set for the web site to work completely.

### Connection strings
The connection string for the database is named `DefaultConnection`. The required value can be found in 1pass.
This database is copied from live and replaced once per day, so it will always be fresh.

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

