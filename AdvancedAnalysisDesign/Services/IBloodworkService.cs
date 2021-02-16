using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.ViewModels;

namespace AdvancedAnalysisDesign.Services
{
    public interface IBloodworkService
    {
        Task<List<BloodworkTest>> GetAllBloodworkTests();
        
        Task<List<BloodworkTest>> ReturnTestSearch(string userInput);

        Task<PatientBloodwork> FetchPatientBloodwork(string patientId, string bloodworkTestName);

        Task AddPatientBloodwork(PatientMedicationViewModel medicationViewModel);
    }
}