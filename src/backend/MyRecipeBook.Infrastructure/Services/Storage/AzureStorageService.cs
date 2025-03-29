using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using MyRecipeBook.Domain.Entities;
using MyRecipeBook.Domain.Services.Storage;
using MyRecipeBook.Domain.ValueObjects;

namespace MyRecipeBook.Infrastructure.Services.Storage
{
    public class AzureStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task Delete(User user, string filename)
        {
            var containerName = user.UserIdentifier.ToString();

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var exist = await containerClient.ExistsAsync();
            if (exist.Value)
            {
                await containerClient.DeleteBlobIfExistsAsync(filename);
            }
        }

        public async Task DeleteContainer(Guid userIdentifier)
        {
            var containerName = userIdentifier.ToString();

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteIfExistsAsync();
        }

        public async Task<string> GetFileUrl(User user, string filename)
        {
            var containerName = user.UserIdentifier.ToString();

            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var exist = await containerClient.ExistsAsync();
            if (!exist.Value)
            {
                return string.Empty;
            }

            var blobClient = containerClient.GetBlobClient(filename);
            exist = await blobClient.ExistsAsync();
            if (exist.Value)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = containerName,
                    BlobName = filename,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(MyRecipeBookRuleConstants.MAXIMUM_IMAGE_URL_LIFETIME_IN_MINUTES)
                };

                sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);

                return blobClient.GenerateSasUri(sasBuilder).ToString();
            }

            return string.Empty;
        }

        public async Task Upload(User user, Stream file, string filename)
        {
            var container = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
            await container.CreateIfNotExistsAsync();

            var blobclient = container.GetBlobClient(filename);

            await blobclient.UploadAsync(file, overwrite: true);
        }
    }
}
