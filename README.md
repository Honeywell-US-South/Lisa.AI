Lisa.AI - In Development


Download .gguf Models From Ollama Library
1. Go to:  [Ollama’s library](https://ollama.com/library)
2. Select a Model
   ![image](https://github.com/user-attachments/assets/847724aa-bf38-4974-baae-32b10f930e55)
4. Get teh digest from the manifest. Format: https://registry.ollama.ai/v2/library/MODEL_NAME/manifests/MODEL_PARAMETERS
   '''code
   https://registry.ollama.ai/v2/library/llama3.2/manifests/3b
   '''
   ![image](https://github.com/user-attachments/assets/abfdc97b-79b9-4bd8-a93f-032a92380126)

6. Download the .gguf file. Format: https://registry.ollama.ai/v2/library/MODEL_NAME/blobs/sha256:DIGEST
   '''code
   https://registry.ollama.ai/v2/library/llama3.2/blobs/sha256:dde5aa3fc5ffc17176b5e8bdc82f587b24b2678c6c66101bf7da77af9f7ccdff
   '''
   ![image](https://github.com/user-attachments/assets/574b96c8-9521-4386-a894-976291c40b75)

8. Rename the data file. Exmaple: llama3.2.gguf
   ![image](https://github.com/user-attachments/assets/e1453c76-bda4-43da-82f7-ad674608f40f)
