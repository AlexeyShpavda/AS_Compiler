using AS_Compiler.Core.CodeAnalysis.Binding;

namespace AS_Compiler.Core.CodeAnalysis.Lowering
{
    internal sealed class Lowering : BoundTreeReWriter
    {
        private Lowering()
        {

        }

        public static BoundStatement Lower(BoundStatement statement)
        {
            var lowering = new Lowering();

            return lowering.RewriteStatement(statement);
        }
    }
}