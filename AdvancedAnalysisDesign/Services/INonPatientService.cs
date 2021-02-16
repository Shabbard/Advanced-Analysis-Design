using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.Payloads;

namespace AdvancedAnalysisDesign.Services
{
    public interface INonPatientService
    {
        Task AddMedicalInstitution(AddMedicalInstitutionPayload medicalInstitutionPayload);

        Task<MedicalInstitution> GetMedicalInstitutionForUser();

        Task RegisterPharmacist(NonPatientRegistrationPayload registrationPayload);

        Task RegisterGp(NonPatientRegistrationPayload registrationPayload);
    }
}