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
            var value = syntax.LiteralSyntaxToken.Value as int? ?? 0;

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
            if(operandType != typeof(int))
            {
                return null;
            }

            return type switch
            {
                SyntaxType.Plus => BoundUnaryOperatorType.Identity,
                SyntaxType.Minus => BoundUnaryOperatorType.Negation,
                _ => throw new Exception($"Unexpected unary operator {type}")
            };
        }

        private BoundBinaryOperatorType? BindBinaryOperatorType(SyntaxType type, Type leftType, Type rightType)
        {
            if (leftType != typeof(int) || rightType != typeof(int))
            {
                return null;
            }

            return type switch
            {
                SyntaxType.Plus => BoundBinaryOperatorType.Addition,
                SyntaxType.Minus => BoundBinaryOperatorType.Subtraction,
                SyntaxType.Star => BoundBinaryOperatorType.Multiplication,
                SyntaxType.Slash => BoundBinaryOperatorType.Division,
                _ => throw new Exception($"Unexpected binary operator {type}")
            };
        }
    }
}