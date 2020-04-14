using System;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Binding;
using AS_Compiler.Core.CodeAnalysis.Symbols;

namespace AS_Compiler.Core.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundBlockStatement _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        private object _lastValue;

        public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            var labelToIndex = new Dictionary<BoundLabel, int>();

            for (var i = 0; i < _root.Statements.Length; i++)
            {
                if (_root.Statements[i] is BoundLabelStatement l)
                {
                    labelToIndex.Add(l.BoundLabel, i + 1);
                }
            }

            var index = 0;

            while (index < _root.Statements.Length)
            {
                var s = _root.Statements[index];

                switch (s.BoundNodeType)
                {
                    case BoundNodeType.VariableDeclaration:
                        EvaluateVariableDeclaration((BoundVariableDeclaration)s);
                        index++;
                        break;
                    case BoundNodeType.ExpressionStatement:
                        EvaluateExpressionStatement((BoundExpressionStatement)s);
                        index++;
                        break;
                    case BoundNodeType.GotoStatement:
                        var gotoStatement = (BoundGotoStatement)s;
                        index = labelToIndex[gotoStatement.BoundLabel];
                        break;
                    case BoundNodeType.ConditionalGotoStatement:
                        var conditionalGotoStatement = (BoundConditionalGotoStatement)s;
                        var condition = (bool)EvaluateExpression(conditionalGotoStatement.Condition);
                        if (condition == conditionalGotoStatement.JumpIfTrue)
                        {
                            index = labelToIndex[conditionalGotoStatement.BoundLabel];
                        }
                        else
                        {
                            index++;
                        }
                        break;
                    case BoundNodeType.LabelStatement:
                        index++;
                        break;
                    default:
                        throw new Exception($"Unexpected node {s.BoundNodeType}");
                }
            }

            return _lastValue;
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

        private static object EvaluateLiteralExpression(BoundLiteralExpression boundLiteralExpression)
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
                    if (boundBinaryExpression.Type == TypeSymbol.Int)
                    {
                        return (int)left + (int)right;
                    }
                    else
                    {
                        return (string)left + (string)right;
                    }
                case BoundBinaryOperatorType.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorType.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorType.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorType.BitwiseAnd:
                    if (boundBinaryExpression.Type == TypeSymbol.Int)
                    {
                        return (int)left & (int)right;
                    }
                    else
                    {
                        return (bool)left & (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseOr:
                    if (boundBinaryExpression.Type == TypeSymbol.Int)
                    {
                        return (int)left | (int)right;
                    }
                    else
                    {
                        return (bool)left | (bool)right;
                    }
                case BoundBinaryOperatorType.BitwiseXor:
                    if (boundBinaryExpression.Type == TypeSymbol.Int)
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