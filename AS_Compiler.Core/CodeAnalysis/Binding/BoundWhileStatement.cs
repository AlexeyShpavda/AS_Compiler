namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundWhileStatement : BoundStatement
    {
        public BoundWhileStatement(BoundExpression condition, BoundStatement body)
        {
            Condition = condition;
            Body = body;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.WhileStatement;
        public BoundExpression Condition { get; }
        public BoundStatement Body { get; }
    }
}