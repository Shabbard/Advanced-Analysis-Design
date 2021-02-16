using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using System.IO;
using AdvancedAnalysisDesign.Models.Database;

namespace AdvancedAnalysisDesign.Services
{

    public class FaceService : IFaceService
    {
        private readonly IConfiguration _configuration;

        public FaceService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private IFaceClient AuthenticationFace()
        {

            var endpoint = _configuration.GetConnectionString("AADFaceDetectionEndpoint");
            var endpointKey = _configuration["AADFaceDetectionKey"];

            return new FaceClient(new ApiKeyServiceClientCredentials(endpointKey)) { Endpoint = endpoint };
        }

        public async Task<(bool,bool)> VerifyTwoImageFaceAsync(PatientImages images)
        {

            IFaceClient faceClient = AuthenticationFace();

            List<DetectedFace> IdFaces = await DetectedFacesAsync(faceClient, images.IDPhoto);
            Guid IdFacesId = IdFaces[0].FaceId.Value;
            List<DetectedFace> SelfieFaces = await DetectedFacesAsync(faceClient, images.SelfiePhoto);
            Guid SelfieFacesId = SelfieFaces[0].FaceId.Value;

            VerifyResult result = await faceClient.Face.VerifyFaceToFaceAsync(IdFacesId, SelfieFacesId);
            return (!result.IsIdentical, result.IsIdentical);
        }

        private async Task<List<DetectedFace>> DetectedFacesAsync(IFaceClient faceClient, byte[] image)
        {
            IList<DetectedFace> detectedFaces;
            using (Stream stream = new MemoryStream(image))
            {
                detectedFaces = await faceClient.Face.DetectWithStreamAsync(stream);
            }
            return detectedFaces.ToList();
        }
    }
}
