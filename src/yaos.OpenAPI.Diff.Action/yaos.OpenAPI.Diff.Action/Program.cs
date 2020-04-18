using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Octokit.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using yaos.OpenAPI.Diff.Output;

namespace yaos.OpenAPI.Diff.Action
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 5 || args.Length != 6)
                throw new ArgumentException("Number of arguments does not match expected amount.");

            var token = args[0];
            if (!long.TryParse(args[1], out var repositoryId))
                throw new ArgumentException("Error casting type");
            if (!int.TryParse(args[2], out var prNumber))
                throw new ArgumentException("Error casting type");
            if (!Uri.TryCreate(args[3], UriKind.RelativeOrAbsolute, out var oldFile) && !oldFile.IsFile)
                throw new ArgumentException("Error casting type");
            if (!Uri.TryCreate(args[4], UriKind.RelativeOrAbsolute, out var newFile) && !newFile.IsFile)
                throw new ArgumentException("Error casting type");
            var addComment = false;
            if (args[5] != null && !bool.TryParse(args[5], out addComment))
                throw new ArgumentException("Error casting type");

            var serviceProvider = Startup.Build();
            var openAPICompare = serviceProvider.GetService<IOpenAPICompare>();
            var renderer = serviceProvider.GetService<IMarkdownRender>();

            string markdown;
            try
            {
                var openAPIDiff = openAPICompare.FromLocations(oldFile.LocalPath, newFile.LocalPath);
                markdown = renderer.Render(openAPIDiff);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            var identifier = $"<!-- [openapi-diff-action-{Path.GetFileNameWithoutExtension(oldFile.LocalPath)}] -->";
            var credentials = new InMemoryCredentialStore(new Credentials(token));
            var github = new GitHubClient(new ProductHeaderValue("openapi-diff-action"), credentials);

            var commentText = $"{identifier}\n{markdown}";

            try
            {
                if (!addComment)
                {
                    var comments = await github.Issue.Comment.GetAllForIssue(repositoryId, prNumber);
                    var existingComment = comments.FirstOrDefault(x => x.Body.Contains(identifier));
                    if (existingComment != null)
                        await github.Issue.Comment.Update(repositoryId, existingComment.Id, commentText);
                }
                else
                    await github.Issue.Comment.Create(repositoryId, prNumber, commentText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
