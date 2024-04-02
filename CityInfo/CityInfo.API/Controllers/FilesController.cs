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
    }
}
