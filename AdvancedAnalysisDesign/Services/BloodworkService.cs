using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.DataTransferObjects;
using AdvancedAnalysisDesign.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Services
{
    public class BloodworkService
    {
        private readonly AADContext _context;
        
        public BloodworkService(AADContext context)
        {
            _context = context;
        }
        
        public async Task<List<BloodworkTest>> GetAllBloodworkTests()
        {
            return await _context.BloodworkTests.ToListAsync();
        }
        
        public async Task<List<BloodworkTest>> ReturnTestSearch(string userInput)
        {
            return await _context.BloodworkTests.Where(test => test.TestName.Contains(userInput)).ToListAsync();
        }

        public async Task<PatientBloodwork> FetchPatientBloodwork(string patientId, string bloodworkTestName)
        {
            var patient = await _context.Patients.Include(x => x.Medications).SingleOrDefaultAsync(y => y.User.Id == patientId);
            
            var medicationWithBloodwork = patient.Medications.SingleOrDefault(x => x.PatientBloodworks.Any(y => y.BloodworkTest.TestName == bloodworkTestName));
            
            return medicationWithBloodwork?.PatientBloodworks.SingleOrDefault(x => x.BloodworkTest.TestName == bloodworkTestName);
        }

        // public List<PatientMedicationViewModel> ConvertPatientMedicationsToViewModel(List<PatientMedication> patientMedicationsList)
        // {
        //     return patientMedicationsList;
        // }

        public async Task AddPatientBloodwork(PatientMedicationViewModel medicationViewModel)
        {
            var patientMedication = await _context.PatientMedications.Include(x => x.PatientBloodworks).ThenInclude(x => x.PatientBloodworkTests).SingleOrDefaultAsync(x => x.Id == medicationViewModel.Id);
            
            if (patientMedication.PatientBloodworks == null)
                patientMedication.PatientBloodworks = new List<PatientBloodwork>();
            
            var bloodwork = patientMedication.PatientBloodworks.SingleOrDefault(x => x.BloodworkTest.TestName.Equals(medicationViewModel.BloodworkTest));
            
            if (bloodwork == null)
            {
                var bloodworkFromDb = await _context.BloodworkTests.SingleOrDefaultAsync(x => x.TestName == medicationViewModel.BloodworkTest);
                bloodwork = new PatientBloodwork
                {
                    BloodworkTest = bloodworkFromDb,
                    PatientBloodworkTests = new List<PatientBloodworkTest>()
                };
                patientMedication.PatientBloodworks.Add(bloodwork);
            }
            
            var bloodworkTest = new PatientBloodworkTest
            {
                Result = medicationViewModel.ResultInput,
                DateOfUpload = DateTimeOffset.Now
            };
            
            bloodwork.PatientBloodworkTests.Add(bloodworkTest);
            
            await _context.SaveChangesAsync();
        }
    }
}