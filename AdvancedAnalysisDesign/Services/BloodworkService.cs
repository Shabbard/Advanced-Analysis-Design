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

        public async Task<List<PatientMedicationView>> ConvertPatientMedicationsToViewModel(List<PatientMedication> patientMedicationsList)
        {
            return patientMedicationsList.Select(x => new PatientMedicationView
            {
                Medication = x.Medication,
                Pickup = x.Pickup,
                BloodworkRequired = x.BloodworkRequired,
                PatientBloodworks = x.PatientBloodworks.Select(y => new PatientBloodworkView
                {
                    BloodworkTest = y.BloodworkTest,
                    PatientMedication = y.PatientMedication,
                    PatientBloodworkTests = y.PatientBloodworkTests.Select(z => new PatientBloodworkTestView
                    {
                        Result = z.Result,
                        PatientBloodwork = z.PatientBloodwork,
                        TestType = z.TestType,
                        DateOfUpload = z.DateOfUpload
                    }).ToList()
                }).ToList(),
                DateIntervalOfBloodworkRenewal = x.DateIntervalOfBloodworkRenewal,
                Patient = x.Patient
            }).ToList();
        }

        public async Task AddPatientBloodwork(PatientMedicationView medicationView)
        {
            // if ( == null)
            // {
            //     var newPatientMedication = new PatientMedication();
            //     newPatientMedication.PatientBloodworks = new List<PatientBloodwork>();
            //     await _context.PatientMedications.AddAsync(newPatientMedication);
            //     await _context.SaveChangesAsync();
            // }

            // var patientMedication = await _context.PatientMedications.SingleOrDefaultAsync(x => x.Id == medicationView.Id);
            //
            // if (patientMedication.PatientBloodworks == null)
            //     patientMedication.PatientBloodworks = new List<PatientBloodwork>();
            //
            //
            // var bloodwork = patientMedication.PatientBloodworks.SingleOrDefault(x => x.BloodworkTest.TestName == medicationView.BloodworkTest);
            //
            // if (bloodwork == null)
            // {
            //     var bloodworkFromDb = await _context.BloodworkTests.SingleOrDefaultAsync(x => x.TestName == medicationView.BloodworkTest);
            //     bloodwork = new PatientBloodwork
            //     {
            //         BloodworkTest = bloodworkFromDb,
            //         PatientBloodworkTests = new List<PatientBloodworkTest>()
            //     };
            //     patientMedication.PatientBloodworks.Add(bloodwork);
            //     // await _context.SaveChangesAsync();
            // }
            //
            // var bloodworkTest = new PatientBloodworkTest
            // {
            //     Result = medicationView.ResultInput,
            //     DateOfUpload = DateTimeOffset.Now
            // };
            //
            // bloodwork.PatientBloodworkTests.Add(bloodworkTest);
            //
            // await _context.SaveChangesAsync();
        }
    }
}