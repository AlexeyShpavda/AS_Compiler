using System;
using AS_Compiler.CommandLine.CodeAnalysis.Binding;
using AS_Compiler.CommandLine.CodeAnalysis.Syntax;

namespace AS_Compiler.CommandLine.CodeAnalysis
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression _root;

        public Evaluator(BoundExpression root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression n)
            {
                return (int)n.Value;
            }

            if (node is BoundUnaryExpression u)
            {
                var operand = EvaluateExpression(u.Operand);

                return u.OperatorType switch
                {
                    BoundUnaryOperatorType.Identity => operand,
                    BoundUnaryOperatorType.Negation => -operand,
                    _ => throw new Exception($"Unexpected unary operator {u.OperatorType}")
                };
            }

            if (node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

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