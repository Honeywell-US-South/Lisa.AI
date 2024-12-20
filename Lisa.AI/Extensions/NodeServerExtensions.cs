using Lisa.AI.Config;
using Lisa.AI.Singletons;

namespace Lisa.AI.Extensions
{
    public static class NodeServerExtensions
    {
        public static NodeServer? SelectNode(this List<NodeServer> nodeServers, string selection)
        {
            if (string.IsNullOrEmpty(selection))
                throw new ArgumentException("Selection cannot be null or empty", nameof(selection));

            Func<NodeServer, bool> predicate = selection.ToLower() switch
            {
                "gpt" => server => server.Gpt && !string.IsNullOrEmpty(server.Url),
                "embedding" => server => server.Embedding && !string.IsNullOrEmpty(server.Url),
                "vision" => server => server.Vision && !string.IsNullOrEmpty(server.Url),
                _ => throw new ArgumentException("Invalid selection", nameof(selection))
            };

            var filteredServers = nodeServers.Where(predicate).ToList();

            if (!filteredServers.Any()) return null;

            NodeServer? selectedServer = null;
            int lowestCount = int.MaxValue;

            foreach (var server in filteredServers)
            {
                string key = $"{server.Url}_{selection}";
                int count = (int)(MemDataStore.Instance.GetData(key) ?? 0);

                if (count < lowestCount)
                {
                    lowestCount = count;
                    selectedServer = server;
                    
                }
            }

            return selectedServer;
        }

        public static bool RegisterNodeUse(this NodeServer node, string selection)
        {
            if (string.IsNullOrEmpty(selection))
                throw new ArgumentException("Selection cannot be null or empty", nameof(selection));
            var val = selection.ToLower();
            string key = $"{node.Url}_{selection}";
            int count = (int)(MemDataStore.Instance.GetData(key) ?? 0);
            switch (val)
            {
                case "gpt":
                case "embedding":
                case "vision":
                    count++;
                    return MemDataStore.Instance.UpdateData(key, count, true);
                default:
                    throw new ArgumentException("Invalid selection", nameof(selection));
            }

        }

        public static bool UnregisterNodeUse(this NodeServer node, string selection)
        {
            if (string.IsNullOrEmpty(selection))
                throw new ArgumentException("Selection cannot be null or empty", nameof(selection));
            var val = selection.ToLower();
            string key = $"{node.Url}_{selection}";
            int count = (int)(MemDataStore.Instance.GetData(key) ?? 0);
            if (count == 0) return true;
            switch (val)
            {
                case "gpt":
                case "embedding":
                case "vision":
                    count--;
                    return MemDataStore.Instance.UpdateData(key, count, true);
                default:
                    throw new ArgumentException("Invalid selection", nameof(selection));
            }

        }

    }
}
