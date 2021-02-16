using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.Payloads;
using AdvancedAnalysisDesign.Models.ViewModels;
using Microsoft.AspNetCore.Components.Forms;

namespace AdvancedAnalysisDesign.Services
{
    public interface IPatientService
    {
        Task<byte[]> ConvertIBrowserFileToBytesAsync(IBrowserFile browserFile);

        Task ApprovePatientImagesAsync(Patient patient);

        Task RegisterPatient(PatientRegistrationPayload regPayload);
        
        (int,int,int) ReturnPrescriptionCounters(List<Patient> patients);

        List<PickupSchedulerViewModel> ReturnPickupScheduler(List<Patient> patients);
        
        PatientMedication CreateMedication(Medication medication, bool isBloodworkRequired,DateTimeOffset dateOfStart, double pickupInterval, double bloodInterval);

        Task PopulateFakeMedicationData();
        
        Task<List<Patient>> FetchAllPatients();

        Task<List<Patient>> FetchAllPatientsWithPickups();

        Task<List<Patient>> FetchAllPatientMedicationAndPickups();

        Task<List<Patient>> FetchAllPatientsForVerification();

        Task<List<Patient>> FetchAllPatientsPrescriptions();

        Task<List<Patient>> GetAllMedicationsforInstitution(MedicalInstitution medicalInstitution);

        Task<List<Medication>> FetchAllMedications();

        Task<List<PatientMedication>> FetchPatientMedicationsFromUserId(string userId);
        
        Task<Patient> FetchUserMedication();

        Task<PatientMedication> FetchUserBloodwork(int MedId);

        Task<List<Pickup>> FetchCurrentSchedule(String Institution);

        Task UpdatePickup(Pickup pickup, DateTime? dateScheduled, MedicalInstitution institution);

        Task DeletePickup(Pickup pickup);
    }
}