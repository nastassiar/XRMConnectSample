using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Security.Claims;

namespace OpenIdConnectXRMToolingWebApp.Utils
{
    public class OnBehalfAuthManager
    {
        public static readonly string AuthManagerCacheKey = "AuthMgr";
        public static readonly string CrmSvcClientCacheKey = "CrmSvcClient";

        private static Version _ADALAsmVersion;

        private AuthenticationParameters ctxParms = null;

        public OnBehalfAuthManager()
        {

        }

        public string Resource { get { if (ctxParms != null) return ctxParms.Resource; else return string.Empty; } }

        public string Authority { get { if (ctxParms != null) return ctxParms.Authority; else return string.Empty; } }

        public AuthenticationResult GetServiceAccessToken()
        {
            string TargetOrgUri = ConfigurationManager.AppSettings["ResourceUri"] +"XRMServices/2011/Organization.svc/web?SdkClientVersion=9.0.0.533";

            if (ctxParms == null)
                ctxParms = GetAuthorityFromTargetService(new Uri(TargetOrgUri));

            string clientId = ConfigurationManager.AppSettings["ClientId"];
            string appKey = ConfigurationManager.AppSettings["AppSecret"];
            string cdsResourceId = ctxParms.Resource;

            AuthenticationContext authContext = new AuthenticationContext(ctxParms.Authority);
            ClientCredential credential = new ClientCredential(clientId, appKey); // this is the site application ( needed to be granted impersonate CRM user )
            UserIdentifier currentUser = new UserIdentifier(ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value, UserIdentifierType.UniqueId);
            AuthenticationResult accessToken = authContext.AcquireTokenSilentAsync(cdsResourceId, credential, currentUser).Result;

            return accessToken;
        }

        #region utils. 
        /// <summary>
        /// Get the Authority and Support data from the requesting system using a sync call. 
        /// </summary>
        /// <param name="targetServiceUrl"></param>
        /// <returns>Populated AuthenticationParameters or null</returns>
        private static AuthenticationParameters GetAuthorityFromTargetService(Uri targetServiceUrl)
        {
            try
            {
                // if using ADAL > 4.x  return.. // else remove oauth2/authorize from the authority
                if (_ADALAsmVersion == null)
                {
                    // initial setup to get the ADAL version 
                    var AdalAsm = System.Reflection.Assembly.GetAssembly(typeof(IPlatformParameters));
                    if (AdalAsm != null)
                        _ADALAsmVersion = AdalAsm.GetName().Version;
                }

                var foundAuthority = AuthenticationParameters.CreateFromUrlAsync(targetServiceUrl).Result;
                if (_ADALAsmVersion != null && _ADALAsmVersion > Version.Parse("4.0.0.0"))
                {
                    foundAuthority.Authority = foundAuthority.Authority.Replace("oauth2/authorize", "");
                }

                return foundAuthority;
            }
            catch (Exception ex)
            {
                // Todo: Add exception handling  
            }
            return null;
           
        }

        #endregion

    }
}