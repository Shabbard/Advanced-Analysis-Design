using System;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Pages;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Services
{
    public class UserService
    {
        private readonly AADContext _context;
        
        public UserService(AADContext context)
        {
            _context = context;
        }

        public async Task ForgotPasswordUpdate(ForgotPasswordPayload forgotPayload)
        {
            var result = await _context.Users.SingleOrDefaultAsync(u => u.EmailAddress == forgotPayload.EmailAddress);

            if (result != null)
            {
                result.Password = forgotPayload.Password;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserDetail> RegisterUserDetails(RegistrationPayload regPayload)
        {
            var userDetail = new UserDetail
            {
                FirstName = regPayload.FirstName,
                LastName = regPayload.LastName,
                DateOfBirth = (System.DateTimeOffset)regPayload.DateOfBirth,
                PhoneNumber = regPayload.PhoneNumber
            };
            
            return userDetail;
        }
        
        public async Task<User> RegisterUser(RegistrationPayload regPayload)
        {
            var userDetails = await RegisterUserDetails(regPayload);
            
            if (_context.Users.Where(u => u.EmailAddress == regPayload.EmailAddress).Any())
            {
                throw new ApplicationException("Email address is already in use!");
            }
            
            var user = new User
            {
                EmailAddress = regPayload.EmailAddress,
                Password = BCrypt.Net.BCrypt.HashPassword(regPayload.Password),
                UserDetail = userDetails,
                UserType = regPayload.UserType
            };

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

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == email);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.Password);
        }
    }
}