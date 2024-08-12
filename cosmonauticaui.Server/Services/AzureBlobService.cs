using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace cosmonauticaui.Server.Services
{
	public class AzureBlobService
	{
		string _storageAccount = "cosmosnauticastorage";
		BlobServiceClient _blobClient;
		BlobContainerClient _containerClient;

		public AzureBlobService()
		{
			var uri = new Uri($"https://{_storageAccount}.blob.core.windows.net");
			_blobClient = new(uri, new DefaultAzureCredential());
			_containerClient = _blobClient.GetBlobContainerClient("documents");
		}

		public async Task<Response<BlobContentInfo>> UploadAsync(IFormFile file)
		{
			using (var stream = file.OpenReadStream())
			{
				return await _containerClient.UploadBlobAsync(file.FileName, stream);
			}
		}
	}
}
