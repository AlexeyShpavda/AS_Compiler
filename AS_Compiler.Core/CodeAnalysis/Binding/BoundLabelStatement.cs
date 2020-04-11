namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.LabelStatement;
        public LabelSymbol Label { get; }
    }
}