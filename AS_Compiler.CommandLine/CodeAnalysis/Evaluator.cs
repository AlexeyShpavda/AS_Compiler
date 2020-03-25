using System;

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
                return (int)n.NumberSyntaxToken.Value;
            }

            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Type == SyntaxType.Plus)
                {
                    return left + right;
                }
                else if (b.OperatorToken.Type == SyntaxType.Minus)
                {
                    return left - right;
                }
                else if (b.OperatorToken.Type == SyntaxType.Star)
                {
                    return left * right;
                }
                else if (b.OperatorToken.Type == SyntaxType.Slash)
                {
                    return left / right;
                }
                else
                {
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