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
                var operand = (int) EvaluateExpression(u.Operand);

                return u.OperatorType switch
                {
                    BoundUnaryOperatorType.Identity => operand,
                    BoundUnaryOperatorType.Negation => -operand,
                    _ => throw new Exception($"Unexpected unary operator {u.OperatorType}")
                };
            }

            if (node is BoundBinaryExpression b)
            {
                var left = (int) EvaluateExpression(b.Left);
                var right = (int) EvaluateExpression(b.Right);

                return b.OperatorType switch
                {
                    BoundBinaryOperatorType.Addition => (left + right),
                    BoundBinaryOperatorType.Subtraction => (left - right),
                    BoundBinaryOperatorType.Multiplication => (left * right),
                    BoundBinaryOperatorType.Division => (left / right),
                    _ => throw new Exception($"Unexpected binary operator {b.OperatorType}")
                };
            }

            throw new Exception($"Unexpected node {node.Type}");
        }
    }
}