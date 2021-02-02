// using AdvancedAnalysisDesign.Models.Database;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Data.SqlClient;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
//
// [assembly: HostingStartup(typeof(AdvancedAnalysisDesign.Areas.Identity.IdentityHostingStartup))]
// namespace AdvancedAnalysisDesign.Areas.Identity
// {
//     public class IdentityHostingStartup : IHostingStartup
//     {
//         public void Configure(IWebHostBuilder builder)
//         {
//             builder.ConfigureServices((context, services) => {
//                 var connectionStringBuilder = new SqlConnectionStringBuilder(
//                     context.Configuration.GetConnectionString("AADDatabase"));
//
//                 connectionStringBuilder.Password = context.Configuration["DbPassword"];
//
//                 services.AddDbContext<AADContext>(options =>
//                     options.UseSqlServer(connectionStringBuilder.ConnectionString));
//
//                 services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
//                     .AddRoles<IdentityRole>()
//                     .AddEntityFrameworkStores<AADContext>();
//             });
//         }
//     }
// }