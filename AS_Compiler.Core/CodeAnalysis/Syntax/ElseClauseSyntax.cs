namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class ElseClauseSyntax : SyntaxNode
    {
        public ElseClauseSyntax(SyntaxToken elseKeyword, StatementSyntax elseStatement)
        {
            ElseKeyword = elseKeyword;
            ElseStatement = elseStatement;
        }

        public override SyntaxType Type => SyntaxType.ElseClause;
        public SyntaxToken ElseKeyword { get; }
        public StatementSyntax ElseStatement { get; }
    }
}