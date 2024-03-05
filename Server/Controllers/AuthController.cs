using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using CUnity.Server.Data;
using CUnity.Server.Models;
using CUnity.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDBContext _context;
        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,ApplicationDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._context = context;

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return BadRequest("User does not exist");
            var singInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!singInResult.Succeeded) return BadRequest("Invalid password");
            await _signInManager.SignInAsync(user, request.RememberMe);
            //string Id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return Ok();

        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest parameters)
        {
            try
            {
                var user = new ApplicationUser();
                user.UserName = parameters.UserName;
                user.Email = parameters.Email;
                user.FirstName = parameters.FirstName;
                user.LastName = parameters.LastName;
                user.UserId = _userManager.Users.ToList().Count > 0 ?  _userManager.Users.ToList().Max(p => p.UserId) + 1 : 1;  // Set Custom UserId 
                user.ClientId = parameters.ClientId;
                var result = await _userManager.CreateAsync(user, parameters.Password);
                if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
                
                if (parameters.RoleName != string.Empty)
                {
                    var res = await _userManager.AddToRoleAsync(user, parameters.RoleName);
                    if (!result.Succeeded) return BadRequest(result.Errors.FirstOrDefault()?.Description);
                }
                //// Add User Claims
                ////await _userManager.AddClaimsAsync(user,new[] { new Claim(ClaimTypes.NameIdentifier, user.UserName) });
                //if (!string.IsNullOrWhiteSpace(user.FirstName))
                //{
                //    await _userManager.AddClaimsAsync(user, new[] { new Claim(ClaimTypes.GivenName.ToString(), user.FirstName) });
                //}

                //if (!string.IsNullOrWhiteSpace(user.LastName))
                //{
                //    await _userManager.AddClaimsAsync(user, new[] { new Claim(ClaimTypes.Surname.ToString(), user.LastName) });
                //}

                //if (!string.IsNullOrWhiteSpace(user.Email))
                //{
                //    await _userManager.AddClaimsAsync(user, new[] { new Claim(ClaimTypes.Email.ToString(), user.Email) });
                //}
                //// Claim addition end
                //Enable below Code once you do Enabling REgistration from Frontend
                //return await Login(new LoginRequest
                //{
                //    UserName = parameters.UserName,
                //    Password = parameters.Password
                //});
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        [HttpGet]
        public async Task<CurrentUser> CurrentUserInfoAsync()
        {
            ApplicationUser user = null;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
            }
            return new CurrentUser
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                Claims = User.Claims
                .ToDictionary(c => c.Type, c => c.Value),
                UserId = user?.UserId,
                UserGuid = user?.Id,
                IsAdmin= User.IsInRole("Administrator"),
                IsSuperAdmin = User.IsInRole("SuperAdmin"),
                ClientId =user?.ClientId
            };
        }
    }
}