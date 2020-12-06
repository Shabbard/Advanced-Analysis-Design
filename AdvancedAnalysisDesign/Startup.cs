using AdvancedAnalysisDesign.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddSingleton<UserService>();
            services.AddMudBlazorDialog();
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
                      
            services.AddDbContextFactory<AADContext>(options =>
                options.UseSqlServer(builder.ConnectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
