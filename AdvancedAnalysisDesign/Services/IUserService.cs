using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.DataTransferObjects;
using AdvancedAnalysisDesign.Models.Payloads;

namespace AdvancedAnalysisDesign.Services
{
    public interface IUserService
    {
        UserDetail RegisterUserDetails(RegistrationPayload regPayload);
        
        Task<User> RegisterUser(RegistrationPayload regPayload);

        Task Login(string email, string password);
        
        Task Logout();
        
        Task<User> GetCurrentUserAsync();

        Task<string> GetCurrentUserRoleAsync();

        Task UpdatePasswordAsync(UpdatePasswordPayload updatePasswordPayload);

        Task UpdatePhoneNumberAsync(string phoneNumber);

        Task UpdateEmailAddressAsync(string newEmail);

        Task DownloadPersonalDataAsync();

        Task DeletePersonalDataAsync(string password);

        Task DeleteSpecificUserAsync(User user);

        Task SubmitForgetPasswordAsync(string emailAddress);

        Task SendConfirmationEmail(string userEmail);

        Task<List<UserWithRoleDto>> FetchAllUsers(MedicalInstitution medicalInstitution = null);
        
        Task<List<User>> GetUsersForInstitution(MedicalInstitution medicalInstitution);
    }
}