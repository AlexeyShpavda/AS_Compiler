using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Binding;
using AS_Compiler.Core.CodeAnalysis.Symbols;
using AS_Compiler.Core.CodeAnalysis.Syntax;

namespace AS_Compiler.Core.CodeAnalysis.Lowering
{
    internal sealed class Lowering : BoundTreeReWriter
    {
        private int _labelCount;

        private Lowering()
        {
        }

        private BoundLabel GenerateLabel()
        {
            var name = $"BoundLabel{++_labelCount}";
            return new BoundLabel(name);
        }

        public static BoundBlockStatement Lower(BoundStatement statement)
        {
            var lowering = new Lowering();
            var result = lowering.RewriteStatement(statement);

            return Flatten(result);
        }

        private static BoundBlockStatement Flatten(BoundStatement statement)
        {
            var builder = ImmutableArray.CreateBuilder<BoundStatement>();
            var stack = new Stack<BoundStatement>();
            stack.Push(statement);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current is BoundBlockStatement blockStatement)
                {
                    foreach (var s in blockStatement.Statements.Reverse())
                    {
                        stack.Push(s);
                    }
                }
                else
                {
                    builder.Add(current);
                }
            }

            return new BoundBlockStatement(builder.ToImmutable());
        }

        protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            if (node.ElseStatement == null)
            {
                var endLabel = GenerateLabel();
                var gotoFalse = new BoundConditionalGotoStatement(endLabel, node.Condition, false);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create(
                    gotoFalse,
                    node.ThenStatement,
                    endLabelStatement));

                return RewriteStatement(result);
            }
            else
            {
                var elseLabel = GenerateLabel();
                var endLabel = GenerateLabel();

                var gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, false);
                var gotoEndStatement = new BoundGotoStatement(endLabel);
                var elseLabelStatement = new BoundLabelStatement(elseLabel);
                var endLabelStatement = new BoundLabelStatement(endLabel);
                var result = new BoundBlockStatement(ImmutableArray.Create(
                    gotoFalse,
                    node.ThenStatement,
                    gotoEndStatement,
                    elseLabelStatement,
                    node.ElseStatement,
                    endLabelStatement));

                return RewriteStatement(result);
            }
        }

        protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var continueLabel = GenerateLabel();
            var checkLabel = GenerateLabel();
            var endLabel = GenerateLabel();

            var gotoCheck = new BoundGotoStatement(checkLabel);
            var continueLabelStatement = new BoundLabelStatement(continueLabel);
            var checkLabelStatement = new BoundLabelStatement(checkLabel);
            var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition);
            var endLabelStatement = new BoundLabelStatement(endLabel);

            var result = new BoundBlockStatement(ImmutableArray.Create(
                gotoCheck,
                continueLabelStatement,
                node.Body,
                checkLabelStatement,
                gotoTrue,
                endLabelStatement));

            return RewriteStatement(result);
        }

        protected override BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var variableExpression = new BoundVariableExpression(node.Variable);
            var variableDeclaration = new BoundVariableDeclaration(node.Variable, node.LowerBound);
            var upperBoundSymbol = new VariableSymbol("upperBound", true, TypeSymbol.Int);
            var upperBoundDeclaration = new BoundVariableDeclaration(upperBoundSymbol, node.UpperBound);

            var condition = new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxType.LessThanOrEqualsToken, TypeSymbol.Int, TypeSymbol.Int),
                new BoundVariableExpression(upperBoundSymbol));

            var increment = new BoundExpressionStatement(
                new BoundAssignmentExpression(
                    node.Variable,
                    new BoundBinaryExpression(
                        variableExpression,
                        BoundBinaryOperator.Bind(SyntaxType.PlusToken, TypeSymbol.Int, TypeSymbol.Int),
                        new BoundLiteralExpression(1))));

            var whileBlock = new BoundBlockStatement(ImmutableArray.Create(node.Body, increment));
            var whileStatement = new BoundWhileStatement(condition, whileBlock);

            var result = new BoundBlockStatement(ImmutableArray.Create<BoundStatement>(
                variableDeclaration,
                upperBoundDeclaration,
                whileStatement));

            return RewriteStatement(result);
        }
    }
}