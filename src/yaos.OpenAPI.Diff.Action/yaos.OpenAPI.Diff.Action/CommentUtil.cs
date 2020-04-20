using System;
using System.Collections.Generic;
using System.Linq;
using yaos.OpenAPI.Diff.Enums;

namespace yaos.OpenAPI.Diff.Action
{
    public static class CommentUtil
    {
        public static string GetIdentifier(string commentBody)
        {
            var key = "openapi-diff-action-identifier-";

            if (!commentBody.Contains(key))
                return string.Empty;

            var pFrom = commentBody.IndexOf(key, StringComparison.Ordinal) + key.Length;
            var pTo = commentBody.AllIndexesOf("]").Where(x => x > pFrom).Min();

            return commentBody[pFrom..pTo];
        }

        public static DiffResultEnum GetDiffResult(string commentBody)
        {
            var key = "openapi-diff-action-changelevel-";

            if (!commentBody.Contains(key))
                return DiffResultEnum.Unknown;

            var pFrom = commentBody.IndexOf(key, StringComparison.Ordinal) + key.Length;
            var pTo = commentBody.AllIndexesOf("]").Where(x => x > pFrom).Min();

            if (Enum.TryParse(typeof(DiffResultEnum), commentBody[pFrom..pTo], out var changeLevel))
                return (DiffResultEnum)changeLevel;

            return DiffResultEnum.Unknown;
        }

        private static List<int> AllIndexesOf(this string str, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", nameof(value));
            var indexes = new List<int>();
            for (var index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index, StringComparison.Ordinal);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }
    }
}
