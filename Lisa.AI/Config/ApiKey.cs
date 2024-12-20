namespace Lisa.AI.Config
{
    public class ApiKey
    {
        /// <summary>
        /// Header name.
        /// </summary>
        public string HeaderKeyName { get; set; } = "ApiKey";

        /// <summary>
        /// API key value
        /// </summary>
        public string KeyValue { get; set; } = "";

        /// <summary>
        /// Allow use for GPT completion request
        /// </summary>
        public bool Gpt { get; set; } = false;

        /// <summary>
        /// Allow use for Embedding request
        /// </summary>
        public bool Embedding { get; set; } = false;

        /// <summary>
        /// Allow use for Vision request
        /// </summary>
        public bool Vision { get; set; } = false;

        /// <summary>
        /// Enable or disable key
        /// </summary>
        public bool Active { get; set; } = true;

        /// <summary>
        /// Empty = allow all; Or fill in ip addresss of allowed clients
        /// </summary>
        public string AllowedClientIps { get; set; } = "";
    }
}
