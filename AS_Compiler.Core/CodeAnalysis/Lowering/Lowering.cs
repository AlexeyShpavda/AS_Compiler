using System.Collections.Immutable;
using AS_Compiler.Core.CodeAnalysis.Binding;
using AS_Compiler.Core.CodeAnalysis.Syntax;

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

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {

            var variableExpression = new BoundVariableExpression(node.Variable);
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);

            var condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxType.LessThanOrEqualsToken, typeof(int), typeof(int)),
                node.UpperBound);

            var increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.Variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxType.PlusToken, typeof(int), typeof(int)),
                        new BoundLiteralExpression(1))));

            var whileBlock = new BoundBlockStatement(ImmutableArray.Create(node.Body, increment));
            var whileStatement = new BoundWhileStatement(condition, whileBlock);
            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(variableDeclaration, whileStatement));

            return RewriteStatement(result);
        }
    }
}