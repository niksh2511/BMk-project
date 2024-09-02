using Azure.Storage.Blobs;

using Microsoft.Extensions.Configuration;

using System.IO;
using System.Threading.Tasks;

namespace BMK.Domain.Domain
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(IConfiguration configuration)
        {
            _blobServiceClient = new BlobServiceClient(configuration["AzureBlobStorage:ConnectionString"]);
         
            _containerName = configuration.GetValue<string>("AzureBlobStorage:ContainerName");
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(fileStream, true);
            return blobClient.Uri.ToString();
        }
    }

    public interface IBlobService
    {
        Task<string> UploadFileAsync(Stream fileStream, string fileName);
    }
}
