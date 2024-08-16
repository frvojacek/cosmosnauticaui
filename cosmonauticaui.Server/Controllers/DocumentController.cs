using cosmonauticaui.Server.Models;
using cosmonauticaui.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace cosmonauticaui.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		AzureBlobService _service;
		CosmosDBService _dbService;

		public DocumentController(AzureBlobService service, CosmosDBService dbService)
		{
			_service = service;
			_dbService = dbService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var files = await _service.GetAll();
			var fileNames = files.Select(f => f.Name).ToList();
			return Ok(fileNames);
		}

		[HttpGet("{fileName}")]
		public async Task<IActionResult> Get(string fileName)
		{
			var file = await _service.DownloadAsync(fileName);
			var byteArray = file.Value.Content.ToArray();
			return File(byteArray, "application/octet-stream", fileName);
		}

		[HttpPost]
		public async Task<IActionResult> Post(IFormCollection form)
		{
			var id = Guid.NewGuid();
			var name = form["name"];
			var file = form.Files[0];
			var fileName = Path.GetFileName(file.FileName);

			var document = new Document(
				id,
				name,
				fileName
			);

			await _service.UploadAsync(file);
			await _dbService.UploadDocument(document);

			return Ok(id);
		}
	}
}
