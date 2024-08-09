using Microsoft.AspNetCore.Mvc;

namespace cosmonauticaui.Server.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController : ControllerBase
	{
		[HttpPost]
		[Consumes("multipart/form-data")]
		public IActionResult Post(IFormFile file)
		{
			return Ok(file);
		}
	}
}
