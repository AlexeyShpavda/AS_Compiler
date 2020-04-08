namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        public WhileStatementSyntax(SyntaxToken keyword, ExpressionSyntax condition, StatementSyntax body)
        {
            Keyword = keyword;
            Condition = condition;
            Body = body;
        }

        public override SyntaxType Type => SyntaxType.WhileStatement;
        public SyntaxToken Keyword { get; }
        public ExpressionSyntax Condition { get; }
        public StatementSyntax Body { get; }
    }
}