using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using CUnity.Shared;
using Microsoft.Graph.Auth;

namespace CUnity.Server.Data
{
    public class MicrosoftGraphClient
    {
        private static GraphServiceClient graphClient;
        private static ClientCredentialProvider authProvider;
        static MicrosoftGraphClient()
        {
            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
             .Create(CUnity.Shared.Constants.AppId)
             .WithTenantId(CUnity.Shared.Constants.TenantId)
             .WithClientSecret(CUnity.Shared.Constants.ClientSecret)
             .Build();
            authProvider = new ClientCredentialProvider(confidentialClientApplication);
        }
        public static GraphServiceClient GetGraphServiceClient()
        {
            graphClient = new GraphServiceClient(authProvider);
            return graphClient;
        }
    }
}
