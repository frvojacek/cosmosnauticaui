using Azure;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

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

		public async Task<List<BlobItem>> GetBlobs(BlobContainerClient containerClient)
		{
			var itemList = new List<BlobItem>();
			var blobs = containerClient.GetBlobsAsync();
			await foreach (var blob in blobs)
			{
				itemList.Add(blob);
			}
			return itemList;
		}

		public async Task<Response<BlobContentInfo>> UploadBlob(
			BlobContainerClient containerClient,
			IFormFile file,
			string fileName,
			bool overwrite = false)
		{
			BlobClient blobClient = containerClient.GetBlobClient(fileName);
			using (var stream = file.OpenReadStream())
			{
				return await blobClient.UploadAsync(stream, overwrite: overwrite);
			}
		}

		public async Task<Response<BlobDownloadResult>> DownloadBlob(
			BlobContainerClient containerClient,
			string fileName)
		{
			var blobClient = containerClient.GetBlobClient(fileName);
			var result = await blobClient.DownloadContentAsync();
			return result;
		}
	}
}
