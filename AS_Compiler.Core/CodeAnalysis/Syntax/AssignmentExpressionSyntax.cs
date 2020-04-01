namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class AssignmentExpressionSyntax : ExpressionSyntax
    {
        public AssignmentExpressionSyntax(SyntaxToken identifierToken, SyntaxToken equalsToken, ExpressionSyntax expressionSyntax)
        {
            IdentifierToken = identifierToken;
            EqualsToken = equalsToken;
            ExpressionSyntax = expressionSyntax;
        }

        public override SyntaxType Type => SyntaxType.AssignmentExpression;
        public SyntaxToken IdentifierToken { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax ExpressionSyntax { get; }
    }
}