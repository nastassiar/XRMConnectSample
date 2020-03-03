# XRMConnectSample

*NOTE: This is just a sample and is not being actively maintained and should not be considered production ready code*

Sample using OpenID connect sample and the XRM tooling library to connect to Dynamics365. 
 
Built upon an [ASP.NET AAD Sample](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-v2-aspnet-webapp) and using the [OAuth behalf of flow to connect](https://docs.microsoft.com/en-us/azure/active-directory/develop/v2-oauth2-on-behalf-of-flow) to the [Common Data Service](https://docs.microsoft.com/en-us/powerapps/maker/common-data-service/data-platform-intro).

In this sample a user will login to Azure Active Directory, within the code there will then be another call out to AAD to get a token for Dynamics/CDS. See Utils/OnBehalfAuthManager.cs for more details. 
 
 ## To Run
 
 ### Register your application
 
To register your application and add the app's registration information to your solution manually, follow these steps:

1. Sign in to the [Azure portal](https://portal.azure.com) using either a work or school account, or a personal Microsoft account.
2. If your account gives you access to more than one tenant, select your account in the top right corner, and set your portal session to the desired Azure AD tenant.
3. Navigate to the Microsoft identity platform for developers [App registrations](https://go.microsoft.com/fwlink/?linkid=2083908) page.
4. Select *New registration*.
5. When the *Register an application* page appears, enter your application's registration information:
 * In the *Name* section, enter a meaningful application name that will be displayed to users of the app, for example ASPNET-XRM-Quickstart.
 * Add https://localhost:44301/ in *Redirect URI*, and click *Register*.
  * Make sure there is a / at the end
 * From the left navigation pane under the Manage section, select *Authentication*
  * Under the Implicit Grant sub-section, select *ID tokens*.
  * And then select Save.
6. After it has saved from the left naivgation under the Manage section, select *API Permissions*
 * Select Add a permission 
 * Find Dynamics CRM (under Microsoft APIs)
 * Select user_impersonation
 * Click Add permissions at the bottom of the page
 * If you are an admin you can click Grant Admin Consent from Microsoft
  * This will save you having to Grant consent upon initial sign in
7. From the left naivgation under the Manage section, select *Certificates & secrets*
 * Click *New Client secret*, provide a name and click add
 * Note the value somewhere (this will not appear again after you leave the page)
8. From the left naivgation select *Overview*
 * Note doen the Application (client) ID and Directory (tenant) ID. These will both need to be added to the Web.config

### Configure Project

1. Download the repository from Github
2. Open the solution in Visual Studio (OpenIdConnect-XRMTooling-Sample.sln)
3. Depending on the version of Visual Studio, you might need to right click on the project OpenIdConnect-XRMTooling-Sample and Restore NuGet packages
4. Open the Package Manager Console (View -> Other Windows -> Package Manager Console) and run Update-Package Microsoft.CodeDom.Providers.DotNetCompilerPlatform -r
5. Edit Web.config and fill in the values for *ClientId*, *Tenant*, *AppSecret* with the values from steps 7 and 8 from Registering your Application
6. Replace the value of *ResourceUri* with the URL to your own Dynamics CRM

### Run the project
1. Run the project and navigate to https://localhost:44301/ in an incognito browser
2. This should pop up a page with a sign in button.
3. Sign in as a user that is within your AAD tenant
