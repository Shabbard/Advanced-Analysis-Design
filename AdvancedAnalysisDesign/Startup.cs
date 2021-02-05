using System;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Services;
using BlazorDownloadFile;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;

namespace AdvancedAnalysisDesign
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

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<UserService>();
            services.AddSingleton<EmailService>();
            services.AddScoped<SignInService>();
            services.AddMudBlazorDialog();
            services.AddBlazoredLocalStorage();
            services.AddBlazorDownloadFile();
            services.AddMudBlazorSnackbar(config =>
            {
                config.PositionClass = Defaults.Classes.Position.BottomLeft;

                config.PreventDuplicates = false;
                config.NewestOnTop = false;
                config.ShowCloseIcon = true;
                config.VisibleStateDuration = 10000;
                config.HideTransitionDuration = 500;
                config.ShowTransitionDuration = 500;
            });
            services.AddMudBlazorResizeListener();
            
            var builder = new SqlConnectionStringBuilder(
                Configuration.GetConnectionString("AADDatabase"));

            builder.Password = Configuration["DbPassword"];
            
            services.AddDbContext<AADContext>(options =>
                options.UseSqlServer(builder.ConnectionString));
            
            services.AddScoped<IHostEnvironmentAuthenticationStateProvider>(sp => {
                var provider = (ServerAuthenticationStateProvider) sp.GetRequiredService<AuthenticationStateProvider>();
                return provider;
            });
            services.AddScoped<PasswordHasher<User>>();
            
            services.AddIdentityCore<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AADContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "MyScheme";
            }).AddCookie("MyScheme", options =>
            {
                options.Cookie.Name = "BinaryBeastAuth";
            });
            
            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
            
            CreateRoles(serviceProvider).Wait();
        }
        
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<User>>();
            IdentityResult roleResult;

            foreach(Role roleName in Enum.GetValues(typeof(Role)))
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName.ToString());
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName.ToString()));
                }
            }

            //Here you could create a super user who will maintain the web app
            var poweruser = new User
            {
                UserName = "admin",
                Email = "admin@admin",
                EmailConfirmed = true,
            };
            
            //Ensure you have these values in your appsettings.json file
            string userPWD = "P@ssword123";
            var _user = await UserManager.FindByEmailAsync("admin@admin");

            if(_user == null)
            {
                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);
                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, Role.Admin.ToString());
                }
            }
        }
    }
}
