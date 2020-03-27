using System;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public BoundAssignmentExpression(string name, BoundExpression expression)
        {
            Name = name;
            Expression = expression;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.AssignmentExpression;
        public override Type Type => Expression.Type;
        public string Name { get; }
        public BoundExpression Expression { get; }
    }
}