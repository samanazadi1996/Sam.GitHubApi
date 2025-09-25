using System.Threading.Tasks;

namespace Sam.GitHubApi
{
    public interface IGithubFileService
    {
        Task<string?> GetFileContentAsync(string filePath);
        Task<bool> CreateOrUpdateFileAsync(string filePath, string content, string commitMessage);
        Task<bool> DeleteFileAsync(string filePath, string commitMessage);
    }
}