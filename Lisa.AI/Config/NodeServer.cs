namespace Lisa.AI.Config
{
    public class NodeServer
    {
        /// <summary>
        /// Url to node
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// ApiKey to access the server
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Use for GPT request
        /// </summary>
        public bool Gpt { get; set; } = false;

        /// <summary>
        /// Use for Embedding request
        /// </summary>
        public bool Embedding { get; set; } = false;

        /// <summary>
        /// Use for Vision request
        /// </summary>
        public bool Vision { get; set; } = false;
    }
}
