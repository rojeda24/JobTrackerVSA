using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace JobTrackerVSA.Web.Infrastructure.Storage;

public interface IResumeStorageService
{
    Task<string> UploadResumeAsync(
        Stream fileStream, 
        string fileName, 
        string contentType, 
        CancellationToken cancellationToken = default);
    Task DeleteResumeAsync(string resumeUrl, CancellationToken cancellationToken = default);
    string GetSecureResumeUrl(string resumeUrl, int expireInMinutes = 1);
}

public class BlobStorageSettings
{
    public required string ConnectionString { get; set; }
    public required string ContainerName { get; set; }
}

public class AzureBlobResumeStorageService : IResumeStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly ILogger<AzureBlobResumeStorageService> _logger;

    public AzureBlobResumeStorageService(BlobServiceClient blobServiceClient, IOptions<BlobStorageSettings> options, ILogger<AzureBlobResumeStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = options.Value.ContainerName;
        _logger = logger;
    }

    public async Task<string> UploadResumeAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Ensure the container exists. In production, you might want to handle this differently (e.g., create it manually or during app initialization with a singleton pattern).
        // TODO: Check security implications of PublicAccessType.None and whether we need to set specific access policies.
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(fileName);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
        };

        await blobClient.UploadAsync(fileStream, options, cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task DeleteResumeAsync(string resumeUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(resumeUrl)) return;

        try
        {
            var uri = new Uri(resumeUrl);
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(Path.GetFileName(uri.LocalPath));
            await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }
        catch (UriFormatException ex)
        {
            _logger.LogWarning(ex, "Failed to parse resume URL '{ResumeUrl}' when attempting to delete the blob.", resumeUrl);
        }
    }

    public string GetSecureResumeUrl(string resumeUrl, int expireInMinutes = 1)
    {
        if (string.IsNullOrEmpty(resumeUrl)) return string.Empty;

        try
        {
            var uri = new Uri(resumeUrl);
            var blobName = Path.GetFileName(uri.LocalPath);
            
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            // Check if we can generate a SAS Token
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS Token builder
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobClient.BlobContainerName,
                    BlobName = blobClient.Name,
                    Resource = "b", // 'b' stands for Blob
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1), // Prevent clock skew issues
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expireInMinutes)
                };

                // Grant Read-only permissions
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                // Generate the full URI with the appended SAS Token
                var sasUri = blobClient.GenerateSasUri(sasBuilder);
                return sasUri.ToString();
            }
            
            // If SAS generation fails (e.g., using a non-delegated Managed Identity), return base URL
            return blobClient.Uri.ToString();
        }
        catch (UriFormatException)
        {
            return string.Empty; // Ignore invalid URIs
        }
    }
}
