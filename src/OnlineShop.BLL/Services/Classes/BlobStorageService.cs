using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using OnlineShop.BLL.Services.Interfaces;

namespace OnlineShop.BLL.Services.Classes;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
        _containerName = configuration["AzureBlobStorage:ContainerName"];
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);

        var blobClient = containerClient.GetBlobClient(uniqueFileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "image/jpeg" });

        return uniqueFileName;
    }

    public async Task DeleteFileAsync(string blobFileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        var blobClient = containerClient.GetBlobClient(blobFileName);

        if (await blobClient.ExistsAsync())
        {
            await blobClient.DeleteAsync();
        }
        else
        {
            throw new Exception("File not found");
        }
    }

    public string GetImageSasUri(string blobFileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var blobClient = containerClient.GetBlobClient(blobFileName);

        // Check if the file exists and set the stub name if the file is not found
        if (!blobClient.Exists())
        {
            blobFileName = "ImageNotFound.png";
            blobClient = containerClient.GetBlobClient(blobFileName);
        }

        return GenerateSasUri(blobClient);
    }

    private string GenerateSasUri(BlobClient blobClient)
    {
        BlobSasBuilder sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTime.UtcNow.AddMinutes(30)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}
