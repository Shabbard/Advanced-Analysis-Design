using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AdvancedAnalysisDesign.Services
{
    internal interface IScopedProcessingService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
    
    internal class ScopedProcessingService : IScopedProcessingService
    {
        private readonly NonPatientService _nonPatientService;
        private readonly AADContext _context;
        private readonly EmailService _emailService;
    
        public ScopedProcessingService(NonPatientService nonPatientService, AADContext context, EmailService emailService)
        {
            _nonPatientService = nonPatientService;
            _context = context;
            _emailService = emailService;
        }
        
        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                await CheckIfBloodworkIsRequired();
                await CheckForPatientPickupEmailReminder();
                await Task.Delay(86400000, stoppingToken);
            }
        }

        public async Task CheckForPatientPickupEmailReminder()
        {
            var patientMedications = await _context.PatientMedications.Include(x => x.Patient)
                .ThenInclude(x => x.User).ThenInclude(x => x.UserDetail)
                .Where(x => x.Pickup.DateScheduled == null)
                .ToListAsync();
            
            foreach (var patientMedication in patientMedications)
            {
                if (patientMedication.DateOfMedicationLastPickedUp.AddDays(patientMedication.DayIntervalOfBloodworkRenewal) == DateTimeOffset.Now.AddDays(-7))
                {
                    var emailMessage =  $"Hi {patientMedication.Patient.User.UserDetail.FirstName} {patientMedication.Patient.User.UserDetail.LastName}. \n\n" + 
                                        "You have a medication that is due in one week but has not had a pickup scheduled. \n" + 
                                        "Please log in to our system to arrange pickup for your medication. \n" + 
                                        "Have a nice day.\n" +
                                        "Binary Beast Bloodwork";
        
                    await _emailService.SendEmailAsync(
                        patientMedication.Patient.User.Email,
                        "Prescription Pickup Reminder",
                        emailMessage);
                }
            }
        }

        public async Task CheckIfBloodworkIsRequired()
        {
            var patientMedications = await _context.PatientMedications.Include(x => x.Patient)
                    .ThenInclude(x => x.User).ThenInclude(x => x.UserDetail).Include(x => x.Patient)
                    .ThenInclude(x => x.GeneralPractitioner).ThenInclude(x => x.User).ThenInclude(x => x.UserDetail)
                    .Include(x => x.PatientBloodworks).ThenInclude(x => x.PatientBloodworkTests)
                    .Where(x => x.PatientBloodworks.Any()).ToListAsync();
                
            foreach (var patientMedication in patientMedications)
            {
                foreach (var patientBloodwork in patientMedication.PatientBloodworks)
                {
                    foreach (var patientBloodworkTest in patientBloodwork.PatientBloodworkTests)
                    {
                        if (patientBloodworkTest.DateOfUpload.AddDays(patientMedication.DayIntervalOfBloodworkRenewal) == DateTimeOffset.Now.AddDays(-7))
                        {
                            var emailMessage =  $"Hi {patientMedication.Patient.GeneralPractitioner.User.UserDetail.FirstName} {patientMedication.Patient.GeneralPractitioner.User.UserDetail.LastName}. \n\n" + 
                                                "You have a patient who's bloodwork is due for renewal soon. \n" + 
                                                $"{patientMedication.Patient.User.UserDetail.FirstName} {patientMedication.Patient.User.UserDetail.LastName}'s {patientBloodwork.BloodworkTest.TestName} test will expire in one week and need renewing." +
                                                "Have a nice day.\n" +
                                                "Binary Beast Bloodwork";
        
                            await _emailService.SendEmailAsync(
                                patientMedication.Patient.GeneralPractitioner.User.Email,
                                "Bloodwork Reminder",
                                emailMessage);
                        }
                    }
                }
            }
        }
    }
}