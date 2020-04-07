namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        public ExpressionStatementSyntax(ExpressionSyntax expression)
        {
            Expression = expression;
        }

        public override SyntaxType Type => SyntaxType.ExpressionStatement;
        public ExpressionSyntax Expression { get; }
    }
}