using AS_Compiler.Core.CodeAnalysis.Symbols;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal abstract class BoundExpression : BoundNode
    {
        public abstract TypeSymbol Type { get; }
    }
}