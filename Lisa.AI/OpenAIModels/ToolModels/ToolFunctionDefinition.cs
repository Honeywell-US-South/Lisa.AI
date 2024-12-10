using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lisa.AI.OpenAIModels.ToolModels
{
    public class ToolFunctionDefinition
    {
        public Func<Dictionary<string, object>, Task<object>> Handler { get; set; }
        public FunctionInfo FunctionInfo { get; set; }
    }
}
