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
                case BoundNodeType.WhileStatement:
                    EvaluateWhileStatement((BoundWhileStatement)node);
                    break;
                case BoundNodeType.IfStatement:
                    EvaluateIfStatement((BoundIfStatement)node);
                    break;
                case BoundNodeType.BlockStatement:
                    EvaluateBlockStatement((BoundBlockStatement)node);
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

        private void EvaluateWhileStatement(BoundWhileStatement node)
        {
            while ((bool)EvaluateExpression(node.Condition))
            {
                EvaluateStatement(node.Body);
            }
        }

        private void EvaluateIfStatement(BoundIfStatement node)
        {
            var condition = (bool)EvaluateExpression(node.Condition);

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
                BoundNodeType.LiteralExpression => EvaluateLiteralExpression((BoundLiteralExpression)node),
                BoundNodeType.VariableExpression => EvaluateVariableExpression((BoundVariableExpression)node),
                BoundNodeType.AssignmentExpression => EvaluateAssignmentExpression((BoundAssignmentExpression)node),
                BoundNodeType.UnaryExpression => EvaluateUnaryExpression((BoundUnaryExpression)node),
                BoundNodeType.BinaryExpression => EvaluateBinaryExpression((BoundBinaryExpression)node),
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
                BoundUnaryOperatorType.OnesComplement => ~(int)operand,
                _ => throw new Exception($"Unexpected unary operator {boundUnaryExpression.Operator.OperatorType}")
            };
        }

        private object EvaluateBinaryExpression(BoundBinaryExpression boundBinaryExpression)
        {
            var left = EvaluateExpression(boundBinaryExpression.Left);
            var right = EvaluateExpression(boundBinaryExpression.Right);

            switch (boundBinaryExpression.Operator.OperatorType)
            {
                case BoundBinaryOperatorType.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperatorType.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorType.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorType.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorType.BitwiseAnd:
                    if (boundBinaryExpression.Type == typeof(int))
                    {
                        return (int)left & (int)right;
                    }
                    else
                    {
                        return (bool)left & (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseOr:
                    if (boundBinaryExpression.Type == typeof(int))
                    {
                        return (int)left | (int)right;
                    }
                    else
                    {
                        return (bool)left | (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseXor:
                    if (boundBinaryExpression.Type == typeof(int))
                    {
                        return (int)left ^ (int)right;
                    }
                    else
                    {
                        return (bool)left ^ (bool)right;
                    }
                case BoundBinaryOperatorType.LogicalAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorType.LogicalOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorType.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorType.NotEquals:
                    return !Equals(left, right);
                case BoundBinaryOperatorType.LessThan:
                    return (int)left < (int)right;
                case BoundBinaryOperatorType.LessThanOrEquals:
                    return (int)left <= (int)right;
                case BoundBinaryOperatorType.GreaterThan:
                    return (int)left > (int)right;
                case BoundBinaryOperatorType.GreaterThanOrEquals:
                    return (int)left >= (int)right;
                default:
                    throw new Exception($"Unexpected binary operator {boundBinaryExpression.Operator.OperatorType}");
            }
        }
    }
}