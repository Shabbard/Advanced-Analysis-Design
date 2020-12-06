using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Data
{
    public class UserService
    {
        private readonly IDbContextFactory<AADContext> _contextFactory;
        
        public UserService(IDbContextFactory<AADContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task ForgotPasswordUpdate(ForgotPasswordPayload forgotPayload)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var result = await context.Users.SingleOrDefaultAsync(u => u.EmailAddress == forgotPayload.EmailAddress);

                if (result != null)
                {
                    result.Password = forgotPayload.Password;
                    context.SaveChanges();
                }
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
            
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.UserDetails.AddAsync(userDetail);
                return userDetail;
            }
        }
        
        public async Task<User> RegisterUser(RegistrationPayload regPayload)
        {
            var userDetails = await RegisterUserDetails(regPayload);
            
            var user = new User
            {
                EmailAddress = regPayload.EmailAddress,
                Password = regPayload.Password,
                UserDetail = userDetails,
                UserType = regPayload.UserType
            };
            
            using (var context = _contextFactory.CreateDbContext())
            {
                user.UserDetail = userDetails;
                await context.Users.AddAsync(user);
                return user;
            }
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

            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Patients.AddAsync(patient);
            }
        }

    }
}