using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace OpenIdConnectXRMToolingWebApp.Utils
{
    public class OnBehalfCdsSvcClientAuthHandler : Microsoft.Xrm.Tooling.Connector.IOverrideAuthHookWrapper
    {
        // In memory cache of access tokens
        Dictionary<string, AuthenticationResult> accessTokens = new Dictionary<string, Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult>();

        public void AddAccessToken(Uri orgUri, Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult accessToken)
        {
            // Access tokens can be matched on the hostname,
            // different endpoints in the same organization can use the same access token
            accessTokens[orgUri.Host] = accessToken;
        }

        public string GetAuthToken(Uri connectedUri)
        {
            // Check if you have an access token for this host
            if (accessTokens.ContainsKey(connectedUri.Host) && accessTokens[connectedUri.Host].ExpiresOn > DateTime.Now)
            {
                return accessTokens[connectedUri.Host].AccessToken;
            }
            else
            {
                // check to see if auth manager is present. 
                OnBehalfAuthManager Mgr = (OnBehalfAuthManager)HttpRuntime.Cache[OnBehalfAuthManager.AuthManagerCacheKey];
                if (Mgr != null)
                    accessTokens[connectedUri.Host] = Mgr.GetServiceAccessToken();
                else
                {
                    Mgr = new OnBehalfAuthManager();
                    accessTokens[connectedUri.Host] = Mgr.GetServiceAccessToken();
                    HttpRuntime.Cache.Add(OnBehalfAuthManager.AuthManagerCacheKey, Mgr, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
                return accessTokens[connectedUri.Host].AccessToken;
            }
        }
    }
}