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
		public async Task<IActionResult> Get()
		{
			var files = await _blobService.GetAll(_blobContainerClient);
			var fileNames = files.Select(f => f.Name).ToList();
			return Ok(fileNames);
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
			var id = Guid.NewGuid();
			var name = form["name"];
			string places = form["places"];
			string counterParties = form["counterParties"];
			string products = form["products"];
			var file = form.Files[0];
			var fileName = Path.GetFileName(file.FileName);

			var document = new Document(
				id,
				name,
				fileName,
				places.Split(", "),
				counterParties.Split(", "),
				products.Split(", ")
			);

			await _blobService.Upload(_blobContainerClient, file);
			await _cosmosService.UploadDocument(_cosmosContainer, document);

			return Ok(id);
		}
	}
}
