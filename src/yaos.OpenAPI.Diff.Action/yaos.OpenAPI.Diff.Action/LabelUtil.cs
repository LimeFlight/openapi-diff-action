using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yaos.OpenAPI.Diff.Enums;

namespace yaos.OpenAPI.Diff.Action
{
    public static class LabelUtil
    {
        public static async Task CreateIfNotExist(GitHubClient github, string owner, string name)
        {
            var allLabels = await github.Issue.Labels.GetAllForRepository(owner, name);
            var labels = GetLabels();
            foreach (var label in labels.Where(label => allLabels.All(x => x.Name != label.Name)))
            {
                await github.Issue.Labels.Create(owner, name, label);
            }
        }

        public static string GetLabelForDiffResult(DiffResultEnum diffResult)
        {
            switch (diffResult)
            {
                case DiffResultEnum.NoChanges:
                    return GetNoChangesLabel().Name;
                case DiffResultEnum.Metadata:
                    return GetMetadataLabel().Name;
                case DiffResultEnum.Compatible:
                    return GetCompatibleLabel().Name;
                case DiffResultEnum.Unknown:
                    return GetUnknownLabel().Name;
                case DiffResultEnum.Incompatible:
                    return GetIncompatibleLabel().Name;
                default:
                    throw new ArgumentOutOfRangeException(nameof(diffResult), diffResult, null);
            }
        }

        public static NewLabel GetNoChangesLabel()
        {
            return new NewLabel("OAS:NoChanges", "cccccc")
            {
                Description = "No OpenAPI Specification changes",
            };
        }
        public static NewLabel GetMetadataLabel()
        {
            return new NewLabel("OAS:Metadata", "1d76db")
            {
                Description = "Metadata OpenAPI Specification changes",
            };
        }
        public static NewLabel GetCompatibleLabel()
        {
            return new NewLabel("OAS:Compatible", "0e8a16")
            {
                Description = "Compatible OpenAPI Specification changes",
            };
        }
        public static NewLabel GetUnknownLabel()
        {
            return new NewLabel("OAS:Unknown", "f9d0c4")
            {
                Description = "Unknown OpenAPI Specification changes",
            };
        }
        public static NewLabel GetIncompatibleLabel()
        {
            return new NewLabel("OAS:Incompatible", "d93f0b")
            {
                Description = "Incompatible OpenAPI Specification changes",
            };
        }

        private static IEnumerable<NewLabel> GetLabels()
        {
            return new List<NewLabel>
            {
                GetNoChangesLabel(),
                GetMetadataLabel(),
                GetCompatibleLabel(),
                GetUnknownLabel(),
                GetIncompatibleLabel()
            };
        }
    }
}