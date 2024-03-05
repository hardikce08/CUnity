using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using CUnity.Server.Data;
using Microsoft.EntityFrameworkCore;
using CUnity.Server.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using CUnity.Client.TagHelpers;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using MatBlazor;
using CUnity.Shared;
using CUnity.Server.Middleware;
using CUnity.Server.PowerBIServices;

namespace CUnity.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<ApplicationDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDBContext>();
            // services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            services.AddSingleton<MicrosoftGraphClient>();

            // Register AadService and PbiEmbedService for dependency injection
            services.AddScoped(typeof(AadService))
                    .AddScoped(typeof(PbiEmbedService));

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                //options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // Require Confirmed Email User settings
                //if (Convert.ToBoolean(Configuration[$"{projectName}:RequireConfirmedEmail"] ?? "false"))
                //{
                //    options.User.RequireUniqueEmail = true;
                //    options.SignIn.RequireConfirmedEmail = true;
                //}
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = false;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });
            services.AddControllers().AddNewtonsoftJson();
            services.AddControllersWithViews();
            //services.AddMatBlazor();
            services.AddRazorPages();
            services.AddLoadingBar();
            services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.BottomRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 3000;
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseExceptionHandler("/Error");
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //Make Default entry in DB when system runs first time 
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var _db = services.GetService<ApplicationDBContext>();
                var databaseInitializer = new DatabaseInitializer(_db, userManager, roleManager, Configuration);
                databaseInitializer.SeedAsync().Wait();
            }

            //app.UseLocalTimeZone();
            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            //Add our new middleware to the pipeline
            app.UseMiddleware<RequestResponseLoggingMiddleware>();
            // after using middlerware getting HTTP_1_1_Required error so to resolve that below code is added 
            app.Use(async (ctx, next) => { await next(); if (ctx.Response.StatusCode == 204) { ctx.Response.ContentLength = 0; } });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
