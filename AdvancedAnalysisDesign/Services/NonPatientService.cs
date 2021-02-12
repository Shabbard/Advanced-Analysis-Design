using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.Payloads;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Services
{
    public class NonPatientService
    {
        private readonly AADContext _context;
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        public NonPatientService(AADContext context,
            UserService userService,
            UserManager<User> userManager)
        {
            _context = context;
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<List<MedicalInstitution>> GetMedicalInstitutions()
        {
            return await _context.MedicalInstitutions.ToListAsync();
        }

        public async Task AddMedicalInstitution(AddMedicalInstitutionPayload medicalInstitutionPayload)
        {
            if (_context.MedicalInstitutions.Any(x => x.Name == medicalInstitutionPayload.Name))
            {
                throw new Exception("Name is already in use.");
            }
            if (_context.MedicalInstitutions.Any(x => x.Address == medicalInstitutionPayload.Address))
            {
                throw new Exception("Address is already in use.");
            }
            if (_context.MedicalInstitutions.Any(x => x.ContactNumber == medicalInstitutionPayload.ContactNumber))
            {
                throw new Exception("Contact number is already in use.");
            }

            await _context.MedicalInstitutions.AddAsync(new MedicalInstitution
            {
                Name = medicalInstitutionPayload.Name,
                Address = medicalInstitutionPayload.Address,
                ContactNumber = medicalInstitutionPayload.ContactNumber
            });

            await _context.SaveChangesAsync();
        }

        public async Task<MedicalInstitution> GetMedicalInstitutionForUser()
        {
            var user = await _userService.GetCurrentUserAsync();
            
            var pharmacist = await _context.Pharmacists.Include(x => x.Pharmacy).SingleOrDefaultAsync(x => x.User == user);
            var generalPractitioner = await _context.GeneralPractitioners.Include(x => x.Surgery).SingleOrDefaultAsync(x => x.User == user);
            
            return pharmacist?.Pharmacy ?? generalPractitioner?.Surgery;
        }

        public async Task RegisterPharmacist(NonPatientRegistrationPayload registrationPayload)
        {
            var user = await _userService.RegisterUser(registrationPayload);

            var pharmacist = new Pharmacist
            {
                Pharmacy = registrationPayload.MedicalInstitution,
                User = user
            };
            
            await _userManager.AddToRoleAsync(user, Role.Pharmacist.ToString());

            await _context.Pharmacists.AddAsync(pharmacist);
            await _context.SaveChangesAsync();
        }

        public async Task RegisterGp(NonPatientRegistrationPayload registrationPayload)
        {
            var user = await _userService.RegisterUser(registrationPayload);

            var generalPractitioner = new GeneralPractitioner
            {
                Surgery = registrationPayload.MedicalInstitution,
                User = user,
                EmergencyContact = registrationPayload.EmergencyContact,
                OfficeNumber = registrationPayload.OfficeNumber
            };
            
            await _userManager.AddToRoleAsync(user, Role.GP.ToString());

            await _context.GeneralPractitioners.AddAsync(generalPractitioner);
            await _context.SaveChangesAsync();
        }
    }
}