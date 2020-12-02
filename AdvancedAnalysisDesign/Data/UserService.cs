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

        public async Task<UserDetail> RegisterUserDetails(UserDetail userDetail)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.UserDetails.AddAsync(userDetail);
                return userDetail;
            }
        }
        
        public async Task<User> RegisterUser(User user, UserDetail userDetails)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                user.UserDetail = userDetails;
                await context.Users.AddAsync(user);
                return user;
            }
        }

        public async Task RegisterPatient(UserDetail userDetailsToRegister, User userToRegister, Patient patientToRegister)
        {
            var userDetails = await RegisterUserDetails(userDetailsToRegister);
            var user = await RegisterUser(userToRegister, userDetails);

            patientToRegister.User = user;
            
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Patients.AddAsync(patientToRegister);
            }
        }
    }
}