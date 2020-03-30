﻿using System;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op, BoundExpression operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.UnaryExpression;
        public override Type Type => Operator.Type;
        public BoundUnaryOperator Operator { get; set; }
        public BoundExpression Operand { get; }
    }
}