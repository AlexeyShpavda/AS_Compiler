namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
    }
}