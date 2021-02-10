using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.Payloads;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Services
{
    public class PatientService
    {
        private readonly AADContext _context;
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;

        public PatientService(AADContext context,
            UserManager<User> userManager,
            UserService userService)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
        }
        
        public async Task<byte[]> ConvertIBrowserFileToBytesAsync(IBrowserFile browserFile)
        {
            var maxByteSize = 10485760;
            var buffer = new byte[browserFile.Size];
            await browserFile.OpenReadStream(maxByteSize).ReadAsync(buffer);
            return buffer;
        }
        
        public async Task RegisterPatient(RegistrationPayload regPayload)
        {
            var user = await _userService.RegisterUser(regPayload);

            PatientImages images = new PatientImages
            {
                SelfiePhoto = await ConvertIBrowserFileToBytesAsync(regPayload.SelfiePhoto),
                IDPhoto = await ConvertIBrowserFileToBytesAsync(regPayload.IDPhoto)
            };

            var patient = new Patient
            {
                User = user,
                NhsNumber = regPayload.NhsNumber,
                PatientImages = images
            };

            await _userManager.AddToRoleAsync(user, Role.Patient.ToString());

            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
            
#if RELEASE
            await _userService.SendConfirmationEmail(user.Email);
#endif
        }
        
        public async Task<List<Patient>> FetchAllPatients()
        {
            return  await _context.Patients.Include(x => x.User).Include(x => x.User.UserDetail).Include(x => x.GeneralPractitioner).ToListAsync();
        }

        public async Task<List<Patient>> FetchAllPatientsWithPickups()
        {
            return await _context.Patients.Include(x=> x.Medications).Include(x => x.Medications.Where(x => x.Pickup.DatePickedUp == null)).ThenInclude(x => x.Pickup).ToListAsync();
        }
        
        public (int,int,int) ReturnPrescriptionCounters(List<Patient> patients)
        {
            int prescriptionsDue = patients.Select(x => x.Medications.Count).Sum();
            int prescriptionsPrepared = patients.Select(x => x.Medications.Count(y => y.Pickup.IsPrepared)).Sum();
            int prescriptionsCollected = patients.Select(x => x.Medications.Count(y => y.Pickup.IsPickedUp)).Sum();

            return (prescriptionsDue, prescriptionsPrepared, prescriptionsCollected);
        }

        public List<PickupSchedulerPayload> ReturnPickupScheduler(List<Patient> patients)
        {
            var listOfPickups = patients.SelectMany(x => x.Medications.Select(y => y.Pickup)).ToList();
            return listOfPickups.Select(x => new PickupSchedulerPayload()
                {
                    StartTime = x.DateScheduled,
                    EndTime = x.DateScheduled?.AddMinutes(15), // every pickup will takeup a 15 minutes slot.
                    Subject = "Medication Pickup",
                    IsPickedUp = x.IsPickedUp,
                    IsPrepared = x.IsPrepared
                }
            ).ToList();
        }
    }
}