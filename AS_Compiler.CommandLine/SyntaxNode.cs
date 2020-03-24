using System.Collections.Generic;

namespace AS_Compiler.CommandLine
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxType SyntaxType { get; }
        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}