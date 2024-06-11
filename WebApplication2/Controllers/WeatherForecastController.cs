using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using System;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {


        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{filename}")]
        public void Get(string filename)
        {
            string bucketName = "sample143";
            string objectName = filename;
            string localPath = "D:\\gcp-downloads\\" + filename;
            try
            {
                if (ObjectExistsAsync(bucketName, objectName))
                {
                    DownloadFileAsync(bucketName, objectName, localPath);
                }
                else
                {
                    Console.WriteLine($"Object '{objectName}' does not exist in bucket '{bucketName}'.");
                }
            }
            catch (Google.GoogleApiException ex)
            {
                Console.WriteLine($"Google API Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

        }



 

        private static bool ObjectExistsAsync(string bucketName, string objectName)
        {
            var storage = StorageClient.Create();
            try
            {
                storage.GetObjectAsync(bucketName, objectName);
                return true;
            }
            catch (Google.GoogleApiException ex) when (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        private static void DownloadFileAsync(string bucketName, string objectName, string localPath)
        {
            var clientId = "764086051850-6qr4p6gpi6hn506pt8ejuq83di341hur.apps.googleusercontent.com";
            var clientSecret = "d-FL95Q19q7MQmFpd7hHD0Ty";
            var credentialParameters = new JsonCredentialParameters
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };
            credentialParameters.Type = "authorized_user";
            credentialParameters.RefreshToken = "1//0g00_SRUO7aPdCgYIARAAGBASNwF-L9IrBYVhsCI7RdqaT0rTiK-lYG4-7HBWC_V0OCSG29WiND3KcOoWH0LhZBYGDCfu72ye4Ws";
            GoogleCredential credential = GoogleCredential.FromJsonParameters(credentialParameters);

            var storage = StorageClient.Create(credential);
            using (var fileStream = System.IO.File.OpenWrite(localPath))
                storage.DownloadObjectAsync(bucketName, objectName, fileStream);
            Console.WriteLine($"Downloaded {objectName} to {localPath}.");
        }
    }
}


