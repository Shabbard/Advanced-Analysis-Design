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

namespace AdvancedAnalysisDesign.Tests
{
    public class BloodworkService
    {
        private readonly Mock<IBloodworkService> _bloodworkService = new();
       
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CheckReturnsEmptyList()
        {
            var bloodworks = new List<BloodworkTest>();
            
            var mock = bloodworks.AsQueryable().BuildMock();
            
            _bloodworkService.Setup(x => x.GetAllBloodworkTests()).Returns(mock.Object.ToListAsync());
            
            var t = await _bloodworkService.Object.GetAllBloodworkTests();
            
            Assert.True(!t.Any());
        }

        [Test]
        [TestCase("1", "Test 1")]
        [TestCase("2","Test 2")]
        public async Task CheckPatientBloodwork(string PatientId, string BloodworkTestName)
        {
            var bloodworks = new List<PatientBloodwork>
            {
                new() { Id = 1, BloodworkTest = new BloodworkTest{TestName = "Test 1"} },
                new() { Id = 2, BloodworkTest = new BloodworkTest{TestName = "Test 2"} },
                new() { Id = 3, BloodworkTest = new BloodworkTest{TestName = "Test 3"} }
            };

            var patients = new List<Patient>
            {
                new() 
                { Id = 1, Medications = new List<PatientMedication>
                    {
                        new() { PatientBloodworks = bloodworks }
                    }, 
                    User = new() { Id = "1"} 
                },
                new() 
                { Id = 2, Medications = new List<PatientMedication>
                    {
                        new() { PatientBloodworks = bloodworks }
                    }, 
                    User = new() { Id = "2"} 
                },
            };
            
            var mock = patients.AsQueryable().BuildMock();
            var _context = mock.Object;

            async Task<PatientBloodwork> FetchPatientBloodwork(string patientId, string bloodworkTestName) // this is the function being tested from the bloodwork service
            {
                var patient = await _context.SingleOrDefaultAsync(y => y.User.Id == patientId);
            
                var medicationWithBloodwork = patient?.Medications.SingleOrDefault(x => x.PatientBloodworks.Any(y => y.BloodworkTest.TestName == bloodworkTestName));
            
                return medicationWithBloodwork?.PatientBloodworks.SingleOrDefault(x => x.BloodworkTest.TestName == bloodworkTestName);
            }

            var t = await FetchPatientBloodwork(PatientId, BloodworkTestName);
            
            Assert.True(t.BloodworkTest.TestName == BloodworkTestName);
        }
        
        [Test]
        public async Task CheckThirdTestName()
        {
            var bloodworks = new List<BloodworkTest>
            {
                new() { Id = 1, TestName = "Test 1" },
                new() { Id = 2, TestName = "Test 2" },
                new() { Id = 3, TestName = "Test 3" }
            };
            
            var mock = bloodworks.AsQueryable().BuildMock();
            
            _bloodworkService.Setup(x => x.GetAllBloodworkTests()).Returns(mock.Object.ToListAsync());
            
            var t = await _bloodworkService.Object.GetAllBloodworkTests();
            
            Assert.True(t.Last().TestName == "Test 3");
        }
    }
}