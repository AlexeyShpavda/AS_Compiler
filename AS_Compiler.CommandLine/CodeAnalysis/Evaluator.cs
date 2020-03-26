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

                switch (u.OperatorType)
                {
                    case BoundUnaryOperatorType.Identity:
                        return operand;
                    case BoundUnaryOperatorType.Negation:
                        return -operand;
                    default:
                        throw new Exception($"Unexpected unary operator {u.OperatorType}");
                }
            }

            if (node is BoundBinaryExpression b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorType)
                {
                    case BoundBinaryOperatorType.Addition:
                        return left + right;
                    case BoundBinaryOperatorType.Subtraction:
                        return left - right;
                    case BoundBinaryOperatorType.Multiplication:
                        return left * right;
                    case BoundBinaryOperatorType.Division:
                        return left / right;
                    default:
                        throw new Exception($"Unexpected binary operator {b.OperatorType}");
                }
            }

            throw new Exception($"Unexpected node {node.Type}");
        }
    }
}