using Microsoft.AspNetCore.Mvc;

namespace Sam.GitHubApi.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GithubController(IGithubFileService github) : ControllerBase
    {
        // 📖 Get file (with content + metadata)
        [HttpGet("file")]
        public async Task<IActionResult> GetFile([FromQuery] string path)
        {
            var file = await github.GetFileContentAsync(path);
            return file is not null ? Ok(file) : NotFound("File not found ❌");
        }

        // ✍️ Create or update file
        [HttpPost("update")]
        public async Task<IActionResult> UpdateFile([FromQuery] string path, [FromQuery] string content)
        {
            var result = await github.CreateOrUpdateFileAsync(path, content, $"Update via API at {DateTime.Now}");
            return result ? Ok("Updated ✅") : BadRequest("Failed ❌");
        }

        // ❌ Delete file
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteFile([FromQuery] string path)
        {
            var result = await github.DeleteFileAsync(path, "Delete via API");
            return result ? Ok("Deleted ✅") : BadRequest("Failed ❌");
        }
    }
}
