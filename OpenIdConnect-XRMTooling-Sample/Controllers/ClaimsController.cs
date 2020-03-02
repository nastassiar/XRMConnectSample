using OpenIdConnectXRMToolingWebApp.Utils;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Web.Mvc;

namespace OpenIdConnectXRMToolingWebApp.Controllers
{
    [Authorize]
    public class ClaimsController : Controller
    {
        /// <summary>
        /// Add user's claims to viewbag
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            CrmServiceClient service = null;
            string cdsStuff = "Not Logged in yet";
            string cdsInfo = "Not Logged in yet";
            bool loggedIn = false;
            try
            {
                var mgr = new OnBehalfCDSConnectionMgr();
                service = mgr.GetCdsConnectionClient();
                if (service.IsReady)
                {
                    Guid userid = ((WhoAmIResponse)service.Execute(new WhoAmIRequest())).UserId;

                    loggedIn = true;
                    cdsInfo = "Successfully logged in!";
                    cdsStuff = userid.ToString();
                }
                else
                {
                    loggedIn = false;
                    cdsInfo = "Not able to login";
                    const string UNABLE_TO_LOGIN_ERROR = "Unable to Login to Common Data Service";
                    if (service.LastCrmError.Equals(UNABLE_TO_LOGIN_ERROR))
                    {

                        cdsStuff = "Check the connection string values in cds/App.config.";
                        throw new Exception(service.LastCrmError);
                    }
                    else
                    {
                        throw new Exception(service.LastCrmError);
                    }
                }
            }
            catch (Exception ex)
            {
                cdsInfo = "Error/Exception when Logging in";
                cdsStuff = "Exception :" + ex.Message;
            }

            finally
            {
                if (service != null)
                    service.Dispose();
            }

            ViewBag.CDSStuff = cdsStuff;
            ViewBag.CDSInfo = cdsInfo;
            ViewBag.LoggedIn = loggedIn;

            var userClaims = User.Identity as System.Security.Claims.ClaimsIdentity;

            //You get the user’s first and last name below:
            ViewBag.Name = userClaims?.FindFirst("name")?.Value;

            // The 'preferred_username' claim can be used for showing the username
            ViewBag.Username = userClaims?.FindFirst("preferred_username")?.Value;

            // The subject/ NameIdentifier claim can be used to uniquely identify the user across the web
            ViewBag.Subject = userClaims?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // TenantId is the unique Tenant Id - which represents an organization in Azure AD
            ViewBag.TenantId = userClaims?.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            return View();
        }
    }
}