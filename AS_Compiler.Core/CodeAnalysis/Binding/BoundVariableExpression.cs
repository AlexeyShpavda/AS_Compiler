using System;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(string name, Type type)
        {
            Name = name;
            Type = type;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.VariableExpression;
        public string Name { get; }
        public override Type Type { get; }
    }
}