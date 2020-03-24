using System.Collections.Generic;

namespace AS_Compiler.CommandLine.CodeAnalysis
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxType Type { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}