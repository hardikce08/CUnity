using CUnity.Shared.Models;
using CUnity.Server.PowerBIServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using CUnity.Server.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PAM = Microsoft.PowerBI.Api.Models;
using Microsoft.PowerBI.Api;

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PowerBIEmbedInfoController : ControllerBase
    {
        private readonly PbiEmbedService pbiEmbedService;
        private readonly ApplicationDBContext _context;
        public PowerBIEmbedInfoController(PbiEmbedService pbiEmbedService, ApplicationDBContext context)
        {
            this.pbiEmbedService = pbiEmbedService;
            this._context = context;
        }

        public string GetEmbedInfoReport(string reportID)
        {
            try
            {
                var lst = _context.AzureSettings.ToList();
                var powerbiconfigurationlst = lst.Where(p => p.GroupName == "PowerBI").FirstOrDefault();
                string PowerBIWorkSpaceId = powerbiconfigurationlst.Value;
                // Validate whether all the required configurations are provided in appsettings.json
                //string configValidationResult = ConfigValidatorService.ValidateConfig(azureAd, powerBI);
                EmbedParams embedParams = pbiEmbedService.GetEmbedParams(new Guid(PowerBIWorkSpaceId), new Guid(reportID));
                return JsonSerializer.Serialize<EmbedParams>(embedParams);
            }
            catch (Exception ex)
            {
                HttpContext.Response.StatusCode = 500;
                return ex.Message + "\n\n" + ex.StackTrace;
            }
        }

        [HttpPost("CreatePowerBIWorkSpace")]
        public async Task<IActionResult> CreatePowerBIWorkSpace(Shared.Models.AzureClientGroup client)
        {
            var pbiClient = pbiEmbedService.GetPowerBIClient();
            PAM.Group createworkspace;

            PAM.GroupUser grpUser = new PAM.GroupUser();
            grpUser.Identifier = client.AzureObjectId;
            grpUser.PrincipalType = "Group";
            grpUser.GroupUserAccessRight = "Viewer";

            PAM.Groups grptest = new PAM.Groups();
            createworkspace = await CreatePowerBiWorkspace(client.AzureGroupName);

            //grptest = await pbiClient.Groups.GetGroupsAsync();

            var test = pbiClient.Groups.AddGroupUserAsync(createworkspace.Id, grpUser);

            // copy reports functionality
            //Guid workspaceId = new Guid("685a65e7-9ca6-4db7-a924-58528e13bf9d");

            //PAM.CloneReportRequest cloneTarget = new PAM.CloneReportRequest();
            //cloneTarget.TargetWorkspaceId = createworkspace.Id;// new Guid("a651bd0e-b192-4f4e-8f28-4f1aed9f1831");
            //cloneTarget.Name = "API DEMO - Hardik Test";
 
            //var grpClone = await pbiClient.Reports.CloneReportAsync(workspaceId, new Guid("62823edd-78ed-451f-9c60-189d799fbdd4"), cloneTarget);


            return Ok();
        }
        public async Task<PAM.Group> CreatePowerBiWorkspace(string Name)
        {
            var pbiClient = pbiEmbedService.GetPowerBIClient();

            PAM.Group createworkspace;
            

                //Guid workspaceId = new Guid("685a65e7-9ca6-4db7-a924-58528e13bf9d");
                // var reports = client.Reports.GetReportsAsAdmin(workspaceId);

                // var test = client.Reports.GetReports();
                //var reports = await client.Reports.GetReportsInGroupAsync(workspaceId);


                PAM.GroupCreationRequest groupInfo = new PAM.GroupCreationRequest();
                groupInfo.Name = Name;
                createworkspace = pbiClient.Groups.CreateGroup(groupInfo, true);



            



            return createworkspace;
            //createworkspace.Id



        }
        //public string GetEmbedInfo()
        //{
        //    try
        //    {
        //        // Validate whether all the required configurations are provided in appsettings.json
        //        EmbedParams embedParams = pbiEmbedService.GetEmbedParams(new Guid(PowerBIWorkSpaceId), new Guid(_configuration.GetSection("PowerBI:ReportId").Get<string>()));
        //        return JsonSerializer.Serialize<EmbedParams>(embedParams);
        //    }
        //    catch (Exception ex)
        //    {
        //        HttpContext.Response.StatusCode = 500;
        //        return ex.Message + "\n\n" + ex.StackTrace;
        //    }
        //}

    }
}
