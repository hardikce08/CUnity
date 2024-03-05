using CUnity.Server.Models;
using CUnity.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CUnity.Server.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<ListConfiguration> ListConfiguration { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }
        public DbSet<CUnity.Shared.Models.Client> Clients { get; set; }
        public DbSet<MenuInfo> MenuInfo { get; set; }
        public DbSet<ProductMaster> ProductMaster { get; set; }
        public DbSet<ClientSubscription> ClientSubscriptions { get; set; }
        public DbSet<AzureSetting> AzureSettings { get; set; }
        public DbSet<PowerBIReport> PowerBIReports { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<CUnity.Shared.Models.AzureClientGroup> AzureClientGroups { get; set; }

    }
}
