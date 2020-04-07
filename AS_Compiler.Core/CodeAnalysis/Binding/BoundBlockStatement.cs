using System.Collections.Immutable;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal sealed class BoundBlockStatement : BoundStatement
    {
        public BoundBlockStatement(ImmutableArray<BoundStatement> statements)
        {
            Statements = statements;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.BlockStatement;
        public ImmutableArray<BoundStatement> Statements { get; }
    }
}