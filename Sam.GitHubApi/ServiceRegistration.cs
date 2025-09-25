using Microsoft.Extensions.DependencyInjection;
using Sam.GitHubApi.Internal;
using System;

namespace Sam.GitHubApi
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddGitHubApi(this IServiceCollection services)
        {
            return services.AddGitHubApi(new GitHubApiOptions());
        }
        public static IServiceCollection AddGitHubApi(this IServiceCollection services, Action<GitHubApiOptions> configureOptions)
        {
            var options = new GitHubApiOptions();
            configureOptions(options);

            return services.AddGitHubApi(options);
        }
        public static IServiceCollection AddGitHubApi(this IServiceCollection services, GitHubApiOptions options)
        {
            services.AddSingleton(options);

            services.AddScoped<IGithubFileService, GithubFileService>();

            return services;
        }
    }
    public class GitHubApiOptions
    {
        public string Token { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Repo { get; set; } = string.Empty;
    }
}
