using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

        public FilesController(
            FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider
                ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
        }
        [HttpGet("{fileId}")]
        public ActionResult GetFile(string fileId)
        {
            // FileContentResult
            // FileStreamResult
            // VirtualFileResult
            var pathFile = "sales.csv";

            // check whether the file exists
            if (!System.IO.File.Exists(pathFile))
            {
                return NotFound();
            }

            if (!_fileExtensionContentTypeProvider.TryGetContentType(pathFile, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            
            var bytes = System.IO.File.ReadAllBytes(pathFile);
            return File(bytes, contentType, Path.GetFileName(pathFile));
        }
        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            // Validate the input. Put limit on filesize to avoid large uploads attacks.
            // Only accept .pdf files (check content-type)
            if (file.Length == 0 || file.Length > 20971520
                || file.ContentType != "application/pdf")
            {
                return BadRequest("No file or an invalid one has been inputted.");
            }

            // Creat the file path. Avoid using file. FileName, as an attacker can provide a malicous one, including full paths or relative paths.
            var path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"uploaded_file_{Guid.NewGuid()}.pdf");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("Your file has been uploaded succesfully.");
        }
    }
}
