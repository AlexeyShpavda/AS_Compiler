namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class VariableDeclarationSyntax : StatementSyntax
    {
        public VariableDeclarationSyntax(
            SyntaxToken keyword,
            SyntaxToken identifier,
            SyntaxToken equalsToken,
            ExpressionSyntax initializer)
        {
            Keyword = keyword;
            Identifier = identifier;
            EqualsToken = equalsToken;
            Initializer = initializer;
        }

        public override SyntaxType Type => SyntaxType.VariableDeclaration;
        public SyntaxToken Keyword { get; }
        public SyntaxToken Identifier { get; }
        public SyntaxToken EqualsToken { get; }
        public ExpressionSyntax Initializer { get; }
    }
}