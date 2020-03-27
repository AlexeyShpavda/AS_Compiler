using System.Collections.Generic;
using System.Linq;

namespace AS_Compiler.Core.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IEnumerable<string> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToList();
            Value = value;
        }

        public IReadOnlyList<string> Diagnostics { get; set; }
        public object Value { get; set; }
    }
}