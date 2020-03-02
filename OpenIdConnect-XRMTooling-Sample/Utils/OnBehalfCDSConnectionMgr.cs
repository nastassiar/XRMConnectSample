using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace OpenIdConnectXRMToolingWebApp.Utils
{
    public class OnBehalfCDSConnectionMgr
    {
        private CrmServiceClient _svcClient = null;

        /// <summary>
        /// Get CrmServiceClient connection for currently signed in user. 
        /// </summary>
        /// <returns></returns>
        public CrmServiceClient GetCdsConnectionClient()
        {
            string TargetOrgUri = ConfigurationManager.AppSettings["ResourceUri"];
            CrmServiceClient cdsSvcClient = (CrmServiceClient)HttpRuntime.Cache[OnBehalfAuthManager.CrmSvcClientCacheKey];
            if (cdsSvcClient == null)
            {
                // Set up new client 
                CrmServiceClient.AuthOverrideHook = new OnBehalfCdsSvcClientAuthHandler();
                _svcClient = new CrmServiceClient(instanceUrl: new Uri(TargetOrgUri), useUniqueInstance: true);
                HttpRuntime.Cache.Add(OnBehalfAuthManager.CrmSvcClientCacheKey, _svcClient, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.High, null);

                cdsSvcClient = _svcClient;
            }

            var userMap = (Dictionary<Guid, Guid>)HttpRuntime.Cache["CDSUserMap"];
            if (userMap == null)
            {
                userMap = new Dictionary<Guid, Guid>();
                HttpRuntime.Cache.Add("CDSUserMap", userMap, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.High, null);
            }

            CrmServiceClient outClient = cdsSvcClient.Clone();
            return outClient;
        }


    }

}