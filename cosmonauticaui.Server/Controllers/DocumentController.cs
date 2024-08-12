using cosmonauticaui.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace cosmonauticaui.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		AzureBlobService _service;

		public DocumentController(AzureBlobService service)
		{
			_service = service;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var files = await _service.GetAll();
			var fileNames = files.Select(f => f.Name).ToList();
			return Ok(fileNames);
		}

		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> Post(IFormFile file)
		{
			await _service.UploadAsync(file);
			return Ok();
		}
	}
}
