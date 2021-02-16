using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Enums;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.Payloads;
using AdvancedAnalysisDesign.Models.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace AdvancedAnalysisDesign.Services
{
    public class PatientService : IPatientService
    {
        private readonly AADContext _context;
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly Random _random;
        private readonly FaceService _faceService;
        private readonly EmailService _emailService;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackbar _snackbar;
        private readonly NonPatientService _nonPatientService;

        public PatientService(AADContext context,
            UserManager<User> userManager,
            UserService userService,
            Random random,
            FaceService faceService,
            EmailService emailService,
            NavigationManager navigationManager,
            ISnackbar snackbar,
            NonPatientService nonPatientService)
        {
            _context = context;
            _userManager = userManager;
            _userService = userService;
            _random = random;
            _faceService = faceService;
            _emailService = emailService;
            _navigationManager = navigationManager;
            _snackbar = snackbar;
            _nonPatientService = nonPatientService;
        }

        public async Task<byte[]> ConvertIBrowserFileToBytesAsync(IBrowserFile browserFile)
        {
            var maxByteSize = 10485760; // max of 10MB
            var buffer = new byte[browserFile.Size];
            await browserFile.OpenReadStream(maxByteSize).ReadAsync(buffer);
            return buffer;
        }

        public async Task ApprovePatientImagesAsync(Patient patient)
        {
            patient.PatientImages.IsFlagged = false;
            patient.PatientImages.IsVerified = true;
            await _context.SaveChangesAsync();
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
                PatientImages = images,
                MedicalInstitution = regPayload.MedicalInstitution
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

        public List<PickupSchedulerViewModel> ReturnPickupScheduler(List<Patient> patients)
        {
            var listOfPickups = patients.SelectMany(x => x.Medications.Select(y => y.Pickup)).ToList();
            return listOfPickups.Select(x => new PickupSchedulerViewModel()
                {
                    StartTime = x.DateScheduled,
                    EndTime = x.DateScheduled?.AddMinutes(15), // every pickup will takeup a 15 minutes slot.
                    Subject = "Medication Pickup",
                    IsPickedUp = x.IsPickedUp,
                    IsPrepared = x.IsPrepared
                }
            ).ToList();
        }
        
        public PatientMedication CreateMedication(Medication medication, bool isBloodworkRequired,DateTimeOffset dateOfStart, double pickupInterval, double bloodInterval)
        {
            return new()
            {
                Medication = medication,
                BloodworkRequired = isBloodworkRequired,
                Pickup = new() { IsPickedUp = false , IsPrepared = false , DateScheduled = null , DatePickedUp = null },
                DayIntervalOfPickup = pickupInterval,
                DateOfMedicationStart = dateOfStart,
                DayIntervalOfBloodworkRenewal = bloodInterval,
                DateOfMedicationLastPickedUp = dateOfStart
            };
        }

        public async Task PopulateFakeMedicationData()
        {
            var patients = await FetchAllPatientMedicationAndPickups();
            var medications = await FetchAllMedications();
            var dateNow = DateTimeOffset.Now;
            var dateInFuture = dateNow.AddMonths(_random.Next(1, 3));
            var timeInterval = (dateInFuture - dateNow).TotalDays;
            foreach(var patient in patients)
            {
                patient.Medications.Add(CreateMedication(medications[_random.Next(medications.Count)], true, dateNow, timeInterval,timeInterval));
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
            var role = await _userService.GetCurrentUserRoleAsync();
            
            if (role.ToLower() == "pharmacist")
            {
                var userInt = await _nonPatientService.GetMedicalInstitutionForUser();
                return await _context.Patients.Include(x => x.User).ThenInclude(x => x.UserDetail).Include(x => x.PatientImages).Where(x => x.PatientImages.IsFlagged.Equals(true) && x.MedicalInstitution == userInt).ToListAsync();
            }
            return await _context.Patients.Include(x => x.User).ThenInclude(x => x.UserDetail).Include(x => x.PatientImages).Where(x => x.PatientImages.IsFlagged.Equals(true)).ToListAsync();

        }

        public async Task<List<Patient>> FetchAllPatientsPrescriptions()
        {
            return await _context.Patients.Include(x => x.Medications.Where(x => x.Pickup.DateScheduled.HasValue)).ThenInclude(x => x.Pickup).Include(x=> x.User.UserDetail).Include(x => x.Medications).ThenInclude(x => x.Medication).ToListAsync();
        }

        public async Task<List<Patient>> GetAllMedicationsforInstitution(MedicalInstitution medicalInstitution)
        {
            return await _context.Patients.Include(x => x.Medications.Where(x => x.Pickup.DateScheduled.HasValue && x.Pickup.MedicalInstitution.Id == medicalInstitution.Id)).ThenInclude(x => x.Pickup).Include(x => x.User.UserDetail).Include(x => x.Medications).ThenInclude(x => x.Medication).ToListAsync();
        }

        public async Task<List<Medication>> FetchAllMedications()
        {
            return await _context.Medications.ToListAsync();
        }

        public async Task<List<PatientMedication>> FetchPatientMedicationsFromUserId(string userId)
        {
            return await _context.PatientMedications.Include(x => x.Medication).Include(x => x.PatientBloodworks).ThenInclude(x => x.PatientBloodworkTests).Where(x => x.Patient.User.Id == userId).ToListAsync();
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

        public async Task<List<Pickup>> FetchCurrentSchedule(String Institution)
        {
            return await _context.Pickups.Where(x => x.MedicalInstitution.Name == Institution).ToListAsync();
        }

        public async Task UpdatePickup(Pickup pickup, DateTime? dateScheduled, MedicalInstitution institution)
        {
            if (pickup.DateScheduled.HasValue)
            {
                throw new Exception("A pickup time has already been made for this medication.");
            }
            if (dateScheduled == null)
            {
                throw new Exception("A date has not been entered");
            }
            if (institution == null)
            {
                throw new Exception("An pharmacy has not been selected.");
            }
            if (pickup == null)
            {
                throw new Exception("An error occurred. Please try again.");
            }

            try
            {
                pickup.DateScheduled = dateScheduled;
                pickup.IsPrepared = false;
                pickup.DatePickedUp = null;
                pickup.MedicalInstitution = institution;
                pickup.IsPickedUp = false;

                await _context.SaveChangesAsync();
            }
            catch(Exception e)
            {
                throw new Exception("An error occurred. Please try again");
            }
        }

        public async Task DeletePickup(Pickup pickup)
        {
            try
            {
                pickup.DateScheduled = null;
                pickup.IsPrepared = false;
                pickup.DatePickedUp = null;
                pickup.MedicalInstitution = null;
                pickup.IsPickedUp = false;
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred. Please try again");
            }
        }
    }
}