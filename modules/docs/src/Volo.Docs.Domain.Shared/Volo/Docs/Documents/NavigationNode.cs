using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Volo.Docs.Documents
{
    public class NavigationNode
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("items")]
        public List<NavigationNode> Items { get; set; }

        public bool IsLeaf => !HasChildItems;

        public bool HasChildItems => Items != null && Items.Any();

        public bool IsEmpty => Text == null && Path == null;

        public virtual DateTime? CreationTime { get; set; }

        public virtual DateTime? LastUpdatedTime { get; set; }

        public DateTime? LastSignificantUpdateTime { get; set; }

        public bool IsSelected(string documentName)
        {
            if (documentName == null)
            {
                return false;
            }

            var path = Path ?? string.Empty;
            var pathHasExtension = System.IO.Path.HasExtension(path);

            if (!pathHasExtension)
            {
                var extension = System.IO.Path.GetExtension(documentName);
                path = path.EnsureEndsWith('/') + "index" + extension;
            }
            

            if (string.Equals(documentName, path, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (Items == null)
            {
                return false;
            }

            foreach (var childItem in Items)
            {
                if (childItem.IsSelected(documentName))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static class NavigationNodeExtension
    {
        public static IEnumerable<NavigationNode> GetAllNodes(this IEnumerable<NavigationNode> source, Func<NavigationNode, IEnumerable<NavigationNode>> selector)
        {
            if (source == null)
            {
                yield break;
            }

            foreach (var item in source)
            {
                yield return item;
                foreach (var subItem in GetAllNodes(selector(item), selector))
                {
                    yield return subItem;
                }
            }
        }
    }
}
