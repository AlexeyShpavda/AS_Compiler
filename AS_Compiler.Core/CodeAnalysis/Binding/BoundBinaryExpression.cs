﻿using AS_Compiler.Core.CodeAnalysis.Symbols;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperator op, BoundExpression right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.BinaryExpression;
        public override TypeSymbol Type => Operator.Type;
        public BoundExpression Left { get; }
        public BoundBinaryOperator Operator { get; set; }
        public BoundExpression Right { get; }
    }
}