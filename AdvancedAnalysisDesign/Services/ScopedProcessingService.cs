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
        Task CheckIfBloodworkIsRequired(CancellationToken stoppingToken);
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
        
        public async Task CheckIfBloodworkIsRequired(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                var patientMedications = await _context.PatientMedications.Include(x => x.Patient)
                    .ThenInclude(x => x.User).ThenInclude(x => x.UserDetail).Include(x => x.Patient)
                    .ThenInclude(x => x.GeneralPractitioner).ThenInclude(x => x.User).ThenInclude(x => x.UserDetail)
                    .Include(x => x.PatientBloodworks).ThenInclude(x => x.PatientBloodworkTests)
                    .Where(x => x.PatientBloodworks.Any()).ToListAsync(cancellationToken: stoppingToken);
                
                foreach (var patientMedication in patientMedications)
                {
                    foreach (var patientBloodwork in patientMedication.PatientBloodworks)
                    {
                        foreach (var patientBloodworkTest in patientBloodwork.PatientBloodworkTests)
                        {
                            if (patientBloodworkTest.DateOfUpload.AddDays(patientMedication.DayIntervalOfBloodworkRenewal) == DateTimeOffset.Now.AddDays(-7))
                            {
                                var emailMessage =  $"Hi {patientMedication.Patient.GeneralPractitioner.User.UserDetail.FirstName} {patientMedication.Patient.GeneralPractitioner.User.UserDetail.LastName}. \n\n" + 
                                                    "You have a patient who's bloodwork is due for renewal soon. \n\n" + 
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
                await Task.Delay(86400000, stoppingToken);
            }
        }
    }
}