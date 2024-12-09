
# Application Settings Configuration

This document explains the `appsettings.json` configuration structure and its purpose.

---

## **Logging**
Defines logging levels for the application:
```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```
- **`Default`**: General log level (`Information`).
- **`Microsoft.AspNetCore`**: Specific log level for ASP.NET Core framework logs (`Warning`).

---

## **AllowedHosts**
```json
"AllowedHosts": "*"
```
- Specifies which hosts are allowed to access the application. `*` allows all hosts.

---

## **API Key**
```json
"ApiKey": ""
```
- Placeholder for the API key used for authentication or external integrations.

---

## **GlobalSettings**
Defines global application settings:
```json
"GlobalSettings": {
  "CurrentModelIndex": 0,
  "AutoReleaseTime": 30
}
```
- **`CurrentModelIndex`**: Index of the currently selected model.
- **`AutoReleaseTime`**: Auto-release time in minutes (0 means no auto-release).

---

## **LLM Model Configuration**
Configures the settings for the Large Language Model (LLM):
```json
"LLmModelSettings": [
  {
    "Name": "llama-3.2-3B",
    "Description": "llama-3.2-3B",
    "Version": "3.1",
    "WebSite": "https://ollama.com/library/llama3.2",
    "SystemPrompt": "You are a helpful assistant",
    "ModelParams": {
      "ModelPath": "C:\Code\models\Llama-Magpie-3.2-3B-Instruct.F16.gguf",
      "ContextSize": 32768,
      "Seed": 1337,
      "GpuLayerCount": 30
    },
    "AntiPrompts": [ "<|eot_id|>", "<|endoftext|>" ],
    "WithTransform": {
      "HistoryTransform": "Lisa.AI.Transform.LLamaHistoryTransform"
    },
    "ToolPrompt": {
      "Index": 1,
      "Lang": "en"
    }
  }
]
```
### Key Components:
- **`Name` & `Description`**: Name and description of the model.
- **`Version`**: Version of the model.
- **`WebSite`**: URL with more information about the model.
- **`SystemPrompt`**: Default prompt used to initialize the assistant's behavior.
- **`ModelParams`**:
  - `ModelPath`: Path to the model file.
  - `ContextSize`: Size of the context window for the model.
  - `Seed`: Random seed for reproducibility.
  - `GpuLayerCount`: Number of layers to offload to the GPU.
- **`AntiPrompts`**: List of tokens or sequences that indicate when to stop generating.
- **`WithTransform`**: Configuration for history transformation.
- **`ToolPrompt`**: Configuration for tool-specific prompts.

---

## **Tool Prompt Configuration**
Defines tool-related configurations:
```json
"ToolPromptConfig": [
  {
    "PromptConfigDesc": "default",
    "FN_NAME": "Action",
    "FN_ARGS": "Action Input",
    "FN_RESULT": "Observation",
    "FN_EXIT": "Answer:",
    "FN_STOP_WORDS": [ "Observation:", "Answer:" ],
    "FN_TEST": "Action:? ?(.*?)\s*(Action Input:? ?(.*?)\s*)(?=Observation|$|\n)",
    "FN_CALL_TEMPLATE": "Action: {0}\nAction Input: {1}",
    "FN_RESULT_SPLIT": "",
    "FN_RESULT_TEMPLATE": "Observation: {0}",
    "FN_CALL_TEMPLATE_INFO": {
      "en": "# Tools\n\n## You have access to the following tools:\n\n{tool_descs}\n"
    }
  }
]
```
- **`PromptConfigDesc`**: Description of the prompt configuration.
- **`FN_NAME`**: Identifier for the function name in the tool prompt.
- **`FN_ARGS`**: Identifier for the function's arguments.
- **`FN_RESULT`**: Identifier for the tool's result.
- **`FN_EXIT`**: Marks the exit point after tool execution.
- **`FN_STOP_WORDS`**: Stop words for halting generation after detecting tool-related outputs.

---

## **Embedding Service**
Defines the configuration for an independent embedding service:
```json
"EmbedingForward": "http://127.0.0.1:5000/embeddings"
```
- **`EmbedingForward`**: URL for the embedding service endpoint.
