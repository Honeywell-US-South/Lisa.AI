using LLama;
using LLama.Native;
using System.Runtime.InteropServices;

namespace Lisa.AI.Extensions
{
    public static class LLamaWeightsExtensions
    {
        /// <summary>
        /// Apply a LoRA adapter to a loaded model. The path_base_model is the path to a higher
        /// quality model to use as a base for the layers modified by the adapter. Can be
        /// NULL to use the current loaded model. The model needs to be reloaded before applying
        /// a new adapter, otherwise the adapter will be applied on top of the previous one.
        /// </summary>
        /// <param name="model">The handle to the loaded model.</param>
        /// <param name="path">The file path to the LoRA adapter.</param>
        /// <param name="scale">The scaling factor for the LoRA adapter weights.</param>
        /// <param name="pathBase">The file path to the base model. Can be NULL to use the current loaded model.</param>
        /// <param name="threads">The number of threads to use for applying the adapter.</param>
        /// <returns>Returns 0 on success.</returns>
        [DllImport("llama", CallingConvention = CallingConvention.Cdecl)]
        private static extern int llama_model_apply_lora_from_file(SafeLlamaModelHandle model, string path, float scale, string? pathBase, int threads);


        /// <summary>
        /// Apply a LoRA adapter to a loaded model. The path_base_model is the path to a higher
        /// quality model to use as a base for the layers modified by the adapter. Can be
        /// NULL to use the current loaded model. The model needs to be reloaded before applying
        /// a new adapter, otherwise the adapter will be applied on top of the previous one.
        /// </summary>
        /// <param name="model">The handle to the loaded model.</param>
        /// <param name="loraPath">The file path to the LoRA adapter.</param>
        /// <param name="scale">The scaling factor for the LoRA adapter weights.</param>
        /// <param name="baseModelPath">The file path to the base model. Can be NULL to use the current loaded model.</param>
        /// <param name="threads">The number of threads to use for applying the adapter.</param>
        /// <returns>Returns 0 on success.</returns>
        public static int ExApplyLoraFromFile(this LLamaWeights model, string loraPath, float scale = 1.0f, string? baseModelPath = null, int threads = 1)
        {
            int result = llama_model_apply_lora_from_file(model.NativeHandle, loraPath, scale, baseModelPath, threads);
            
            return result;
        }
    }
}
