namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundGotoStatement : BoundStatement
    {
        public BoundGotoStatement(LabelSymbol label)
        {
            Label = label;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.GotoStatement;
        public LabelSymbol Label { get; }
    }
}