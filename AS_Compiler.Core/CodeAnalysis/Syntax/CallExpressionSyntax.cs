namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class CallExpressionSyntax : ExpressionSyntax
    {
        public CallExpressionSyntax(
            SyntaxToken identifier,
            SyntaxToken openingParenthesisToken,
            SeparatedSyntaxList<ExpressionSyntax> arguments,
            SyntaxToken closingParenthesisToken)
        {
            Identifier = identifier;
            OpeningParenthesisToken = openingParenthesisToken;
            Arguments = arguments;
            ClosingParenthesisToken = closingParenthesisToken;
        }

        public override SyntaxType Type => SyntaxType.CallExpression;
        public SyntaxToken Identifier { get; }
        public SyntaxToken OpeningParenthesisToken { get; }
        public SeparatedSyntaxList<ExpressionSyntax> Arguments { get; }
        public SyntaxToken ClosingParenthesisToken { get; }
    }
}