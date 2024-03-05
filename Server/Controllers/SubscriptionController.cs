using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CUnity.Server.Data;
using CUnity.Shared;
using CUnity.Shared.Models;
using CUnity.Shared.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MIC = Microsoft.Identity.Client;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Microsoft.Graph;
using Newtonsoft.Json;
using Microsoft.PowerBI.Api;

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public SubscriptionController(ApplicationDBContext context)
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var devs = await _context.ClientSubscriptions.ToListAsync();
            //return new ApiResponse(Status200OK, string.Empty, devs);
            return Ok(devs);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var dev = await _context.ClientSubscriptions.FirstOrDefaultAsync(a => a.Id == id);
            return Ok(dev);
        }
        [HttpGet("GetClientSubscription")]
        public async Task<IActionResult> GetClientSubscription(int id)
        {
            var dev = await _context.ClientSubscriptions.Where(a => a.ClientId == id).ToListAsync();
            return Ok(dev);
        }
        [HttpGet("GetAllSubscription")]
        public async Task<IActionResult> GetAllSubscription()
        {
            var lst = await (from a in _context.ClientSubscriptions
                             join b in _context.Clients on a.ClientId equals b.Id
                             select new AllSubscriptionViewModel
                             {
                                 ClientName = b.Name,
                                 ProductName = a.ProductName,
                                 SubscriptionStartDate = a.SubscriptionStartDate,
                                 SubscriptionEndDate = a.SubscriptionEndDate,
                                 SubscriptionKey = a.SubscriptionKey,
                                 CreatedDate = a.CreatedDate
                             }).ToListAsync();
            return Ok(lst);
        }

        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts()
        {
            var prd = await _context.ProductMaster.ToListAsync();
            return Ok(prd);
        }

        [HttpGet("ClientListView")]
        public async Task<IActionResult> ClientListView()
        {
            //var prd = await _context.Clients.ToListAsync();

            var lst = await (from a in _context.Clients
                             join p in _context.AzureClientGroups on a.Id equals p.ClientId into ps
                             from p in ps.DefaultIfEmpty()
                             select new CUnity.Shared.Models.ClientListView
                             {
                                 AzureGroupName = p.AzureGroupName,
                                 AzureObjectId = p.AzureObjectId,
                                 IsReportCopied=p.IsReportCopied,
                                 PowerBIWorkSpaceId=p.PowerBIWorkSpaceId,
                                 Id = a.Id,
                                 Email = a.Email,
                                 Name = a.Name
                             }).ToListAsync();
            return Ok(lst);

            //return Ok(prd);
        }

        [HttpGet("GetClients")]
        public async Task<IActionResult> GetClients()
        {
            var prd = await _context.Clients.ToListAsync();
            return Ok(prd);
        }
       
        [HttpPost("SaveClient")]
        public async Task<IActionResult> SaveClient(Shared.Models.Client client)
        {
            if (client.Id == 0)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return Ok(client.Id);
            }
            else
            {
                _context.Entry(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
        [HttpPost("CreateAzureGroup")]
        public async Task<IActionResult> CreateAzureGroup([FromBody] string ClientName)
        {
            try
            {
                GraphServiceClient graphClient;
                graphClient = MicrosoftGraphClient.GetGraphServiceClient();
                var group = new Group
                {
                    Description = ClientName + " descrition",
                    DisplayName = ClientName + " Group",
                    GroupTypes = new List<String>()
                    {

                    },
                    // GroupTypes : [],
                    MailNickname = "apinickname",
                    SecurityEnabled = true,
                    MailEnabled = false,
                };
                var lstGraph = await graphClient.Groups
                            .Request().Filter($"startsWith(displayName,'{group.DisplayName}')").GetAsync();
                //        var users = await graphClient.Users
                //.Request()
                //.Filter("startswith(displayName,'Eric'),")
                //.Select("displayName,signInActivity")
                //.GetAsync();
                if (lstGraph.Count == 0)
                {
                    var res = await graphClient.Groups
                               .Request()
                               .AddAsync(group);
                    var test = res.Id;
                    //await Task.Run(() =>
                    //{
                    //Task<Group> task = Test(ClientName);
                    //task.Wait();
                    //var x = task.Result;
                    ///users/{id | userPrincipalName}
                    //        await graphClient.Users["{user-id}"]
                    //.Request()
                    //.DeleteAsync();
                    //var test = res.Result.wai;
                    //Group test = JsonConvert.DeserializeObject<Group>(JsonConvert.SerializeObject(result));
                    // var res = await Test(ClientName);
                    return Ok(test);
                    //});
                }
                else
                {
                    return Ok(lstGraph.FirstOrDefault().Id);
                }
            }
            catch (Exception ex)
            {
                return Ok("Error");
            }
        }
      
            [HttpPost]
        public async Task<IActionResult> Post(ClientSubscription subscription)
        {
            _context.Add(subscription);
            await _context.SaveChangesAsync();
            return Ok(subscription.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Put(ClientSubscription subscription)
        {
            _context.Entry(subscription).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var subscription = new ClientSubscription { Id = id };
            _context.Remove(subscription);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("SaveAzureClientGroup")]
        public async Task<IActionResult> SaveAzureClientGroup(Shared.Models.AzureClientGroup client)
        {
            if (client.Id == 0)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return Ok(client.Id);
            }
            else
            {
                _context.Entry(client).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
        [HttpGet("GetAzureClientGroup")]
        public async Task<IActionResult> GetAzureClientGroup(int id)
        {
            var dev = await _context.AzureClientGroups.Where(a => a.ClientId == id).FirstOrDefaultAsync();
            return Ok(dev);
        }
    }
}