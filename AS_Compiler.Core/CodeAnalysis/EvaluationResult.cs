using System.Collections.Generic;
using System.Linq;

namespace AS_Compiler.Core.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public EvaluationResult(IEnumerable<Diagnostic> diagnostics, object value)
        {
            Diagnostics = diagnostics.ToArray();
            Value = value;
        }

        public IReadOnlyList<Diagnostic> Diagnostics { get; set; }
        public object Value { get; set; }
    }
}