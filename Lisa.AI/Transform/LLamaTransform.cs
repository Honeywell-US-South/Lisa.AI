namespace Lisa.AI.Transform
{
    /// <summary>
    /// ChatML History Transformation for LLama Model
    /// </summary>
    public class LLamaHistoryTransform : BaseHistoryTransform
    {
        /// <inheritdoc/>
        protected override string userToken => "<|start_header_id|>user<|end_header_id|>\n";

        /// <inheritdoc/>
        protected override string assistantToken => "<|start_header_id|>assistant<|end_header_id|>\n";

        /// <inheritdoc/>
        protected override string systemToken => "<|start_header_id|>system<|end_header_id|>\n";

        /// <inheritdoc/>
        protected override string endToken => "<|eot_id|>";
    }
}
