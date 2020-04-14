using System;
using System.Collections.Immutable;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal abstract class BoundTreeReWriter
    {
        public virtual BoundStatement RewriteStatement(BoundStatement node)
        {
            switch (node.BoundNodeType)
            {
                case BoundNodeType.BlockStatement:
                    return RewriteBlockStatement((BoundBlockStatement)node);
                case BoundNodeType.VariableDeclaration:
                    return RewriteVariableDeclaration((BoundVariableDeclaration)node);
                case BoundNodeType.IfStatement:
                    return RewriteIfStatement((BoundIfStatement)node);
                case BoundNodeType.WhileStatement:
                    return RewriteWhileStatement((BoundWhileStatement)node);
                case BoundNodeType.ForStatement:
                    return RewriteForStatement((BoundForStatement)node);
                case BoundNodeType.LabelStatement:
                    return RewriteLabelStatement((BoundLabelStatement)node);
                case BoundNodeType.GotoStatement:
                    return RewriteGotoStatement((BoundGotoStatement)node);
                case BoundNodeType.ConditionalGotoStatement:
                    return RewriteConditionalGotoStatement((BoundConditionalGotoStatement)node);
                case BoundNodeType.ExpressionStatement:
                    return RewriteExpressionStatement((BoundExpressionStatement)node);
                default:
                    throw new Exception($"Unexpected node: '{node.BoundNodeType}'");
            }
        }

        protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
        {
            ImmutableArray<BoundStatement>.Builder builder = null;

            for (var i = 0; i < node.Statements.Length; i++)
            {
                var oldStatement = node.Statements[i];
                var newStatement = RewriteStatement(oldStatement);

                if (oldStatement != newStatement)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundStatement>(node.Statements.Length);

                        for (var j = 0; j < i; j++)
                        {
                            builder.Add(node.Statements[j]);
                        }
                    }
                }

                builder?.Add(newStatement);
            }

            return builder == null
                ? node
                : new BoundBlockStatement(builder.MoveToImmutable());
        }

        protected virtual BoundStatement RewriteVariableDeclaration(BoundVariableDeclaration node)
        {
            var initializer = RewriteExpression(node.Initializer);

            return initializer == node.Initializer
                ? node
                : new BoundVariableDeclaration(node.Variable, initializer);
        }

        protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var thenStatement = RewriteStatement(node.ThenStatement);
            var elseStatement = node.ElseStatement == null ? null : RewriteStatement(node.ElseStatement);

            return condition == node.Condition
                   && thenStatement == node.ThenStatement
                   && elseStatement == node.ElseStatement
                ? node
                : new BoundIfStatement(condition, thenStatement, elseStatement);
        }

        protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
        {
            var condition = RewriteExpression(node.Condition);
            var body = RewriteStatement(node.Body);

            return condition == node.Condition
                   && body == node.Body
                ? node
                : new BoundWhileStatement(condition, body);
        }

        protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
        {
            var lowerBound = RewriteExpression(node.LowerBound);
            var upperBound = RewriteExpression(node.UpperBound);
            var body = RewriteStatement(node.Body);

            return lowerBound == node.LowerBound
                   && upperBound == node.UpperBound
                   && body == node.Body
                ? node
                : new BoundForStatement(node.Variable, lowerBound, upperBound, body);
        }

        protected virtual BoundStatement RewriteLabelStatement(BoundLabelStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteGotoStatement(BoundGotoStatement node)
        {
            return node;
        }

        protected virtual BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
        {
            var condition = RewriteExpression(node.Condition);

            return condition == node.Condition
                ? node
                : new BoundConditionalGotoStatement(node.BoundLabel, condition, node.JumpIfTrue);
        }

        protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
        {
            var expression = RewriteExpression(node.Expression);

            return expression == node.Expression
                ? node
                : new BoundExpressionStatement(expression);
        }

        public virtual BoundExpression RewriteExpression(BoundExpression node)
        {
            switch (node.BoundNodeType)
            {
                case BoundNodeType.LiteralExpression:
                    return RewriteLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeType.VariableExpression:
                    return RewriteVariableExpression((BoundVariableExpression)node);
                case BoundNodeType.AssignmentExpression:
                    return RewriteAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeType.UnaryExpression:
                    return RewriteUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeType.BinaryExpression:
                    return RewriteBinaryExpression((BoundBinaryExpression)node);
                case BoundNodeType.ErrorExpression:
                    return RewriteErrorExpression((BoundErrorExpression)node);
                case BoundNodeType.CallExpression:
                    return RewriteCallExpression((BoundCallExpression)node);
                default:
                    throw new Exception($"Unexpected node: '{node.BoundNodeType}'");
            }
        }

        protected virtual BoundExpression RewriteCallExpression(BoundCallExpression node)
        {
            ImmutableArray<BoundExpression>.Builder builder = null;

            for (var i = 0; i < node.Arguments.Length; i++)
            {
                var oldArgument = node.Arguments[i];
                var newArgument = RewriteExpression(oldArgument);

                if (oldArgument != newArgument)
                {
                    if (builder == null)
                    {
                        builder = ImmutableArray.CreateBuilder<BoundExpression>(node.Arguments.Length);

                        for (var j = 0; j < i; j++)
                        {
                            builder.Add(node.Arguments[j]);
                        }
                    }
                }

                builder?.Add(newArgument);
            }

            return builder == null
                ? node
                : new BoundCallExpression(node.Function, builder.MoveToImmutable());
        }

        protected virtual BoundExpression RewriteErrorExpression(BoundErrorExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node)
        {
            return node;
        }

        protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
        {
            var expression = RewriteExpression(node.Expression);

            return expression == node.Expression
                ? node
                : new BoundAssignmentExpression(node.Variable, expression);
        }

        protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
        {
            var expression = RewriteExpression(node.Operand);

            return expression == node.Operand
                ? node
                : new BoundUnaryExpression(node.Operator, expression);
        }

        protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
        {
            var left = RewriteExpression(node.Left);
            var right = RewriteExpression(node.Right);

            return left == node.Left
                   && right == node.Right
                ? node
                : new BoundBinaryExpression(left, node.Operator, right);
        }
    }
}