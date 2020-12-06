using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models;
using AdvancedAnalysisDesign.Models.Database;
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
            
            var user = new User
            {
                EmailAddress = regPayload.EmailAddress,
                Password = regPayload.Password,
                UserDetail = userDetails,
                UserType = regPayload.UserType
            };

            return user;
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
                await context.SaveChangesAsync();
            }
        }
    }
}