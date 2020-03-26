namespace AS_Compiler.CommandLine.CodeAnalysis.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
    }
}