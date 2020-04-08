using System;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Binding;

namespace AS_Compiler.Core.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            EvaluateStatement(_root);

            return _lastValue;
        }

        private void EvaluateStatement(BoundStatement node)
        {
            switch (node.BoundNodeType)
            {
                case BoundNodeType.IfStatement:
                    EvaluateIfStatement((BoundIfStatement)node);
                    break;
                case BoundNodeType.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement) node);
                    break;
                case BoundNodeType.VariableDeclaration:
                    EvaluateVariableDeclaration((BoundVariableDeclaration)node);
                    break;
                case BoundNodeType.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)node);
                    break;
                default:
                    throw new Exception($"Unexpected node {node.BoundNodeType}");
            }
        }

        private void EvaluateIfStatement(BoundIfStatement node)
        {
            var condition = (bool) EvaluateExpression(node.Condition);

            if (condition)
            {
                EvaluateStatement(node.ThenStatement);
            }
            else if (node.ElseStatement != null)
            {
                EvaluateStatement(node.ElseStatement);
            }
        }

        private void EvaluateBlockStatement(BoundBlockStatement node)
        {
            foreach (var statement in node.Statements)
            {
                EvaluateStatement(statement);
            }
        }

        private void EvaluateVariableDeclaration(BoundVariableDeclaration node)
        {
            var value = EvaluateExpression(node.Initializer);
            _variables[node.Variable] = value;
            _lastValue = value;
        }

        private void EvaluateExpressionStatement(BoundExpressionStatement node)
        {
            _lastValue = EvaluateExpression(node.Expression);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            return node.BoundNodeType switch
            {
                BoundNodeType.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression) node),
                BoundNodeType.VariableExpression => EvaluateVariableExpression((BoundVariableExpression) node),
                BoundNodeType.AssignmentExpression => EvaluateAssignmentExpression((BoundAssignmentExpression) node),
                BoundNodeType.UnaryExpression => EvaluateUnaryExpression((BoundUnaryExpression) node),
                BoundNodeType.BinaryExpression => EvaluateBinaryExpression((BoundBinaryExpression) node),
                _ => throw new Exception($"Unexpected node {node.Type}")
            };
        }

        private object EvaluateLiteralExpression(BoundLiteralExpression boundLiteralExpression)
        {
            return boundLiteralExpression.Value;
        }

        private object EvaluateVariableExpression(BoundVariableExpression boundVariableExpression)
        {
            return _variables[boundVariableExpression.Variable];
        }

        private object EvaluateAssignmentExpression(BoundAssignmentExpression boundAssignmentExpression)
        {
            var value = EvaluateExpression(boundAssignmentExpression.Expression);
            _variables[boundAssignmentExpression.Variable] = value;

            return value;
        }

        private object EvaluateUnaryExpression(BoundUnaryExpression boundUnaryExpression)
        {
            var operand = EvaluateExpression(boundUnaryExpression.Operand);

            return boundUnaryExpression.Operator.OperatorType switch
            {
                BoundUnaryOperatorType.Identity => (int)operand,
                BoundUnaryOperatorType.Negation => -(int)operand,
                BoundUnaryOperatorType.LogicalNegation => !(bool)operand,
                _ => throw new Exception($"Unexpected unary operator {boundUnaryExpression.Operator.OperatorType}")
            };
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression boundBinaryExpression)
        {
            var left = EvaluateExpression(boundBinaryExpression.Left);
            var right = EvaluateExpression(boundBinaryExpression.Right);

            return boundBinaryExpression.Operator.OperatorType switch
            {
                BoundBinaryOperatorType.Addition => (int)left + (int)right,
                BoundBinaryOperatorType.Subtraction => (int)left - (int)right,
                BoundBinaryOperatorType.Multiplication => (int)left * (int)right,
                BoundBinaryOperatorType.Division => (int)left / (int)right,
                BoundBinaryOperatorType.LogicalAnd => (bool)left && (bool)right,
                BoundBinaryOperatorType.LogicalOr => (bool)left || (bool)right,
                BoundBinaryOperatorType.Equals => Equals(left, right),
                BoundBinaryOperatorType.NotEquals => !Equals(left, right),
                BoundBinaryOperatorType.LessThan => (int)left < (int)right,
                BoundBinaryOperatorType.LessThanOrEquals => (int)left <= (int)right,
                BoundBinaryOperatorType.GreaterThan => (int)left > (int)right,
                BoundBinaryOperatorType.GreaterThanOrEquals => (int)left >= (int)right,
                _ => throw new Exception($"Unexpected binary operator {boundBinaryExpression.Operator.OperatorType}")
            };
        }
    }
}