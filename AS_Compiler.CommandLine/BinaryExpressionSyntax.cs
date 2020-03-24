namespace AS_Compiler.CommandLine
{
    public sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxNode operatorToken, ExpressionSyntax right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }

        public override SyntaxType SyntaxType => SyntaxType.BinaryExpression;
        public ExpressionSyntax Right { get; }
        public SyntaxNode OperatorToken { get; }
        public ExpressionSyntax Left { get; }
    }
}