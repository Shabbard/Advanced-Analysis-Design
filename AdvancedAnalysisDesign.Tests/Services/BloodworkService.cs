using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using AdvancedAnalysisDesign.Models.ViewModels;
using AdvancedAnalysisDesign.Services;
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
            _bloodworkService.Setup(x => x.GetAllBloodworkTests())
                .ReturnsAsync(new List<BloodworkTest>());
            
            // _bloodworkService.Setup(x => x.FetchPatientBloodwork("1", "Test1"))
            //     .Returns<string,string>((patientId,bloodworkTestName) => );
        }

        [Test]
        public async Task CheckReturnsEmptyList()
        {
            var t = await _bloodworkService.Object.GetAllBloodworkTests();
            
            Assert.True(!t.Any());
        }
    }
}