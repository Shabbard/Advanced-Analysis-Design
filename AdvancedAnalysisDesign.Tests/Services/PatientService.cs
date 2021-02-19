using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.ViewModels;
using AdvancedAnalysisDesign.Services;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace AdvancedAnalysisDesign.Tests.Services
{
    public class PatientService
    {
        private readonly Mock<IPatientService> _patientService = new();
        [SetUp]
        public void SetupAllPatients()
        {
            _patientService.Setup(x => x.FetchAllPatients())
                .ReturnsAsync(new List<Patient>());
        }

        [Test]

        public async Task CheckPatientListReturnsEmpty()
        {
            var t = await _patientService.Object.FetchAllPatients();

            Assert.True(!t.Any());
        }

        [SetUp]
        public void SetupAllPatientsWithPickups()
        {
            _patientService.Setup(x => x.FetchAllPatientsWithPickups())
                .ReturnsAsync(new List<Patient>());
        }

        [Test]

        public async Task CheckPatientWithPickupsListReturnsEmpty()
        {
            var t = await _patientService.Object.FetchAllPatientsWithPickups();

            Assert.True(!t.Any());
        }

        [SetUp]
        public void SetupAllPatientsMedicationAndPickups()
        {
            _patientService.Setup(x => x.FetchAllPatientMedicationAndPickups())
                .ReturnsAsync(new List<Patient>());
        }

        [Test]

        public async Task CheckPatientMeciationAndPickupsListReturnsEmpty()
        {
            var t = await _patientService.Object.FetchAllPatientMedicationAndPickups();

            Assert.True(!t.Any());
        }

        [SetUp]
        public void SetupAllPatientsForVerification()
        {
            _patientService.Setup(x => x.FetchAllPatientsForVerification())
                .ReturnsAsync(new List<Patient>());
        }

        [Test]

        public async Task CheckPatientForVerificationListReturnsEmpty()
        {
            var t = await _patientService.Object.FetchAllPatientsForVerification();

            Assert.True(!t.Any());
        }

        [SetUp]
        public void SetupAllPatientsPrescriptions()
        {
            _patientService.Setup(x => x.FetchAllPatientsPrescriptions())
                .ReturnsAsync(new List<Patient>());
        }

        [Test]

        public async Task CheckPatientPrescriptionsListReturnsEmpty()
        {
            var t = await _patientService.Object.FetchAllPatientsPrescriptions();

            Assert.True(!t.Any());
        }

        [SetUp]
        public void SetupAllMedication()
        {
            _patientService.Setup(x => x.FetchAllMedications())
                .ReturnsAsync(new List<Medication>());
        }

        [Test]

        public async Task CheckUserMedicationReturnsEmpty()
        {
            var t = await _patientService.Object.FetchAllMedications();

            Assert.True(!t.Any());
        }
    }
}
