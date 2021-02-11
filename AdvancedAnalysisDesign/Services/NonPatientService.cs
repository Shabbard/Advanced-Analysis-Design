using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Services
{
    public class NonPatientService
    {
        private readonly AADContext _context;
        public NonPatientService(AADContext context)
        {
            _context = context;
        }

        public async Task<List<MedicalInstitution>> GetSurgeries()
        {
            return await _context.Surgeries.ToListAsync();
        }
        
        public async Task<List<MedicalInstitution>> GetPharmacies()
        {
            return await _context.Pharmacies.ToListAsync();
        }
    }
}