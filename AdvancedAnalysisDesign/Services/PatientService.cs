using System;
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
        private readonly Random _random;

        public PatientService(AADContext context,
            UserManager<User> userManager,
            UserService userService,
            Random random)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
            _random = random;
        }
        
        public async Task<byte[]> ConvertIBrowserFileToBytesAsync(IBrowserFile browserFile)
        {
            var maxByteSize = 10485760; // max of 10MB
            var buffer = new byte[browserFile.Size];
            await browserFile.OpenReadStream(maxByteSize).ReadAsync(buffer);
            return buffer;
        }
        
        public async Task RegisterPatient(PatientRegistrationPayload regPayload)
        {
            var user = await _userService.RegisterUser(regPayload);

            PatientImages images = new PatientImages
            {
                SelfiePhoto = await ConvertIBrowserFileToBytesAsync(regPayload.SelfiePhoto),
                IDPhoto = await ConvertIBrowserFileToBytesAsync(regPayload.IDPhoto),
                IsFlagged = false,
                IsVerified = false
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
        
        public PatientMedication CreateMedication(Medication medication, bool isBloodworkRequired)
        {
            return new()
            {
                Medication = medication,
                BloodworkRequired = isBloodworkRequired,
                Pickup = new() { IsPickedUp = false , IsPrepared = false , DateScheduled = null , DatePickedUp = null }
            };
        }

        public async Task PopulateFakeMedicationData()
        {
            var patients = await FetchAllPatientMedicationAndPickups();
            var medications = await FetchAllMedications();
            foreach(var patient in patients)
            {
                patient.Medications.Add(CreateMedication(medications[_random.Next(medications.Count)], true));
            }
            await _context.SaveChangesAsync();
        }
        
        public async Task<List<Patient>> FetchAllPatients()
        {
            return  await _context.Patients.Include(x => x.User).Include(x => x.User.UserDetail).Include(x => x.GeneralPractitioner).ToListAsync();
        }

        public async Task<List<Patient>> FetchAllPatientsWithPickups()
        {
            return await _context.Patients.Where(x => x.Medications.Any(y => y.Pickup.DateScheduled.HasValue)).Include(x => x.Medications.Where(x => x.Pickup.DateScheduled.HasValue)).ThenInclude(x => x.Pickup).ToListAsync();
        }

        public async Task<List<Patient>> FetchAllPatientMedicationAndPickups()
        {
            return await _context.Patients.Include(x => x.Medications).ThenInclude(x => x.Pickup).Include(x => x.Medications).ThenInclude(x=>x.Medication).Include(x => x.Medications).ToListAsync();
        }

        public async Task<List<Medication>> FetchAllMedications()
        {
            return await _context.Medications.ToListAsync();
        }

        public async Task<List<Patient>> FetchUserMedication()
        {
            var user = await _userService.GetCurrentUserAsync();
            return await _context.Patients.Where(y => y.User.Id.Equals(user.Id)).Include(x => x.Medications).ThenInclude(x => x.Pickup).Include(x => x.Medications).ThenInclude(x => x.Medication).Include(x => x.Medications).ToListAsync();
        }

        public async Task<List<Patient>> FetchUserBloodwork()
        {
            var user = await _userService.GetCurrentUserAsync();
            return await _context.Patients.Where(y => y.User.Id.Equals(user.Id)).Include(x => x.Medications).ThenInclude(x => x.PatientBloodworks).ThenInclude(x => x.PatientBloodworkTests).ToListAsync();
        }
    }
}