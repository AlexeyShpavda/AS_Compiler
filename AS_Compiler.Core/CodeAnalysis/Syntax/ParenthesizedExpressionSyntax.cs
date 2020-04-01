namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisToken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisToken = openParenthesisToken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public override SyntaxType Type => SyntaxType.ParenthesizedExpression;
        public SyntaxToken OpenParenthesisToken { get; set; }
        public ExpressionSyntax Expression { get; set; }
        public SyntaxToken CloseParenthesisToken { get; set; }
    }
}