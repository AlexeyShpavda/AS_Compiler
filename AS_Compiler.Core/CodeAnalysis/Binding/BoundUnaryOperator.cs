﻿using System;
using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryOperator
    {
        public BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType operatorType, Type operandType)
            : this(syntaxType, operatorType, operandType, operandType)
        {
        }

        public BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType operatorType, Type operandType, Type resultType)
        {
            SyntaxType = syntaxType;
            OperatorType = operatorType;
            OperandType = operandType;
            Type = resultType;
        }

        public SyntaxType SyntaxType { get; }
        public BoundUnaryOperatorType OperatorType { get; }
        public Type OperandType { get; }
        public Type Type { get; }

        private static readonly BoundUnaryOperator[] Operators =
        {
            new BoundUnaryOperator(SyntaxType.BangToken, BoundUnaryOperatorType.LogicalNegation, typeof(bool)),

            new BoundUnaryOperator(SyntaxType.PlusToken, BoundUnaryOperatorType.Identity, typeof(int)),
            new BoundUnaryOperator(SyntaxType.MinusToken, BoundUnaryOperatorType.Negation, typeof(int)),
            new BoundUnaryOperator(SyntaxType.TildeToken, BoundUnaryOperatorType.OnesComplement, typeof(int))
        };

        public static BoundUnaryOperator Bind(SyntaxType syntaxType, Type operandType)
        {
            return Operators.FirstOrDefault(op =>
                op.SyntaxType == syntaxType
                && op.OperandType == operandType);
        }
    }
}