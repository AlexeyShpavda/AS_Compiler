using System;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Binding;

namespace AS_Compiler.Core.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _variables;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
        {
            _root = root;
            _variables = variables;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
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
                _ => throw new Exception($"Unexpected binary operator {boundBinaryExpression.Operator.OperatorType}")
            };
        }
    }
}