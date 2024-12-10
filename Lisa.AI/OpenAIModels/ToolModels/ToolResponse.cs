using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.AI.OpenAIModels.ToolModels
{
    public class ToolResponse
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public object Result { get; set; }
        public string Error { get; set; }
    }
}
