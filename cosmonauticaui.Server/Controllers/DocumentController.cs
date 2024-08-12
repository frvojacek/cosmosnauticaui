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

		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> Post(IFormFile file)
		{
			await _service.UploadAsync(file);
			return Ok();
		}
	}
}
