using System;
using AS_Compiler.CommandLine.CodeAnalysis.Syntax;

namespace AS_Compiler.CommandLine.CodeAnalysis
{
    public sealed class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is LiteralExpressionSyntax n)
            {
                return (int)n.LiteralSyntaxToken.Value;
            }

            if (node is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);

                switch (u.OperatorToken.Type)
                {
                    case SyntaxType.Plus:
                        return operand;
                    case SyntaxType.Minus:
                        return -operand;
                    default:
                        throw new Exception($"Unexpected unary operator {u.OperatorToken.Type}");
                }
            }

            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch (b.OperatorToken.Type)
                {
                    case SyntaxType.Plus:
                        return left + right;
                    case SyntaxType.Minus:
                        return left - right;
                    case SyntaxType.Star:
                        return left * right;
                    case SyntaxType.Slash:
                        return left / right;
                    default:
                        throw new Exception($"Unexpected binary operator {b.OperatorToken.Type}");
                }
            }

            if (node is ParenthesizedExpressionSyntax p)
            {
                return EvaluateExpression(p.Expression);
            }

            throw new Exception($"Unexpected node {node.Type}");
        }
    }
}