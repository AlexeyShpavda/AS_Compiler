using System;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly Dictionary<string, object> _variables;

        public Binder(Dictionary<string, object> variables)
        {
            _variables = variables;
        }

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.Type switch
            {
                SyntaxType.ParenthesizedExpression => BindParenthesizedExpression((ParenthesizedExpressionSyntax) syntax),
                SyntaxType.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax) syntax),
                SyntaxType.NameExpression => BindNameExpression((NameExpressionSyntax) syntax),
                SyntaxType.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax) syntax),
                SyntaxType.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax) syntax),
                SyntaxType.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax) syntax),
                _ => throw new Exception($"Unexpected syntax {syntax.Type}")
            };
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;

            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

            if (!_variables.TryGetValue(name, out var value))
            {
                Diagnostics.ReportUndefinedName(syntax.IdentifierToken.TextSpan, name);
                return new BoundLiteralExpression(0);
            }

            var type = value.GetType();
            return new BoundVariableExpression(name, type);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.ExpressionSyntax);

            var defaultValue = boundExpression.Type == typeof(int)
                ? 0
                : boundExpression.Type == typeof(bool)
                    ? (object) false
                    : null;

            _variables[name] = defaultValue ?? throw new Exception($"Unsupported variable type: {boundExpression.Type}");
            return new BoundAssignmentExpression(name, boundExpression);
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