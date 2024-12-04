Lisa.AI - In Development


### Download .gguf Models From Ollama Library
##### 1. Go to:  [Ollama’s library](https://ollama.com/library)
   ![image](https://github.com/user-attachments/assets/e3b5a4cb-f3f4-454f-bedf-e541fe3cc952)

##### 2. Get Model Information
   ![image](https://github.com/user-attachments/assets/65d95e7d-9f2c-4355-8003-0de5def5c320)

##### 3. Get teh digest from the manifest. <strong>Format: `https://registry.ollama.ai/v2/library/MODEL_NAME/manifests/MODEL_PARAMETERS`</strong>
   ```plaintext
   https://registry.ollama.ai/v2/library/llama3.2/manifests/3b
   ```
   ![image](https://github.com/user-attachments/assets/abfdc97b-79b9-4bd8-a93f-032a92380126)

##### 4. Download the .gguf file. <strong>Format: `https://registry.ollama.ai/v2/library/MODEL_NAME/blobs/sha256:DIGEST`</strong>
   ```plaintext
   https://registry.ollama.ai/v2/library/llama3.2/blobs/sha256:dde5aa3fc5ffc17176b5e8bdc82f587b24b2678c6c66101bf7da77af9f7ccdff
   ```
   ![image](https://github.com/user-attachments/assets/574b96c8-9521-4386-a894-976291c40b75)

##### 5. Rename the data file. <strong>Exmaple: llama3.2.gguf</strong>
   ![image](https://github.com/user-attachments/assets/e1453c76-bda4-43da-82f7-ad674608f40f)
