namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(BoundLabel boundLabel)
        {
            BoundLabel = boundLabel;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.GotoStatement;
        public BoundLabel BoundLabel { get; }
    }
}