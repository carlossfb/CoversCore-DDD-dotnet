using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CoversFunctionApp.src.application.dto;
using CoversFunctionApp.src.domain.ports;

namespace CoversFunctionApp.src.infraestructure
{
    public class AzureBlobStorageImpl : IStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public AzureBlobStorageImpl()
        {
            _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage") 
                ?? throw new InvalidOperationException("AzureWebJobsStorage environment variable not configured");
            _containerName = Environment.GetEnvironmentVariable("BlobContainerName") 
                ?? throw new InvalidOperationException("BlobContainerName environment variable not configured");
        }
        public Task<ResponseStorageDTO> GetStorageInfo()
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // obtém informações da conta
            var blobUri = containerClient.Uri;
            var blobUriBuilder = new BlobUriBuilder(blobUri);

            // gera o SAS (válido por 2 horas, permissões de Create e Write)
            BlobSasBuilder sasBuilder = SetSasBuilderConfigs("c",2);

            // cria o token com a SharedKey
            var storageAccountName = blobUriBuilder.Host.Split('.')[0]; // exemplo: "conta.blob.core.windows.net"
            var accountKey = GetAccountKeyFromConnectionString(_connectionString);

            var credential = new StorageSharedKeyCredential(storageAccountName, accountKey);
            var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();

            var fullUrl = $"{blobUri}?{sasToken}";

            return Task.FromResult(new ResponseStorageDTO(fullUrl));

        }


        public async Task UploadAsync(string fileName, Stream fileStream)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, overwrite: true);
        }


        private BlobSasBuilder SetSasBuilderConfigs(string permissions, int hours)
        {
            if (hours < 1 || hours > 24)
                throw new ArgumentException("Hours must be between 1 and 24");
           
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                Resource = permissions, // container-level SAS
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(hours)
            };

            sasBuilder.SetPermissions(BlobContainerSasPermissions.Create | BlobContainerSasPermissions.Write);
            return sasBuilder;
        }

        private static string GetAccountKeyFromConnectionString(string connectionString)
        {
            var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                if (part.StartsWith("AccountKey=", StringComparison.OrdinalIgnoreCase))
                    return part.Substring("AccountKey=".Length);
            }
            throw new InvalidOperationException("AccountKey not found in connection string");
        }
    }
}