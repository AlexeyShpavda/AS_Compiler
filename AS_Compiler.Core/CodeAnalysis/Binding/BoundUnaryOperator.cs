using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Symbols;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundUnaryOperator
    {
        public BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType operatorType, TypeSymbol operandType)
            : this(syntaxType, operatorType, operandType, operandType)
        {
        }

        public BoundUnaryOperator(SyntaxType syntaxType, BoundUnaryOperatorType operatorType, TypeSymbol operandType, TypeSymbol resultType)
        {
            SyntaxType = syntaxType;
            OperatorType = operatorType;
            OperandType = operandType;
            Type = resultType;
        }

        public SyntaxType SyntaxType { get; }
        public BoundUnaryOperatorType OperatorType { get; }
        public TypeSymbol OperandType { get; }
        public TypeSymbol Type { get; }

        private static readonly BoundUnaryOperator[] Operators =
        {
            new BoundUnaryOperator(SyntaxType.BangToken, BoundUnaryOperatorType.LogicalNegation, TypeSymbol.Bool),

            new BoundUnaryOperator(SyntaxType.PlusToken, BoundUnaryOperatorType.Identity, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxType.MinusToken, BoundUnaryOperatorType.Negation, TypeSymbol.Int),
            new BoundUnaryOperator(SyntaxType.TildeToken, BoundUnaryOperatorType.OnesComplement, TypeSymbol.Int)
        };

        public static BoundUnaryOperator Bind(SyntaxType syntaxType, TypeSymbol operandType)
        {
            return Operators.FirstOrDefault(op =>
                op.SyntaxType == syntaxType
                && op.OperandType == operandType);
        }
    }
}