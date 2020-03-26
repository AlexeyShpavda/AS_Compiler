using System.Collections.Generic;

namespace AS_Compiler.CommandLine.CodeAnalysis.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalSyntaxToken)
            : this(literalSyntaxToken, literalSyntaxToken.Value)
        {
        }

        public LiteralExpressionSyntax(SyntaxToken literalSyntaxToken, object value)
        {
            LiteralSyntaxToken = literalSyntaxToken;
            Value = value;
        }

        public override SyntaxType Type => SyntaxType.LiteralExpression;
        public SyntaxToken LiteralSyntaxToken { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralSyntaxToken;
        }
    }
}