using System;
using System.Collections.Generic;
using AS_Compiler.CommandLine.CodeAnalysis.Syntax;

namespace AS_Compiler.CommandLine.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.Type switch
            {
                SyntaxType.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax) syntax),
                SyntaxType.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax) syntax),
                SyntaxType.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax) syntax),
                _ => throw new Exception($"Unexpected syntax {syntax.Type}")
            };
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;

            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorType = BindUnaryOperatorType(syntax.OperatorToken.Type, boundOperand.Type);

            if (boundOperatorType == null)
            {
                _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Type}' is not defined for type {boundOperand.Type}");
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperatorType.Value, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperatorType = BindBinaryOperatorType(syntax.OperatorToken.Type, boundLeft.Type, boundRight.Type);

            if (boundOperatorType == null)
            {
                _diagnostics.Add($"Binary operator '{syntax.OperatorToken.Type}' is not defined for type {boundLeft.Type} and { boundRight.Type}");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorType.Value, boundRight);
        }


        private BoundUnaryOperatorType? BindUnaryOperatorType(SyntaxType type, Type operandType)
        {
            if(operandType == typeof(int))
            {
                switch (type)
                {
                    case SyntaxType.PlusToken:
                        return BoundUnaryOperatorType.Identity;
                    case SyntaxType.MinusToken:
                        return BoundUnaryOperatorType.Negation;
                }
            }

            if (operandType == typeof(bool))
            {
                switch (type)
                {
                    case SyntaxType.BangToken:
                        return BoundUnaryOperatorType.LogicalNegation;
                }
            }

            return null;
        }

        private BoundBinaryOperatorType? BindBinaryOperatorType(SyntaxType type, Type leftType, Type rightType)
        {
            if (leftType == typeof(int) && rightType == typeof(int))
            {
                switch (type)
                {
                    case SyntaxType.PlusToken:
                        return BoundBinaryOperatorType.Addition;
                    case SyntaxType.MinusToken:
                        return BoundBinaryOperatorType.Subtraction;
                    case SyntaxType.StarToken:
                        return BoundBinaryOperatorType.Multiplication;
                    case SyntaxType.SlashToken:
                        return BoundBinaryOperatorType.Division;
                }
            }

            if (leftType == typeof(bool) && rightType == typeof(bool))
            {
                switch (type)
                {
                    case SyntaxType.AmpersandAmpersandToken:
                        return BoundBinaryOperatorType.LogicalAnd;
                    case SyntaxType.PipePipeToken:
                        return BoundBinaryOperatorType.LogicalOr;
                }
            }

            return null;
        }
    }
}