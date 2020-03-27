using System;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.Type switch
            {
                SyntaxType.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax) syntax),
                SyntaxType.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax) syntax),
                SyntaxType.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax) syntax),
                SyntaxType.ParenthesizedExpression => BindExpression(((ParenthesizedExpressionSyntax) syntax).Expression),
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
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Type, boundOperand.Type);

            if (boundOperator == null)
            {
                Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.TextSpan, syntax.OperatorToken.Text, boundOperand.Type);
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Type, boundLeft.Type, boundRight.Type);

            if (boundOperator == null)
            {
                Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.TextSpan, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    }
}