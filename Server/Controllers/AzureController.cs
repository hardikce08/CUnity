using CUnity.Server.Data;
using CUnity.Server.Models;
using CUnity.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AzureController : ControllerBase
    {
        // GET: api/<AzureController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<AzureController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AzureController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AzureController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AzureController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #region Azure Client Groups

        #endregion

        [HttpGet("GetAzureGroupUsers")]
        public async Task<IActionResult> GetAzureGroupUsers(string GroupId)
        {
            GraphServiceClient graphClient;
            graphClient = MicrosoftGraphClient.GetGraphServiceClient();
            List<AzureUserList> lstuser = new List<AzureUserList>();
            var members = await graphClient.Groups[GroupId].Members
    .Request()
    .GetAsync();

            //        var memberOf = await graphClient.Users[""].MemberOf
            //.Request()
            //.GetAsync();
            foreach (var user in members)
            {
                //Console.WriteLine(JsonConvert.SerializeObject(user));
                Microsoft.Graph.User objuser = (User)user;
                lstuser.Add(new AzureUserList { AzureGroupId = GroupId, DisplayName = objuser.DisplayName, AzureObjectId = objuser.Id, GivenName = objuser.GivenName, FirstName = objuser.UserPrincipalName });
                // Only output the custom attributes...
                //Console.WriteLine(JsonConvert.SerializeObject(user.AdditionalData));
            }
            return Ok(lstuser);
        }
        [HttpPost("RemoveUserFromAzureGroup")]
        public async Task<IActionResult> RemoveUserFromAzureGroup(AzureUserList azureuser)
        {
            GraphServiceClient graphClient;
            graphClient = MicrosoftGraphClient.GetGraphServiceClient();

            await graphClient.Groups[azureuser.AzureGroupId].Members[azureuser.AzureObjectId].Reference.Request().DeleteAsync();
            return Ok();
        }

        [HttpPost("AddUsertoAzureGroup")]
        public async Task<IActionResult> AddMemberUsertoAzureGroup(AzureUserList azureuser)
        {
            GraphServiceClient graphClient;
            graphClient = MicrosoftGraphClient.GetGraphServiceClient();

            try
            {


                var lst1 = await graphClient.Users
                                .Request().Filter($"userprincipalname eq '{azureuser.EmailAddress}'").GetAsync();
                if (lst1.Count > 0)
                {
                    string a = lst1.FirstOrDefault().Id;

                    var groupIds = new List<String>()
{
    azureuser.AzureGroupId
};
                    var lstgroup = await graphClient.Users[a].CheckMemberGroups(groupIds).Request().PostAsync();
                    if (lstgroup.Count == 0)
                    {
                        await graphClient.Groups[azureuser.AzureGroupId].Members.References.Request().AddAsync(lst1.FirstOrDefault());// PutAsync("11f13986-bc28-4d78-b5a9-bfff687d479d");
                    }

                }
                else
                {
                    // var req = graphClient.Users.Request();
                    var req = new User
                    {
                        AccountEnabled = true,
                        DisplayName = azureuser.DisplayName,
                        MailNickname = azureuser.NickName,
                        UserPrincipalName = azureuser.EmailAddress,
                        PasswordProfile = new PasswordProfile
                        {
                            ForceChangePasswordNextSignIn = true,
                            Password = azureuser.Password
                        }
                    };
                    User user2 = await graphClient.Users
                                         .Request()
                                         .AddAsync(req);
                    string id = user2.Id;
                    await graphClient.Groups[azureuser.AzureGroupId].Members.References.Request().AddAsync(user2);// PutAsync("11f13986-bc28-4d78-b5a9-bfff687d479d");

                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            //await graphClient.Groups.Members[azureuser.AzureObjectId].Reference.Request().DeleteAsync();
            return Ok();
        }

        public async Task<IActionResult> AddGuestUsertoAzureGroup(AzureUserList azureuser)
        {
            GraphServiceClient graphClient;
            graphClient = MicrosoftGraphClient.GetGraphServiceClient();

            try
            {
                var req2 = new Invitation
                {
                    InvitedUserEmailAddress = azureuser.EmailAddress,
                    InvitedUserDisplayName = azureuser.DisplayName,
                    SendInvitationMessage = false,
                    InviteRedirectUrl = "http://test.com"
                };

                //var lst1 = await graphClient.Users
                //                .Request().Filter($"userprincipalname eq '{azureuser.EmailAddress}'").GetAsync();

                var lst2 = await graphClient.Users
                                  .Request().Filter($"displayname eq '{azureuser.DisplayName}'").GetAsync();
                User objuser = null;
                if (lst2.CurrentPage.Count > 0)
                {
                    var groupIds = new List<String>()
{
    azureuser.AzureGroupId
};
                    objuser = lst2.CurrentPage[0];
                    var lstgroup = await graphClient.Users[objuser.Id].CheckMemberGroups(groupIds).Request().PostAsync();
                    if (lstgroup.Count == 0)
                    {
                        await graphClient.Groups[azureuser.AzureGroupId].Members.References.Request().AddAsync(objuser);
                    }
                }
                else
                {
                    Invitation user3 = await graphClient.Invitations
                                          .Request()
                                          .AddAsync(req2);

                    objuser = (User)user3.InvitedUser;

                    await graphClient.Groups[azureuser.AzureGroupId].Members.References.Request().AddAsync(objuser);// PutAsync("11f13986-bc28-4d78-b5a9-bfff687d479d");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            //await graphClient.Groups.Members[azureuser.AzureObjectId].Reference.Request().DeleteAsync();
            return Ok();
        }
    }
}
