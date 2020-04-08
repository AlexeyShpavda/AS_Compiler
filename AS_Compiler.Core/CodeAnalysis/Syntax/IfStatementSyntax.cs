namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class IfStatementSyntax : StatementSyntax
    {
        public IfStatementSyntax(
            SyntaxToken ifKeyword,
            ExpressionSyntax condition,
            StatementSyntax thenStatement,
            ElseClauseSyntax elseClause)
        {
            IfKeyword = ifKeyword;
            Condition = condition;
            ThenStatement = thenStatement;
            ElseClause = elseClause;
        }

        public override SyntaxType Type => SyntaxType.IfStatement;
        public SyntaxToken IfKeyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax ThenStatement { get; }
        public ElseClauseSyntax ElseClause { get; }
    }
}