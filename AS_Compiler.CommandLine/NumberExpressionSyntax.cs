namespace AS_Compiler.CommandLine
{
    public class NumberExpressionSyntax : ExpressionSyntax
    {
        public override SyntaxType SyntaxType => SyntaxType.NumberExpression;
        public SyntaxToken NumberSyntaxToken { get; }

        public NumberExpressionSyntax(SyntaxToken numberSyntaxToken)
        {
            NumberSyntaxToken = numberSyntaxToken;
        }
    }
}