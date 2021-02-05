using System.Security.Claims;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace AdvancedAnalysisDesign.Services
{
    public class SignInService
    {
        private readonly CookieAuthenticationOptions _cookieAuthenticationOptions;
        private readonly IHostEnvironmentAuthenticationStateProvider _hostAuthentication;
        private readonly IJSRuntime _jsRuntime;
        private readonly SignInManager<User> _signInManager;
        
        public SignInService(IOptionsMonitor<CookieAuthenticationOptions> cookieAuthenticationOptionsMonitor,
            IHostEnvironmentAuthenticationStateProvider hostAuthentication,
            IJSRuntime jsRuntime,
            SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _hostAuthentication = hostAuthentication;
            _jsRuntime = jsRuntime;
            _cookieAuthenticationOptions = cookieAuthenticationOptionsMonitor.Get("MyScheme");
        }
        
        public async Task SignInAsync(User user)
        {
            var principal = await _signInManager.CreateUserPrincipalAsync(user);

            var identity = new ClaimsIdentity(
                principal.Claims,
                "MyScheme"
            );
            principal = new ClaimsPrincipal(identity);
            _signInManager.Context.User = principal;
            _hostAuthentication.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));

            // this is where we create a ticket, encrypt it, and invoke a JS method to save the cookie
            var ticket = new AuthenticationTicket(principal, null, "MyScheme");
            var value = _cookieAuthenticationOptions.TicketDataFormat.Protect(ticket);
            await _jsRuntime.InvokeVoidAsync("blazorExtensions.WriteCookie", "BinaryBeastAuth", value, _cookieAuthenticationOptions.ExpireTimeSpan.TotalDays);
        }
        
        public async Task SignOutAsync()
        {
            var principal = _signInManager.Context.User = new ClaimsPrincipal(new ClaimsIdentity());
            _hostAuthentication.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));

            await _jsRuntime.InvokeVoidAsync("blazorExtensions.DeleteCookie", "BinaryBeastAuth");
            await Task.CompletedTask;
        }

        public async Task ResetUserLoginAsync(User user)
        {
            await SignOutAsync();
            await SignInAsync(user);
        }
    }
}