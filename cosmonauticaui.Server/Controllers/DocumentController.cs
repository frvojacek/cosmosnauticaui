using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using cosmonauticaui.Server.Models;
using cosmonauticaui.Server.Services;

namespace cosmonauticaui.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		AzureBlobService _blobService;
		BlobContainerClient _blobContainerClient;
		CosmosDBService _cosmosService;
		Container _cosmosContainer;

		public DocumentController(AzureBlobService blobService, CosmosDBService cosmosService)
		{
			_blobService = blobService;
			_blobContainerClient = _blobService.GetContainerClient("documents");

			_cosmosService = cosmosService;
			_cosmosContainer = _cosmosService.GetContainer("Documents", "Items");
		}

		[HttpGet]
		public async Task<IActionResult> Get(string? searchType, string? searchInput)
		{
			string query = "SELECT * FROM c";

			if (searchInput != null && searchType != null)
			{
				query += $" WHERE ARRAY_CONTAINS(c.{searchType}, '{searchInput}')";
			}

			var documents = await _cosmosService.Query<Document>(_cosmosContainer, query);
			return Ok(documents);
		}

		[HttpGet("{fileName}")]
		public async Task<IActionResult> Get(string fileName)
		{
			var file = await _blobService.Download(_blobContainerClient, fileName);
			var byteArray = file.Value.Content.ToArray();
			return File(byteArray, "application/octet-stream", fileName);
		}

		[HttpPost]
		public async Task<IActionResult> Post(IFormCollection form)
		{
			var file = form.Files[0];
			var uploadResponse = await _blobService.Upload(_blobContainerClient, file);

			Document document = (FormCollection)form;
			document.Version = uploadResponse.Value.VersionId;

			await _cosmosService.UploadDocument(_cosmosContainer, document);

			return Ok(document.id);
		}

		[HttpPut]
		public async Task<IActionResult> Put(IFormCollection form)
		{
			var file = form.Files[0];
			var uploadResponse = await _blobService.Upload(_blobContainerClient, file, true);

			Document document = (FormCollection)form;
			document.Version = uploadResponse.Value.VersionId;

			await _cosmosService.UploadDocument(_cosmosContainer, document);

			return Ok(document.id);
		}
	}
}
