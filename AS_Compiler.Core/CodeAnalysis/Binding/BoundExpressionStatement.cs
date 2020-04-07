namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpressionStatement(BoundExpression expression)
        {
            Expression = expression;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.ExpressionStatement;
        public BoundExpression Expression { get; }
    }
}