using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
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
        
        public async Task<List<BloodworkTest>> ReturnTestSearch(string userinput)
        {
            return await _context.BloodworkTests.Where(test => test.TestName.Contains(userinput)).ToListAsync();
        }

        public async Task<PatientBloodwork> FetchPatientBloodwork(int patientID, string bloodworkTestName)
        {
            var patientMedications = await _context.PatientMedications.Include(x => x.Patient).Where(y => y.Patient.Id == patientID).ToListAsync();

            var medicationWithBloodwork = patientMedications.SingleOrDefault(x => x.PatientBloodworks.Any(y => y.BloodworkTest.TestName == bloodworkTestName));

            return medicationWithBloodwork.PatientBloodworks.SingleOrDefault(x => x.BloodworkTest.TestName == bloodworkTestName);
        }
    }
}