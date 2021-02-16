using System.Collections.Generic;
using System.Threading.Tasks;
using AdvancedAnalysisDesign.Models.Database;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;

namespace AdvancedAnalysisDesign.Services
{
    public interface IFaceService
    {
        // IFaceClient AuthenticationFace();


        Task<(bool, bool)> VerifyTwoImageFaceAsync(PatientImages images);

        // Task<List<DetectedFace>> DetectedFacesAsync(IFaceClient faceClient, byte[] image);
    }
}