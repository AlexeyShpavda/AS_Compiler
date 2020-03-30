using System.Collections;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using Xunit;

namespace AS_Compiler.Tests.CodeAnalysis.Syntax
{
    public class ParserTests
    {
        [Theory]
        [MemberData(nameof(GetBinaryOperatorPairsData))]
        public void Parser_BinaryExpression_HonorsPrecedences(SyntaxType operator1, SyntaxType operator2)
        {
            var operator1Precedence = operator1.GetBinaryOperatorPrecedence();
            var operator2Precedence = operator2.GetBinaryOperatorPrecedence();

            var operator1Text = SyntaxFacts.GetText(operator1);
            var operator2Text = SyntaxFacts.GetText(operator2);

            var text = $"a {operator1Text} b {operator2Text} c";
            var expression = SyntaxTree.Parse(text).Root;

            if(operator1Precedence >= operator2Precedence)
            {
                using var e = new AssertingEnumerator(expression);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "a");
                e.AssertToken(operator1, operator1Text);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "b");
                e.AssertToken(operator2, operator2Text);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "c");
            }
            else
            {
                using var e = new AssertingEnumerator(expression);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "a");
                e.AssertToken(operator1, operator1Text);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "b");
                e.AssertToken(operator2, operator2Text);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "c");
            }
        }

        [Theory]
        [MemberData(nameof(GetUnaryOperatorPairsData))]
        public void Parser_UnaryExpression_HonorsPrecedences(SyntaxType unaryType, SyntaxType binaryType)
        {
            var unaryPrecedence = unaryType.GetUnaryOperatorPrecedence();
            var binaryPrecedence = binaryType.GetBinaryOperatorPrecedence();

            var unaryText = SyntaxFacts.GetText(unaryType);
            var binaryText = SyntaxFacts.GetText(binaryType);

            var text = $"{unaryText} a {binaryText} b";
            var expression = SyntaxTree.Parse(text).Root;

            if (unaryPrecedence >= binaryPrecedence)
            {
                using var e = new AssertingEnumerator(expression);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.UnaryExpression);
                e.AssertToken(unaryType, unaryText);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "a");
                e.AssertToken(binaryType, binaryText);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "b");
            }
            else
            {
                using var e = new AssertingEnumerator(expression);
                e.AssertNode(SyntaxType.UnaryExpression);
                e.AssertToken(unaryType, unaryText);
                e.AssertNode(SyntaxType.BinaryExpression);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "a");
                e.AssertToken(binaryType, binaryText);
                e.AssertNode(SyntaxType.NameExpression);
                e.AssertToken(SyntaxType.IdentifierToken, "b");
            }
        }

        public static IEnumerable<object[]> GetBinaryOperatorPairsData()
        {
            foreach (var op1 in SyntaxFacts.GetBinaryOperatorTypes())
            {
                foreach (var op2 in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object[] {op1, op2};
                }
            }
        }

        public static IEnumerable<object[]> GetUnaryOperatorPairsData()
        {
            foreach (var unary in SyntaxFacts.GetUnaryOperatorTypes())
            {
                foreach (var binary in SyntaxFacts.GetBinaryOperatorTypes())
                {
                    yield return new object[] { unary, binary };
                }
            }
        }
    }
}