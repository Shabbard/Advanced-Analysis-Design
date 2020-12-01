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
        
        public async Task<User> GetUserAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.Users.Where(x => x.Username == "adam").FirstOrDefaultAsync();
            }
        } 
    }
}