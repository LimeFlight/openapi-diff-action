﻿using Microsoft.Extensions.DependencyInjection;
using Octokit;
using Octokit.Internal;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using yaos.OpenAPI.Diff.Action.Utils;
using yaos.OpenAPI.Diff.Enums;
using yaos.OpenAPI.Diff.Extensions;
using yaos.OpenAPI.Diff.Output;

namespace yaos.OpenAPI.Diff.Action
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            if (args.Length != 7)
                throw new ArgumentException("Number of arguments does not match expected amount.");

            var token = args[0];
            var repository = args[1];
            var owner = repository.Split('/')[0];
            var repositoryName = repository.Split('/')[1];
            if (!int.TryParse(args[2], out var prNumber))
                throw new ArgumentException("Error casting type");
            if (!PathUtil.TryGetAbsoluteUri(args[3], out var oldFile) || !oldFile.IsFile)
                throw new ArgumentException("Error casting type");
            if (!PathUtil.TryGetAbsoluteUri(args[4], out var newFile) || !newFile.IsFile)
                throw new ArgumentException("Error casting type");
            var addComment = false;
            if (args.GetValue(5) != null && !bool.TryParse(args[5], out addComment))
                throw new ArgumentException("Error casting type");
            var excludeLabels = false;
            if (args.GetValue(6) != null && !bool.TryParse(args[6], out excludeLabels))
                throw new ArgumentException("Error casting type");

            var serviceProvider = Startup.Build();
            var openAPICompare = serviceProvider.GetService<IOpenAPICompare>();
            var renderer = serviceProvider.GetService<IMarkdownRender>();

            var fileName = Path.GetFileNameWithoutExtension(oldFile.LocalPath);
            Console.WriteLine($"Running OpenAPI Diff for {fileName}");

            string markdown;
            DiffResultEnum diffResult;
            try
            {
                var openAPIDiff = openAPICompare.FromLocations(oldFile.LocalPath, newFile.LocalPath);
                diffResult = openAPIDiff.IsChanged().DiffResult;
                markdown = renderer.Render(openAPIDiff);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.ExitCode = 1;
                throw;
            }

            Console.WriteLine($"Completed OpenAPI DIff with Result {diffResult}");

            var commentMarkdown = CommentUtil.GetCommentMarkdown(fileName, diffResult, markdown);

            var credentials = new InMemoryCredentialStore(new Credentials(token));
            var github = new GitHubClient(new ProductHeaderValue("openapi-diff-action"), credentials);

            try
            {
                var comments = await github.Issue.Comment.GetAllForIssue(owner, repositoryName, prNumber);

                if (!addComment)
                {
                    var existingComment = comments.FirstOrDefault(x => x.Body.Contains(CommentUtil.GetIdentifierString(fileName)));
                    if (existingComment != null)
                    {
                        await github.Issue.Comment.Update(owner, repositoryName, existingComment.Id, commentMarkdown);
                        Console.WriteLine($"Updated existing comment with id {existingComment}");
                    }
                    else
                    {
                        await github.Issue.Comment.Create(owner, repositoryName, prNumber, commentMarkdown);
                        Console.WriteLine("Added new comment because no existing comment found");
                    }
                }
                else
                {
                    await github.Issue.Comment.Create(owner, repositoryName, prNumber, commentMarkdown);
                    Console.WriteLine($"Added new comment because addComment is true");
                }

                if (!excludeLabels)
                {
                    await LabelUtil.CreateIfNotExist(github, owner, repositoryName);

                    // get all comments with identifiers
                    // group all comments by unique identifier
                    // get latest change level for each unique identifier
                    // get highest change level including current changes
                    // remove all old change labels
                    // add new change label
                    var highestChangeLevel = comments
                        .Select(x => new
                        {
                            comment = x,
                            identifier = CommentUtil.GetIdentifier(x.Body)
                        }).Where(x => !x.identifier.IsNullOrEmpty())
                        .GroupBy(x => x.identifier)
                        .Select(x => new
                        {
                            key = x.Key,
                            value = x.OrderByDescending(y => y.comment.CreatedAt)
                                .FirstOrDefault()
                        })
                        .Select(x => CommentUtil.GetDiffResult(x.value.comment.Body))
                        .DefaultIfEmpty(DiffResultEnum.NoChanges)
                        .Max();

                    var allLabels = await github.Issue.Labels.GetAllForIssue(owner, repositoryName, prNumber);
                    foreach (var label in allLabels.Where(x => x.Name.StartsWith("OAS:")))
                    {
                        await github.Issue.Labels.RemoveFromIssue(owner, repositoryName, prNumber, label.Name);
                    }

                    var labelName = LabelUtil.GetLabelForDiffResult(diffResult > highestChangeLevel ? diffResult : highestChangeLevel);
                    await github.Issue.Labels.AddToIssue(owner, repositoryName, prNumber, new[] { labelName });
                    Console.WriteLine($"Added label {labelName}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Environment.ExitCode = 1;
                throw;
            }

            return Environment.ExitCode;
        }
    }
}
