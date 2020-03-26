using System;

namespace AS_Compiler.CommandLine.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperatorType operatorType, BoundExpression operand)
        {
            OperatorType = operatorType;
            Operand = operand;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.UnaryExpression;
        public override Type Type => Operand.Type;
        public BoundUnaryOperatorType OperatorType { get; set; }
        public BoundExpression Operand { get; }
    }
}