using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.Payloads;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace AdvancedAnalysisDesign.Services
{
    public class PatientService
    {
        private readonly AADContext _context;
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly Random _random;
        private readonly FaceService _faceService;
        private readonly EmailService _emailService;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackbar _snackbar;

        public PatientService(AADContext context,
            UserManager<User> userManager,
            UserService userService,
            Random random,
            FaceService faceService,
            EmailService emailService,
            NavigationManager navigationManager,
            ISnackbar snackbar)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
            _random = random;
            _faceService = faceService;
            _emailService = emailService;
            _navigationManager = navigationManager;
            _snackbar = snackbar;
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
                IDPhoto = await ConvertIBrowserFileToBytesAsync(regPayload.IDPhoto)
            };
            (images.IsFlagged, images.IsVerified) = await _faceService.VerifyTwoImageFaceAsync(images);

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
            if (images.IsFlagged)
            {
                var subject = "Image verification failed";
                var emailMessage = $"Hi {user.UserDetail.FirstName} {user.UserDetail.LastName}. \n\n" +
                                   "Our automatic image verification system has flagged your uploaded images.\n" +
                                   "A manual review is now in progress. Please be aware that this may take up to 3 working days.\n" +
                                   "In the meantime please confirm your account.\n\n" +
                                   "Have a nice day.\n" +
                                   "Binary Beast Bloodwork";
                await _emailService.SendEmailAsync(user.Email, subject, emailMessage);
                _snackbar.Add("Automatic image verification failed. Please review the email sent to you.", Severity.Warning, config => { config.ShowCloseIcon = false; });
                _navigationManager.NavigateTo("/", true);
            }
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

        public async Task<List<Patient>> FetchAllPatientsForVerification()
        {
            return await _context.Patients.Include(x => x.PatientImages).Where(x => x.PatientImages.IsFlagged.Equals(false)).ToListAsync();
        }

        public async Task<List<Patient>> FetchAllPatientsPrescriptions()
        {
            return await _context.Patients.Include(x => x.Medications.Where(x => x.Pickup.DateScheduled.HasValue)).ThenInclude(x => x.Pickup).Include(x=> x.User.UserDetail).Include(x => x.Medications).ThenInclude(x => x.Medication).ToListAsync();
        }

        public async Task<List<Patient>> GetAllMedicationsforInstitution(MedicalInstitution medicalInstitution)
        {
            return await _context.Patients.Include(x => x.Medications.Where(x => x.Pickup.DateScheduled.HasValue).Where(x => x.Pickup.MedicalInstitution.Id == medicalInstitution.Id)).ThenInclude(x => x.Pickup).Include(x => x.User.UserDetail).Include(x => x.Medications).ThenInclude(x => x.Medication).ToListAsync();
        }

        public async Task<List<Medication>> FetchAllMedications()
        {
            return await _context.Medications.ToListAsync();
        }

        public async Task<Patient> FetchUserMedication()
        {
            var user = await _userService.GetCurrentUserAsync();
            return await _context.Patients.Include(x => x.Medications).ThenInclude(x => x.Pickup).Include(x => x.Medications).ThenInclude(x => x.Medication).SingleOrDefaultAsync(x => x.User.Id == user.Id);
        }

        public async Task<PatientMedication> FetchUserBloodwork(int MedId)
        {
            return await _context.PatientMedications.Include(x => x.PatientBloodworks).SingleOrDefaultAsync(x => x.Id == MedId);
        }
    }
}