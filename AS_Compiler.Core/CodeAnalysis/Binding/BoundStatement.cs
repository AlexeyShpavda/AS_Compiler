﻿namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal abstract class BoundStatement : BoundNode
    {

    }

    internal sealed class BoundVariableDeclaration : BoundStatement
    {
        public BoundVariableDeclaration(VariableSymbol variable, BoundExpression initializer)
        {
            Variable = variable;
            Initializer = initializer;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.VariableDeclaration;
        public VariableSymbol Variable { get; }
        public BoundExpression Initializer { get; }
    }
}