using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace cosmonauticaui.Server.Services
{
	public class AzureBlobService
	{
		string _accountName = "cosmosnauticastorage";
		BlobServiceClient _serviceClient;

		public AzureBlobService()
		{
			_serviceClient = GetServiceClient(_accountName);
		}

		BlobServiceClient GetServiceClient(string accountName)
		{
			BlobServiceClient client = new(
				new Uri($"https://{accountName}.blob.core.windows.net"),
				new DefaultAzureCredential());

			return client;
		}

		public BlobContainerClient GetContainerClient(string containerName)
		{
			BlobContainerClient client = _serviceClient.GetBlobContainerClient(containerName);
			return client;
		}

		public async Task<List<BlobItem>> GetAll(BlobContainerClient containerClient)
		{
			var items = new List<BlobItem>();
			var blobs = containerClient.GetBlobsAsync();
			await foreach (var blob in blobs)
			{
				items.Add(blob);
			}
			return items;
		}

		public async Task<Response<BlobContentInfo>> Upload(
			BlobContainerClient containerClient,
			IFormFile file, bool overwrite = false)
		{
			BlobClient blobClient = containerClient.GetBlobClient(file.FileName);
			using (var stream = file.OpenReadStream())
			{
				return await blobClient.UploadAsync(stream, overwrite: overwrite);
			}
		}

		public async Task<Response<BlobDownloadResult>> Download(
			BlobContainerClient containerClient,
			string fileName)
		{
			var blobClient = containerClient.GetBlobClient(fileName);
			var content = await blobClient.DownloadContentAsync();
			return content;
		}
	}
}
