using CUnity.Server.Data;
using CUnity.Server.Models;
using CUnity.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUnity.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDBContext _context;
        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            this._context = context;
        }
        //api/users/GetUsers
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var userList = _userManager.Users.AsQueryable();
            var userDtoList = new List<UserViewModel>(); // This sucks, but Select isn't async happy, and the passing into a 'ProcessEventAsync' is another level of misdirection
            foreach (var applicationUser in userList.ToList())
            {
                userDtoList.Add(new UserViewModel
                {
                    FirstName = applicationUser.FirstName,
                    LastName = applicationUser.LastName,
                    UserName = applicationUser.UserName,
                    Email = applicationUser.Email,
                    UserGuId = applicationUser.Id,
                    Roles = await _userManager.GetRolesAsync(applicationUser).ConfigureAwait(true) as List<string>
                });
            }
            return Ok(userDtoList);
        }
        //api/users/GetRoles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            // var roleList = await  _roleManager.Roles.ToListAsync();

            var res = await _context.Roles.ToListAsync();
            //var context = new ApplicationDBContext();
            return Ok(res);
        }
    }
}
