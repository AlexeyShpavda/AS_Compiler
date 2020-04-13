using System;
using AS_Compiler.Core.CodeAnalysis.Symbols;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;

            Type = value switch
            {
                bool _ => TypeSymbol.Bool,
                int _ => TypeSymbol.Int,
                string _ => TypeSymbol.String,
                _ => throw new Exception($"Unexpected literal '{value}' of type '{value.GetType()}'.")
            };
        }

        public override TypeSymbol Type { get; }
        public object Value { get; set; }
        public override BoundNodeType BoundNodeType => BoundNodeType.LiteralExpression;
    }
}