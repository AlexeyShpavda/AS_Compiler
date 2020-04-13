using System;
using AS_Compiler.Core.CodeAnalysis.Symbols;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public BoundVariableExpression(VariableSymbol variable)
        {
            Variable = variable;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.VariableExpression;
        public override Type Type => Variable.Type;
        public VariableSymbol Variable { get; }
    }
}