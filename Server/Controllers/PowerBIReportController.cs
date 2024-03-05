using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CUnity.Server.Data;
using CUnity.Server.PowerBIServices;
using CUnity.Shared.Models;
using CUnity.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PAM = Microsoft.PowerBI.Api.Models;
using Microsoft.PowerBI.Api;
using CUnity.Server.Middleware;

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBIReportController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly PbiEmbedService pbiEmbedService;

        public PowerBIReportController(PbiEmbedService pbiEmbedService, ApplicationDBContext context)
        {
            this.pbiEmbedService = pbiEmbedService;

            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var devs = await _context.PowerBIReports.ToListAsync();
            return Ok(devs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dev = await _context.PowerBIReports.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(dev);
        }
        [HttpPost]
        public async Task<IActionResult> Post(PowerBIReport developer)
        {
            _context.Add(developer);

            await _context.SaveChangesAsync();
            return Ok(developer.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(PowerBIReport developer)
        {
            _context.Entry(developer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var dev = new PowerBIReport { Id = id };
            _context.Remove(dev);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("GetPowerBIReports")]
        public async Task<IActionResult> GetPowerBIReports()
        {
            //string text = s.ClientName + "," + s.StartDate.ToString() + "," + s.EndDate.ToString() + "," + s.ProductName;
            var res = await _context.PowerBIReports.ToListAsync();
            return Ok(res);
        }

        [HttpPost("CreatePowerBIWorkSpace")]
        public async Task<IActionResult> CreatePowerBIWorkSpace(Shared.Models.AzureClientGroup client)
        {
            try
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

                return Ok(createworkspace.Id);
            }
            catch (Exception ex)
            {
                await ExceptionLogging.SendExcepToDB("CreatePowerBIWorkSpace", ex, _context);
                return Ok("Error");
            }
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
            createworkspace = await pbiClient.Groups.CreateGroupAsync(groupInfo, true);

            return createworkspace;
        }
        [HttpPost("CopyPowerBIReports")]
        public async Task<IActionResult> CopyPowerBIReports([FromBody] string TargetWorkSpaceId)
        {
            try
            {
                var pbiClient = pbiEmbedService.GetPowerBIClient();
                // copy reports functionality
                var reports = await pbiClient.Reports.GetReportsInGroupAsync(new Guid("685a65e7-9ca6-4db7-a924-58528e13bf9d"));

                foreach (var report in reports.Value)
                {
                    PAM.CloneReportRequest cloneTarget = new PAM.CloneReportRequest();
                    cloneTarget.TargetWorkspaceId = new Guid(TargetWorkSpaceId);
                    cloneTarget.Name = report.Name;
                    cloneTarget.TargetModelId = report.DatasetId;

                    //PAM.RebindReportRequest rTarget = new PAM.RebindReportRequest();
                    //rTarget.DatasetId = report.DatasetId;
                     
                   var grpClone = await pbiClient.Reports.CloneReportAsync(new Guid("685a65e7-9ca6-4db7-a924-58528e13bf9d"), report.Id, cloneTarget);
                    
                  //var grpClone1 = pbiClient.Reports.RebindReportAsync(grpClone.Id, rTarget);
                    //var r = await pbiClient.Reports.GetReportsInGroupAsync(new Guid(TargetWorkSpaceId));
                    //foreach (var rr in r.Value)
                    //{
                    //    if (rr.Name != report.Name)
                    //        
                    //}
                }
                return Ok("Done");
            }
            catch (Exception ex)
            {
                await ExceptionLogging.SendExcepToDB("CopyPowerBIReports", ex, _context);
                return Ok("Error");
            }
        }

    }
}