namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(BoundLabel boundLabel)
        {
            BoundLabel = boundLabel;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.LabelStatement;
        public BoundLabel BoundLabel { get; }
    }
}