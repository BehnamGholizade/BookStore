using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UploadController : Controller
    {
        private readonly IHostingEnvironment _environment;

        public UploadController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    var uploadDir = "images/covers/";
                    var uploadPath = Path.Combine(_environment.WebRootPath, uploadDir);
                    using (var fileStream = new FileStream(Path.Combine(uploadPath, file.FileName), FileMode.Create, FileAccess.Write))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return Ok(uploadDir + file.FileName);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return BadRequest("Error while uploading file");
        }

        [HttpPost]
        public IActionResult UploadFileAjax()
        {
            if (Request.Form.Files.Count > 0)
            {
                try
                {
                    var file = Request.Form.Files[0]; //first file only
                    if (file.Length > 0)
                    {
                        var uploadDir = "images/covers/";
                        var uploadPath = Path.Combine(_environment.WebRootPath, uploadDir);
                        using (var fileStream = new FileStream(Path.Combine(uploadPath, file.FileName), FileMode.Create, FileAccess.Write))
                        {
                            file.CopyTo(fileStream);
                        }
                        return Ok("/" + uploadDir + file.FileName);
                    }
                    else
                    {
                        return BadRequest("Zero length file!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return BadRequest("No files selected.");
            }
        }
    }
}
