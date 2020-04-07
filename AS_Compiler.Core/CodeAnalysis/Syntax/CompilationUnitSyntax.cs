namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(ExpressionSyntax expression, SyntaxToken endOfFileToken)
        {
            Expression = expression;
            EndOfFileToken = endOfFileToken;
        }

        public override SyntaxType Type => SyntaxType.CompilationUnit;
        public ExpressionSyntax Expression { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
}