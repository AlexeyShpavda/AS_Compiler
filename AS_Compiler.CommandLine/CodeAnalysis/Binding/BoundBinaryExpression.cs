using System;

namespace AS_Compiler.CommandLine.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorType operatorType, BoundExpression right)
        {
            Left = left;
            OperatorType = operatorType;
            Right = right;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.BinaryExpression;
        public override Type Type => Left.Type;
        public BoundExpression Left { get; }
        public BoundBinaryOperatorType OperatorType { get; set; }
        public BoundExpression Right { get; }
    }
}