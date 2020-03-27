using System;
using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        public BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, Type type)
            : this(syntaxType, operatorType, type, type, type)
        {
        }

        public BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, Type operandType, Type resultType)
            : this(syntaxType, operatorType, operandType, operandType, resultType)
        {
        }

        public BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, Type leftType, Type rightType, Type resultType)
        {
            SyntaxType = syntaxType;
            OperatorType = operatorType;
            LeftType = leftType;
            RightType = rightType;
            Type = resultType;
        }

        public SyntaxType SyntaxType { get; }
        public BoundBinaryOperatorType OperatorType { get; }
        public Type LeftType { get; }
        public Type RightType { get; }
        public Type Type { get; }

        private static readonly BoundBinaryOperator[] Operators =
        {
            new BoundBinaryOperator(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, typeof(int)),
            new BoundBinaryOperator(SyntaxType.MinusToken, BoundBinaryOperatorType.Subtraction, typeof(int)),
            new BoundBinaryOperator(SyntaxType.StarToken, BoundBinaryOperatorType.Multiplication, typeof(int)),
            new BoundBinaryOperator(SyntaxType.SlashToken, BoundBinaryOperatorType.Division, typeof(int)),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, typeof(int), typeof(bool)),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.NotEquals, typeof(int), typeof(bool)),

            new BoundBinaryOperator(SyntaxType.AmpersandAmpersandToken, BoundBinaryOperatorType.LogicalAnd, typeof(bool)),
            new BoundBinaryOperator(SyntaxType.PipePipeToken, BoundBinaryOperatorType.LogicalOr, typeof(bool)),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, typeof(bool)),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.NotEquals, typeof(bool)),
        };

        public static BoundBinaryOperator Bind(SyntaxType syntaxType, Type leftType, Type rightType)
        {
            return Operators.FirstOrDefault(op =>
                op.SyntaxType == syntaxType 
                && op.LeftType == leftType 
                && op.RightType == rightType);
        }
    }
}