using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
                SyntaxType.BlockStatement => BindBlockStatement((BlockStatementSyntax)syntax),
                SyntaxType.VariableDeclaration => BindVariableDeclaration((VariableDeclarationSyntax)syntax),
                SyntaxType.ExpressionStatement => BindExpressionStatement((ExpressionStatementSyntax)syntax),
                _ => throw new Exception($"Unexpected syntax {syntax.Type}")
            };
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

        private BoundExpression BindExpression(ExpressionSyntax syntax)
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

            if (!_scope.TryLookup(name, out var variable))
            {
                Diagnostics.ReportUndefinedName(syntax.IdentifierToken.TextSpan, name);
                return new BoundLiteralExpression(0);
            }

            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.ExpressionSyntax);

            if(!_scope.TryLookup(name, out var variable))
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