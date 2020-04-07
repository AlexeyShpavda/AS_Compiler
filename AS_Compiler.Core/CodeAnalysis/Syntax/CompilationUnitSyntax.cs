namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class CompilationUnitSyntax : SyntaxNode
    {
        public CompilationUnitSyntax(StatementSyntax statement, SyntaxToken endOfFileToken)
        {
            Statement = statement;
            EndOfFileToken = endOfFileToken;
        }

        public override SyntaxType Type => SyntaxType.CompilationUnit;
        public StatementSyntax Statement { get; }
        public SyntaxToken EndOfFileToken { get; }
    }
}