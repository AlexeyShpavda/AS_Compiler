using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Symbols;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private BoundScope _scope;

        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }

        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax compilationUnitSyntax)
        {
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var expression = binder.BindStatement(compilationUnitSyntax.Statement);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostic = binder.Diagnostics.ToImmutableArray();

            if (previous != null)
            {
                diagnostic = diagnostic.InsertRange(0, previous.Diagnostics);
            }

            return new BoundGlobalScope(previous, diagnostic, variables, expression);
        }

        private static BoundScope CreateParentScopes(BoundGlobalScope previous)
        {
            var stack = new Stack<BoundGlobalScope>();

            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }

            BoundScope parent = null;

            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);

                foreach (var variable in previous.Variables)
                {
                    scope.TryDeclare(variable);
                }

                parent = scope;
            }

            return parent;
        }

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

        private BoundStatement BindStatement(StatementSyntax syntax)
        {
            return syntax.Type switch
            {
                SyntaxType.ForStatement => BindForStatement((ForStatementSyntax)syntax),
                SyntaxType.WhileStatement => BindWhileStatement((WhileStatementSyntax)syntax),
                SyntaxType.IfStatement => BindIfStatement((IfStatementSyntax)syntax),
                SyntaxType.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
                SyntaxType.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax),
                SyntaxType.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
                _ => throw new Exception($"Unexpected syntax {syntax.Type}")
            };
        }

        private BoundStatement BindForStatement(ForStatementSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var variable = new VariableSymbol(name, true, TypeSymbol.Int);

            if (!_scope.TryDeclare(variable))
            {
                Diagnostics.ReportVariableAlreadyDeclared(syntax.TextSpan, name);
            }

            var lowerBound = BindExpression(syntax.LowerBound, TypeSymbol.Int);
            var upperBound = BindExpression(syntax.UpperBound, TypeSymbol.Int);

            _scope = new BoundScope(_scope);

            var body = BindStatement(syntax.Body);

            _scope = _scope.Parent;

            return new BoundForStatement(variable, lowerBound, upperBound, body);
        }

        private BoundStatement BindWhileStatement(WhileStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var body = BindStatement(syntax.Body);

            return new BoundWhileStatement(condition, body);
        }

        private BoundStatement BindIfStatement(IfStatementSyntax syntax)
        {
            var condition = BindExpression(syntax.Condition, TypeSymbol.Bool);
            var thenStatement = BindStatement(syntax.ThenStatement);
            var elseStatement = syntax.ElseClause == null ? null : BindStatement(syntax.ElseClause.ElseStatement);

            return new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        private BoundStatement BindBlockStatement(BlockStatementSyntax syntax)
        {
            var statements = ImmutableArray.CreateBuilder<BoundStatement>();
            _scope = new BoundScope(_scope);

            foreach (var statement in syntax.Statements.Select(BindStatement))
            {
                statements.Add(statement);
            }

            _scope = _scope.Parent;

            return new BoundBlockStatement(statements.ToImmutable());
        }

        private BoundStatement BindVariableDeclaration(VariableDeclarationSyntax syntax)
        {
            var name = syntax.Identifier.Text;
            var isReadOnly = syntax.Keyword.Type == SyntaxType.LetKeyword;
            var initializer = BindExpression(syntax.Initializer);
            var variable = new VariableSymbol(name, isReadOnly, initializer.Type);

            if (!_scope.TryDeclare(variable))
            {
                Diagnostics.ReportVariableAlreadyDeclared(syntax.Identifier.TextSpan, name);
            }

            return new BoundVariableDeclaration(variable, initializer);
        }

        private BoundStatement BindExpressionStatement(ExpressionStatementSyntax syntax)
        {
            var expression = BindExpression(syntax.Expression);

            return new BoundExpressionStatement(expression);
        }

        private BoundExpression BindExpression(ExpressionSyntax expressionSyntax, TypeSymbol targetType)
        {
            var result = BindExpression(expressionSyntax);

            if (result.Type != targetType)
            {
                Diagnostics.ReportCannotConvert(expressionSyntax.TextSpan, result.Type, targetType);
            }

            return result;
        }

        private BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            return syntax.Type switch
            {
                SyntaxType.ParenthesizedExpression => BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax),
                SyntaxType.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax)syntax),
                SyntaxType.NameExpression => BindNameExpression((NameExpressionSyntax)syntax),
                SyntaxType.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax)syntax),
                SyntaxType.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax)syntax),
                SyntaxType.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax)syntax),
                _ => throw new Exception($"Unexpected syntax {syntax.Type}")
            };
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private static BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;

            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;

            if (string.IsNullOrEmpty(name))
            {
                return new BoundErrorExpression();
            }

            if (!_scope.TryLookup(name, out var variable))
            {
                Diagnostics.ReportUndefinedName(syntax.IdentifierToken.TextSpan, name);
                return new BoundErrorExpression();
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.ExpressionSyntax);

            if (!_scope.TryLookup(name, out var variable))
            {
                Diagnostics.ReportUndefinedName(syntax.IdentifierToken.TextSpan, name);
                return boundExpression;
            }

            if (variable.IsReadOnly)
            {
                Diagnostics.ReportCannotAssign(syntax.EqualsToken.TextSpan, name);
            }

            if (boundExpression.Type != variable.Type)
            {
                Diagnostics.ReportCannotConvert(syntax.ExpressionSyntax.TextSpan, boundExpression.Type, variable.Type);

                return boundExpression;
            }

            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Type, boundOperand.Type);

            if (boundOperand.Type == TypeSymbol.Error)
            {
                return new BoundErrorExpression();
            }

            if (boundOperator == null)
            {
                Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.TextSpan, syntax.OperatorToken.Text, boundOperand.Type);
                return new BoundErrorExpression();
            }

            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Type, boundLeft.Type, boundRight.Type);

            if (boundLeft.Type == TypeSymbol.Error || boundRight.Type == TypeSymbol.Error)
            {
                return new BoundErrorExpression();
            }

            if (boundOperator == null)
            {
                Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.TextSpan, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return new BoundErrorExpression();
            }

            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }
    }
}