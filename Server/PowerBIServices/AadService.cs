using CUnity.Server.Data;
using CUnity.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.Linq;
namespace CUnity.Server.PowerBIServices
{
    public class AadService
    {
        private readonly ApplicationDBContext _context;
        public AadService(ApplicationDBContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Generates and returns Access token
        /// </summary>
        /// <returns>AAD token</returns>
        public string GetAccessToken()
        {
            AuthenticationResult authenticationResult = null;
            //if (azureAd.Value.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    // Create a public client to authorize the app with the AAD app
            //    IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(azureAd.Value.ClientId).WithAuthority(azureAd.Value.AuthorityUri).Build();
            //    var userAccounts = clientApp.GetAccountsAsync().Result;
            //    try
            //    {
            //        // Retrieve Access token from cache if available
            //        authenticationResult = clientApp.AcquireTokenSilent(azureAd.Value.Scope, userAccounts.FirstOrDefault()).ExecuteAsync().Result;
            //    }
            //    catch (MsalUiRequiredException)
            //    {
            //        SecureString password = new SecureString();
            //        foreach (var key in azureAd.Value.PbiPassword)
            //        {
            //            password.AppendChar(key);
            //        }
            //        authenticationResult = clientApp.AcquireTokenByUsernamePassword(azureAd.Value.Scope, azureAd.Value.PbiUsername, password).ExecuteAsync().Result;
            //    }
            //}

            // Service Principal auth is the recommended by Microsoft to achieve App Owns Data Power BI embedding
            //else if (azureAd.Value.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
            //{
            var lst = _context.AzureSettings.ToList();
            var cfglst = lst.Where(p => p.GroupName == "AzureAd").ToList();
            //string PowerBIWorkSpaceId = lst.Where(p => p.GroupName == "PowerBI" && p.Key == "WorkspaceId").Select(c => c.Value).ToString();
            string AuthorityUri = cfglst.Where(p=>p.Key == "AuthorityUri").FirstOrDefault().Value.ToString();
            string TenantId = cfglst.Where(p => p.Key == "TenantId").FirstOrDefault().Value.ToString();
            string ClientId = cfglst.Where(p => p.Key == "ClientId").FirstOrDefault().Value.ToString();
            string ClientSecret = cfglst.Where(p => p.Key == "ClientSecret").FirstOrDefault().Value.ToString();
            string[] Scope = cfglst.Where(p => p.Key == "Scope").FirstOrDefault().Value.Split(",");

            // For app only authentication, we need the specific tenant id in the authority url
            var tenantSpecificUrl = AuthorityUri.Replace("organizations", TenantId);

            // Create a confidential client to authorize the app with the AAD app
            IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                                            .Create(ClientId)
                                                                            .WithClientSecret(ClientSecret)
                                                                            .WithAuthority(tenantSpecificUrl)
                                                                            .Build();
            // Make a client call if Access token is not available in cache
            authenticationResult = clientApp.AcquireTokenForClient(Scope).ExecuteAsync().Result;
            //}

            return authenticationResult.AccessToken;
        }
    }
}
