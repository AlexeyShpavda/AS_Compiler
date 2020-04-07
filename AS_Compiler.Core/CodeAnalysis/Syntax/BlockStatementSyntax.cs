using System.Collections.Immutable;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class BlockStatementSyntax : StatementSyntax
    {
        public BlockStatementSyntax(SyntaxToken openSyntaxToken, ImmutableArray<StatementSyntax> statements, SyntaxToken closeSyntaxToken)
        {
            OpenSyntaxToken = openSyntaxToken;
            Statements = statements;
            CloseSyntaxToken = closeSyntaxToken;
        }

        public override SyntaxType Type => SyntaxType.BlockStatement;
        public SyntaxToken OpenSyntaxToken { get; }
        public ImmutableArray<StatementSyntax> Statements { get; }
        public SyntaxToken CloseSyntaxToken { get; }
    }
}