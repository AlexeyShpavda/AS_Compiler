using AS_Compiler.Core.CodeAnalysis.Symbols;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundConversionExpression : BoundExpression
    {
        public BoundConversionExpression(TypeSymbol type, BoundExpression expression)
        {
            Type = type;
            Expression = expression;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.ConversionExpression;
        public override TypeSymbol Type { get; }
        public BoundExpression Expression { get; }
    }
}