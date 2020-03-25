using System;
using System.Collections.Generic;
using AS_Compiler.CommandLine.CodeAnalysis.Syntax;

namespace AS_Compiler.CommandLine.CodeAnalysis.Binding
{
    internal enum BoundNodeType
    {
        LiteralExpression,
        UnaryExpression,
        BinaryExpression
    }

    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }
    }

    internal abstract class BoundExpression : BoundNode
    {
        public abstract Type Type { get; }
    }

    internal enum BoundUnaryOperatorType
    {
        Identity,
        Negation
    }

    internal sealed class BoundLiteralExpression : BoundExpression
    {
        public BoundLiteralExpression(object value)
        {
            Value = value;
        }

        public override Type Type => Value.GetType();
        public object Value { get; set; }
        public override BoundNodeType BoundNodeType => BoundNodeType.LiteralExpression;
    }

    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperatorType operatorType, BoundExpression operand)
        {
            OperatorType = operatorType;
            Operand = operand;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.UnaryExpression;
        public override Type Type => Operand.Type;
        public BoundUnaryOperatorType OperatorType { get; set; }
        public BoundExpression Operand { get; }
    }

    internal enum BoundBinaryOperatorType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division
    }

    internal sealed class BoundBinaryExpression : BoundExpression
    {
        public BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorType operatorType, BoundExpression right)
        {
            Left = left;
            OperatorType = operatorType;
            Right = right;
        }

        public override BoundNodeType BoundNodeType => BoundNodeType.BinaryExpression;
        public override Type Type => Left.Type;
        public BoundExpression Left { get; }
        public BoundBinaryOperatorType OperatorType { get; set; }
        public BoundExpression Right { get; }
    }

    internal sealed class Binder
    {
        private readonly List<string> _diagnostics = new List<string>();

        public IEnumerable<string> Diagnostics => _diagnostics;

        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Type)
            {
                case SyntaxType.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax) syntax);
                case SyntaxType.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax) syntax);
                case SyntaxType.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax) syntax);
                default:
                    throw new Exception($"Unexpected syntax {syntax.Type}");
            }
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.LiteralSyntaxToken.Value as int? ?? 0;

            return new BoundLiteralExpression(value);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperatorType = BindUnaryOperatorType(syntax.OperatorToken.Type, boundOperand.Type);

            if (boundOperatorType == null)
            {
                _diagnostics.Add($"Unary operator '{syntax.OperatorToken.Type}' is not defined for type {boundOperand.Type}");
                return boundOperand;
            }

            return new BoundUnaryExpression(boundOperatorType.Value, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperatorType = BindBinaryOperatorType(syntax.OperatorToken.Type, boundLeft.Type, boundRight.Type);

            if (boundOperatorType == null)
            {
                _diagnostics.Add($"Binary operator '{syntax.OperatorToken.Type}' is not defined for type {boundLeft.Type} and { boundRight.Type}");
                return boundLeft;
            }

            return new BoundBinaryExpression(boundLeft, boundOperatorType.Value, boundRight);
        }


        private BoundUnaryOperatorType? BindUnaryOperatorType(SyntaxType type, Type operandType)
        {
            if(operandType != typeof(int))
            {
                return null;
            }

            switch (type)
            {
                case SyntaxType.Plus:
                    return BoundUnaryOperatorType.Identity;
                case SyntaxType.Minus:
                    return BoundUnaryOperatorType.Negation;
                default:
                    throw new Exception($"Unexpected unary operator {type}");
            }
        }

        private BoundBinaryOperatorType? BindBinaryOperatorType(SyntaxType type, Type leftType, Type rightType)
        {
            if (leftType != typeof(int) || rightType != typeof(int))
            {
                return null;
            }

            switch (type)
            {
                case SyntaxType.Plus:
                    return BoundBinaryOperatorType.Addition;
                case SyntaxType.Minus:
                    return BoundBinaryOperatorType.Subtraction;
                case SyntaxType.Star:
                    return BoundBinaryOperatorType.Multiplication;
                case SyntaxType.Slash:
                    return BoundBinaryOperatorType.Division;
                default:
                    throw new Exception($"Unexpected binary operator {type}");
            }
        }
    }
}