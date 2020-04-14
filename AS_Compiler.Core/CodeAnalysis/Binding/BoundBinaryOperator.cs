using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Symbols;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundBinaryOperator
    {
        public BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, TypeSymbol type)
            : this(syntaxType, operatorType, type, type, type)
        {
        }

        public BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, TypeSymbol operandType, TypeSymbol resultType)
            : this(syntaxType, operatorType, operandType, operandType, resultType)
        {
        }

        public BoundBinaryOperator(SyntaxType syntaxType, BoundBinaryOperatorType operatorType, TypeSymbol leftType, TypeSymbol rightType, TypeSymbol resultType)
        {
            SyntaxType = syntaxType;
            OperatorType = operatorType;
            LeftType = leftType;
            RightType = rightType;
            Type = resultType;
        }

        public SyntaxType SyntaxType { get; }
        public BoundBinaryOperatorType OperatorType { get; }
        public TypeSymbol LeftType { get; }
        public TypeSymbol RightType { get; }
        public TypeSymbol Type { get; }

        private static readonly BoundBinaryOperator[] Operators =
        {
            new BoundBinaryOperator(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.MinusToken, BoundBinaryOperatorType.Subtraction, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.StarToken, BoundBinaryOperatorType.Multiplication, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.SlashToken, BoundBinaryOperatorType.Division, TypeSymbol.Int),

            new BoundBinaryOperator(SyntaxType.AmpersandToken, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.PipeToken, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Int),
            new BoundBinaryOperator(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Int),

            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Int, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.LessThanToken, BoundBinaryOperatorType.LessThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.LessThanOrEqualsToken, BoundBinaryOperatorType.LessThanOrEquals, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.GreaterThanToken, BoundBinaryOperatorType.GreaterThan, TypeSymbol.Int, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.GreaterThanOrEqualsToken, BoundBinaryOperatorType.GreaterThanOrEquals, TypeSymbol.Int, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.HatToken, BoundBinaryOperatorType.BitwiseXor, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.AmpersandToken, BoundBinaryOperatorType.BitwiseAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.PipeToken, BoundBinaryOperatorType.BitwiseOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.AmpersandAmpersandToken, BoundBinaryOperatorType.LogicalAnd, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.PipePipeToken, BoundBinaryOperatorType.LogicalOr, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.EqualsEqualsToken, BoundBinaryOperatorType.Equals, TypeSymbol.Bool),
            new BoundBinaryOperator(SyntaxType.BangEqualsToken, BoundBinaryOperatorType.NotEquals, TypeSymbol.Bool),

            new BoundBinaryOperator(SyntaxType.PlusToken, BoundBinaryOperatorType.Addition, TypeSymbol.String), 
        };

        public static BoundBinaryOperator Bind(SyntaxType syntaxType, TypeSymbol leftType, TypeSymbol rightType)
        {
            return Operators.FirstOrDefault(op =>
                op.SyntaxType == syntaxType
                && op.LeftType == leftType
                && op.RightType == rightType);
        }
    }
}