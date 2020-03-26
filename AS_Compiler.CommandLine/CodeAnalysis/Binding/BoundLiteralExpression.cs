using System;

namespace AS_Compiler.CommandLine.CodeAnalysis.Binding
{
    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public override Type Type => Value.GetType();
        public object Value { get; set; }
        public override BoundNodeType BoundNodeType => BoundNodeType.LiteralExpression;
    }
}