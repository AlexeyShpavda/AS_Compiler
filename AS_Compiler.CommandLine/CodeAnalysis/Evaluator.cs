using System;
using AS_Compiler.CommandLine.CodeAnalysis.Binding;

namespace AS_Compiler.CommandLine.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        public object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression n)
            {
                return n.Value;
            }

            if (node is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);

                return u.Operator.OperatorType switch
                {
                    BoundUnaryOperatorType.Identity => (int) operand,
                    BoundUnaryOperatorType.Negation => -(int) operand,
                    BoundUnaryOperatorType.LogicalNegation => !(bool) operand,
                    _ => throw new Exception($"Unexpected unary operator {u.Operator.OperatorType}")
                };
            }

            if (node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                return b.Operator.OperatorType switch
                {
                    BoundBinaryOperatorType.Addition => (int) left + (int) right,
                    BoundBinaryOperatorType.Subtraction => (int) left - (int) right,
                    BoundBinaryOperatorType.Multiplication => (int) left * (int) right,
                    BoundBinaryOperatorType.Division => (int) left / (int) right,
                    BoundBinaryOperatorType.LogicalAnd => (bool)left && (bool)right,
                    BoundBinaryOperatorType.LogicalOr => (bool)left || (bool)right,
                    BoundBinaryOperatorType.Equals => Equals(left, right),
                    BoundBinaryOperatorType.NotEquals => !Equals(left, right),
                    _ => throw new Exception($"Unexpected binary operator {b.Operator.OperatorType}")
                };
            }

            throw new Exception($"Unexpected node {node.Type}");
        }
    }
}