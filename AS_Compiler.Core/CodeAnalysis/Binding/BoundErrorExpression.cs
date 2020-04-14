using AS_Compiler.Core.CodeAnalysis.Symbols;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundErrorExpression : BoundExpression
    {
        public override BoundNodeType BoundNodeType => BoundNodeType.ErrorExpression;
        public override TypeSymbol Type => TypeSymbol.Error;
    }
}