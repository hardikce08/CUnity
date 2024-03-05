using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CUnity.Server.Data;
using CUnity.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace CUnity.Server
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        //public void SeedData(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration Configuration)
        //{
        //    SeedRoles(roleManager);
        //    SeedClient(Configuration);
        //    SeedUsers(userManager);
        //}
        public DatabaseInitializer(
              ApplicationDBContext context,
              UserManager<ApplicationUser> userManager,
              RoleManager<IdentityRole> roleManager,
              IConfiguration configuration
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        public virtual async Task SeedAsync()
        {
            await SeedRoles();
            await SeedClient();
            await SeedUsers();
        }
        public async Task SeedUsers()
        {
            if (_userManager.FindByNameAsync("superadmin").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.ClientId = 1;
                user.Email = "superadmin@cunity.com";
                user.FirstName = "SuperAdmin";
                user.UserId = _userManager.Users.ToList().Count > 0 ? _userManager.Users.ToList().Max(p => p.UserId) + 1 : 1;  // Set Custom UserId 
                user.UserName = "superadmin";
                IdentityResult result = _userManager.CreateAsync(user, "superadmin@123").Result;
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SuperAdmin");
                }
            }


            if (_userManager.FindByNameAsync("admin").Result == null)
            {
                ApplicationUser user = new ApplicationUser();
                user.ClientId = 1;
                user.Email = "admin@cunity.com";
                user.FirstName = "Administrator";
                user.UserId = _userManager.Users.ToList().Count > 0 ? _userManager.Users.ToList().Max(p => p.UserId) + 1 : 1;  // Set Custom UserId 
                user.UserName = "admin";
                IdentityResult result = _userManager.CreateAsync(user, "admin@123").Result;
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Administrator");
                }
            }
        }
        public async Task SeedClient()
        {
            var clientname = _configuration.GetSection("CUnity:ClientName").Get<string>() ?? new string("CURise");
            if (_context.Clients.Where(c => c.Name == clientname).FirstOrDefault() == null)
            {
                CUnity.Shared.Models.Client client = new Shared.Models.Client();
                client.Name = clientname;
                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();
            }
        }
        public async Task SeedRoles()
        {
            if (!_roleManager.RoleExistsAsync("SuperAdmin").Result)
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = "SuperAdmin";
                role.NormalizedName = role.Name.ToUpper();
                role.ConcurrencyStamp = DateTime.Now.ToString("yyyy-MM-dd");
                role.Id = _roleManager.Roles.ToList().Count > 0 ? (Convert.ToInt32(_roleManager.Roles.ToList().Max(p => p.Id)) + 1).ToString() : "1";  // Set Custom RoleId 
                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }
            if (!_roleManager.RoleExistsAsync("Administrator").Result)
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = "Administrator";
                role.NormalizedName = role.Name.ToUpper();
                role.ConcurrencyStamp = DateTime.Now.ToString("yyyy-MM-dd");
                role.Id = _roleManager.Roles.ToList().Count > 0 ? (Convert.ToInt32(_roleManager.Roles.ToList().Max(p => p.Id)) + 1).ToString() : "1";  // Set Custom RoleId
                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }
            if (!_roleManager.RoleExistsAsync("User").Result)
            {
                ApplicationRole role = new ApplicationRole();
                role.Name = "User";
                role.NormalizedName = role.Name.ToUpper();
                role.ConcurrencyStamp = DateTime.Now.ToString("yyyy-MM-dd");
                role.Id = _roleManager.Roles.ToList().Count > 0 ? (Convert.ToInt32(_roleManager.Roles.ToList().Max(p => p.Id)) + 1).ToString() : "1";  // Set Custom RoleId
                IdentityResult roleResult = await _roleManager.CreateAsync(role);
            }
        }
    }
}
