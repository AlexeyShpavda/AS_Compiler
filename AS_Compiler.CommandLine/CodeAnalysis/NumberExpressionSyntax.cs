using System.Collections.Generic;

namespace AS_Compiler.CommandLine.CodeAnalysis
{
    public sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberSyntaxToken)
        {
            NumberSyntaxToken = numberSyntaxToken;
        }

        public override SyntaxType Type => SyntaxType.NumberExpression;
        public SyntaxToken NumberSyntaxToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberSyntaxToken;
        }
    }
}