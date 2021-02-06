using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.Payloads;
using BlazorDownloadFile;
using Blazored.LocalStorage;
using Flurl;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor;

namespace AdvancedAnalysisDesign.Services
{
    public class UserService
    {
        private readonly AADContext _context;
        private readonly EmailService _emailService;
        private readonly SignInService _signInService;
        private readonly UserManager<User> _userManager;
        private readonly ISnackbar _snackbar;
        private readonly NavigationManager _navigationManager;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IBlazorDownloadFileService _blazorDownloadFileService;
        
        public UserService(UserManager<User> userManager,
            AADContext context,
            EmailService emailService,
            SignInService signInService,
            ISnackbar snackbar,
            NavigationManager navigationManager,
            PasswordHasher<User> passwordHasher,
            AuthenticationStateProvider authenticationStateProvider,
            IBlazorDownloadFileService blazorDownloadFileService)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _signInService = signInService;
            _snackbar = snackbar;
            _navigationManager = navigationManager;
            _passwordHasher = passwordHasher;
            _authenticationStateProvider = authenticationStateProvider;
            _blazorDownloadFileService = blazorDownloadFileService;
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
            
            var user = new User
            {
                UserDetail = userDetails,
                Email = regPayload.EmailAddress,
                UserName = regPayload.EmailAddress,
                PhoneNumber = regPayload.PhoneNumber,
                EmailConfirmed = true //TODO: Remove this later
            };
            
            var result = await _userManager.CreateAsync(user, regPayload.Password);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }
            }

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

            await _signInService.SignInAsync(user);

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

            await _signInService.SignInAsync(user);
        
            _snackbar.Add("Login successful!", Severity.Success, config => { config.ShowCloseIcon = false; });
            _navigationManager.NavigateTo("/");
        }
        
        public async Task Logout()
        {
            await _signInService.SignOutAsync();
        
            _snackbar.Add("Logout successful!", Severity.Success, config => { config.ShowCloseIcon = false; });
            _navigationManager.NavigateTo("/");
        }
        
        public async Task<User> GetCurrentUserAsync()
        {
            var authenticationState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var authenticationStateUser = authenticationState.User;
            
            var user = await _userManager.FindByNameAsync(authenticationStateUser.Identity.Name);
            user = await _context.Users.Include(x => x.UserDetail).SingleOrDefaultAsync(x => x == user);

            return user;
        }

        public async Task UpdatePasswordAsync(UpdatePasswordPayload updatePasswordPayload)
        {
            var user = await GetCurrentUserAsync();
            
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, updatePasswordPayload.CurrentPassword, updatePasswordPayload.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    throw new Exception(error.Description);
                }
            }
            await _signInService.ResetUserLoginAsync(user);
        }

        public async Task UpdatePhonenumberAsync(string phonenumber)
        {
            var user = await GetCurrentUserAsync();

            var currentPhonenumber = await _userManager.GetPhoneNumberAsync(user);

            if (currentPhonenumber != phonenumber)
            {
                var changePasswordResult = await _userManager.SetPhoneNumberAsync(user, phonenumber);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        throw new Exception(error.Description);
                    }
                }
                await _signInService.ResetUserLoginAsync(user);
            }
        }
        public async Task UpdateEmailaddressAsync(string newEmail)
        {
            if (string.IsNullOrEmpty(newEmail))
            {
                throw new Exception("Please enter an email address.");
            }
            
            var user = await GetCurrentUserAsync();
            var currentEmail = await _userManager.GetEmailAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            
            var code = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            
            Url confirmationUrl = _navigationManager.BaseUri.AppendPathSegments("ConfirmEmail", userId, newEmail, code);
            
            var emailMessage = $"Hi {user.UserDetail.FirstName} {user.UserDetail.LastName}. \n\n" + 
                               "You have requested an email address change, to confirm this please follow the url. \n\n" + 
                               $"Please confirm your account by {HtmlEncoder.Default.Encode(confirmationUrl)}\n\n" +
                               "If you did not request this change, please disregard this email.\n\n" +
                               "Have a nice day.\n" +
                               "Binary Beast Bloodwork";
            
            await _emailService.SendEmailAsync(
                currentEmail,
                "Confirm your email address change",
                emailMessage);
        }

        public async Task DownloadPersonalDataAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                _snackbar.Add("User was not found.", Severity.Error, config => { config.ShowCloseIcon = false; });
                return;
            }

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var userPersonalDataProps = typeof(User).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            
            var userDetailsPersonalDataProps = typeof(UserDetail).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            
            foreach (var p in userPersonalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }
            foreach (var p in userDetailsPersonalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user.UserDetail)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            var y = new FileContentResult(JsonSerializer.SerializeToUtf8Bytes(personalData), "application/json");
            await _blazorDownloadFileService.DownloadFile("Personal Data", y.FileContents, TimeSpan.FromMinutes(1), contentType: "application/json");
        }

        public async Task DeletePersonalDataAsync(string password)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                throw new Exception("User was not found.");
            }

            var requirePassword = await _userManager.HasPasswordAsync(user);
            if (requirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, password))
                {
                    throw new Exception("Incorrect Password.");
                }
            }

            await DeleteUserAssociations(user);
                
            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            await _signInService.SignOutAsync();
        }

        private async Task DeleteUserAssociations(User user)
        {
            if (_context.Patients.Any(x => x.User == user))
            {
                _context.Patients.Remove(await _context.Patients.SingleOrDefaultAsync(x => x.User == user));
            }
            
            _context.UserDetails.Remove(user.UserDetail);
            
            await _context.SaveChangesAsync();
        }

        public async Task<List<Patient>> FetchAllPatients()
        {
            return  await _context.Patients.Include(x => x.User).Include(x => x.User.UserDetail).Include(x => x.GeneralPractitioner).ToListAsync();
        }

        public async Task<List<BloodworkTest>>ReturnTestSearch(string userinput)
        {
            return await _context.BloodworkTests.Where(test => test.TestName.Contains(userinput)).ToListAsync();
        }
    }
}