using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models;
using AdvancedAnalysisDesign.Models.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using MudBlazor;

namespace AdvancedAnalysisDesign.Services
{
    public class UserService
    {
        private readonly AADContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly ISnackbar _snackbar;
        private readonly NavigationManager _navigationManager;
        private readonly CookieAuthenticationOptions _cookieAuthenticationOptions;
        private readonly IHostEnvironmentAuthenticationStateProvider _hostAuthentication;
        private readonly IJSRuntime _jsRuntime;
        private readonly PasswordHasher<User> _passwordHasher;
        
        public UserService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            AADContext context,
            ISnackbar snackbar,
            NavigationManager navigationManager,
            IOptionsMonitor<CookieAuthenticationOptions> cookieAuthenticationOptionsMonitor,
            IHostEnvironmentAuthenticationStateProvider hostAuthentication,
            IJSRuntime jsRuntime,
            PasswordHasher<User> passwordHasher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _snackbar = snackbar;
            _navigationManager = navigationManager;
            _hostAuthentication = hostAuthentication;
            _jsRuntime = jsRuntime;
            _cookieAuthenticationOptions = cookieAuthenticationOptionsMonitor.Get("MyScheme");
            _passwordHasher = passwordHasher;
        }

        public async Task ForgotPasswordUpdate(ForgotPasswordPayload forgotPayload)
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.Email == forgotPayload.EmailAddress);

            if (result != null)
            {
                result.PasswordHash = forgotPayload.Password;
                await _context.SaveChangesAsync();
            }
        }

        public UserDetail RegisterUserDetails(RegistrationPayload regPayload)
        {
            var userDetail = new UserDetail
            {
                FirstName = regPayload.FirstName,
                LastName = regPayload.LastName,
                DateOfBirth = (System.DateTimeOffset)regPayload.DateOfBirth,
            };
            
            return userDetail;
        }
        
        public async Task<User> RegisterUser(RegistrationPayload regPayload)
        {
            var userDetails = RegisterUserDetails(regPayload);
            
            if (_context.Users.Where(u => u.Email == regPayload.EmailAddress).Any())
            {
                throw new ApplicationException("Email address is already in use!");
            }
            
            var user = new User
            {
                UserDetail = userDetails,
                Email = regPayload.EmailAddress,
                UserName = regPayload.EmailAddress,
                PhoneNumber = regPayload.PhoneNumber,
                EmailConfirmed = true //TODO: Remove this later
            };
            
            var result = await _userManager.CreateAsync(user, regPayload.Password);

            return user; // Depending other user types are implemented, this may need changing. Currently standalone users cannot be registered.
        }

        public async Task RegisterPatient(RegistrationPayload regPayload)
        {
            var user = await RegisterUser(regPayload);
            
            var patient = new Patient
            {
                User = user,
                NhsNumber = regPayload.NhsNumber,
                VerificationImage = regPayload.VerificationImage
            };

            await _userManager.AddToRoleAsync(user, Role.Patient.ToString());

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task Login(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
            {
                _snackbar.Add("Login was unsuccessful. Please try again.", Severity.Error, config => { config.ShowCloseIcon = false; });
                return;
            }

            await SignInAsync(user);
        
            _snackbar.Add("Login successful!", Severity.Success, config => { config.ShowCloseIcon = false; });
            _navigationManager.NavigateTo("/");
        }
        
        public async Task Logout()
        {
            await SignOutAsync();
        
            _snackbar.Add("Logout successful!", Severity.Success, config => { config.ShowCloseIcon = false; });
            _navigationManager.NavigateTo("/");
        }
        
        private async Task SignInAsync(User user)
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
        
        private async Task SignOutAsync()
        {
            var principal = _signInManager.Context.User = new ClaimsPrincipal(new ClaimsIdentity());
            _hostAuthentication.SetAuthenticationState(Task.FromResult(new AuthenticationState(principal)));

            await _jsRuntime.InvokeVoidAsync("blazorExtensions.DeleteCookie", "BinaryBeastAuth");

            await Task.CompletedTask;
        }

        public async Task<List<BloodworkTest>>ReturnTestSearch(string userinput)
        {
            return await _context.BloodworkTests.Where(test => test.TestName.Contains(userinput)).ToListAsync();
        }
    }
}