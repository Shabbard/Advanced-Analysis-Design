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
using AdvancedAnalysisDesign.Models.DataTransferObjects;
using AdvancedAnalysisDesign.Models.Payloads;
using AdvancedAnalysisDesign.Models.ViewModels;
using BlazorDownloadFile;
using Flurl;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace AdvancedAnalysisDesign.Services
{
    public class UserService : IUserService
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
#if DEBUG
                EmailConfirmed = true
#endif
            };
            
            var result = await _userManager.CreateAsync(user, regPayload.Password);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    throw new Exception(error.Description);
                }
            }

            return user;
        }

        public async Task Login(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

            if(user == null)
            {
                throw new Exception("Login was unsuccessful. Please try again.");
            }

            if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
            {
                throw new Exception("Login was unsuccessful. Please try again.");
            }

            var patient = await _context.Patients.Include(x => x.PatientImages).SingleOrDefaultAsync(x => x.User.Id == user.Id);
            if (patient != null && patient.PatientImages != null && patient.PatientImages.IsFlagged)
            {
                throw new Exception("Your account is under manual review. Please try again later.");
            }

            if (!user.EmailConfirmed)
            {
                throw new Exception("Account has not been confirmed.");
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

        public async Task<string> GetCurrentUserRoleAsync()
        {
            var user = await GetCurrentUserAsync();
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles[0];
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

        public async Task UpdatePhoneNumberAsync(string phoneNumber)
        {
            var user = await GetCurrentUserAsync();

            var currentPhoneNumber = await _userManager.GetPhoneNumberAsync(user);

            if (currentPhoneNumber != phoneNumber)
            {
                var changePasswordResult = await _userManager.SetPhoneNumberAsync(user, phoneNumber);
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
        public async Task UpdateEmailAddressAsync(string newEmail)
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
            newEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(newEmail));
            userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));
            
            Url confirmationUrl = _navigationManager.BaseUri.AppendPathSegments("ConfirmEmailChange", userId, newEmail, code);
            
            var emailMessage = $"Hi {user.UserDetail.FirstName} {user.UserDetail.LastName}. \n\n" + 
                               "You have requested an email address change, to confirm this please follow the url. \n\n" + 
                               $"{HtmlEncoder.Default.Encode(confirmationUrl)}\n\n" +
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
            
            if (!await _userManager.CheckPasswordAsync(user, password)) 
            {
                throw new Exception("Incorrect Password.");
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

        public async Task DeleteSpecificUserAsync(User user)
        {
            if (user == null)
            {
                throw new Exception("user could not be found.");
            }

            await DeleteUserAssociations(user);
            var result = await _userManager.DeleteAsync(user);
            var userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception($"Unexpected error occurred deleting user with ID '{userId}'.");
            }
        }

        public async Task SubmitForgetPasswordAsync(string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new Exception("Please enter an email address.");
            }
            
            var user = await _userManager.FindByEmailAsync(emailAddress);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return;

            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id));
            
            Url passwordResetUrl = _navigationManager.BaseUri.AppendPathSegments("PasswordReset", userId, code);
            
            var emailMessage = "You have requested a password reset, to reset your password please follow the url. \n\n" + 
                               $"{HtmlEncoder.Default.Encode(passwordResetUrl)}\n\n" +
                               "If you did not request this change, please disregard this email.\n\n" +
                               "Have a nice day.\n" +
                               "Binary Beast Bloodwork";
            
            await _emailService.SendEmailAsync(
                emailAddress,
                "Reset your password",
                emailMessage);
        }

        public async Task SendConfirmationEmail(string userEmail)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);
            var userId = await _userManager.GetUserIdAsync(user);
            
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));
            
            Url confirmationUrl = _navigationManager.BaseUri.AppendPathSegments("ConfirmAccount", userId, code);
            
            var emailMessage = $"Hi {user.UserDetail.FirstName} {user.UserDetail.LastName}. \n\n" + 
                               "You have created an account using our software, to confirm your account please follow the url. \n\n" + 
                               $"{HtmlEncoder.Default.Encode(confirmationUrl)}\n\n" +
                               "Have a nice day.\n" +
                               "Binary Beast Bloodwork";
            
            await _emailService.SendEmailAsync(
                userEmail,
                "Confirm your account",
                emailMessage);
        }

        public async Task<List<UserWithRoleDto>> FetchAllUsers(MedicalInstitution medicalInstitution = null)
        {
            // this monstrosity gets all users except the default admin, includes user details and includes the users role
            var users = _context.Users
                .Include(x => x.UserDetail)
                .Where(x => x.UserName != "admin")
                .Join(_context.UserRoles,
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new
                    {
                        User = user,
                        RoleId = userRole.RoleId
                    })
                .Join(_context.Roles,
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => new UserWithRoleDto
                    {
                        Role = Enum.Parse<Role>(role.Name),
                        User = userRole.User
                    });

            if (medicalInstitution == null)
            {
                return await users.ToListAsync();
            }

            var usersForInstitution = await GetUsersForInstitution(medicalInstitution);

            return await users.Where(x => usersForInstitution.Contains(x.User)).ToListAsync();
        }
        
        public async Task<List<User>> GetUsersForInstitution(MedicalInstitution medicalInstitution)
        {
            var pharmacists = _context.Pharmacists.Include(x => x.Pharmacy)
                .Where(x => x.Pharmacy == medicalInstitution).Select(x => x.User);

            var generalPractitioners = _context.GeneralPractitioners.Include(x => x.Surgery)
                .Where(x => x.Surgery == medicalInstitution).Select(x => x.User);
            
            return await pharmacists.Union(generalPractitioners).ToListAsync();
        }
    }
}