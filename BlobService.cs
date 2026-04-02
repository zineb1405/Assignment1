using Azure.Storage.Blobs;

public class BlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobService(IConfiguration config)
    {
        _connectionString = config["AzureBlobStorageConnectionString"];
        _containerName = config["AzureBlobStorageContainerName"];
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var containerClient = new BlobContainerClient(_connectionString, _containerName);

        var blobClient = containerClient.GetBlobClient(file.FileName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, overwrite: true);
        }

        return blobClient.Uri.ToString();
    }
}