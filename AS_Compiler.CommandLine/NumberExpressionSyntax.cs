namespace AS_Compiler.CommandLine
{
    public sealed class NumberExpressionSyntax : ExpressionSyntax
    {
        public NumberExpressionSyntax(SyntaxToken numberSyntaxToken)
        {
            NumberSyntaxToken = numberSyntaxToken;
        }

        public override SyntaxType SyntaxType => SyntaxType.NumberExpression;
        public SyntaxToken NumberSyntaxToken { get; }
    }
}