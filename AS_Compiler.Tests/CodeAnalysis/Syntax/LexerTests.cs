using System;
using System.Collections.Generic;
using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using Xunit;

namespace AS_Compiler.Tests.CodeAnalysis.Syntax
{
    public class LexerTests
    {
        [Fact]
        public void Lexer_Tests_AllTokens()
        {
            var tokenTypes = Enum.GetValues(typeof(SyntaxType))
                .Cast<SyntaxType>()
                .Where(t => t.ToString().EndsWith("Keyword") || t.ToString().EndsWith("Token"))
                .ToList();

            var testedTokenTypes = GetTokens()
                .Concat(GetSeparators())
                .Select(t => t.syntaxType);

            var untestedTokenTypes = new SortedSet<SyntaxType>(tokenTypes);
            untestedTokenTypes.Remove(SyntaxType.UnknownToken);
            untestedTokenTypes.Remove(SyntaxType.EndOfFileToken);
            untestedTokenTypes.ExceptWith(testedTokenTypes);

            Assert.Empty(untestedTokenTypes);
        }

        [Theory]
        [MemberData(nameof(GetTokensData))]
        public void Lexer_Lex_Token(SyntaxType syntaxType, string text)
        {
            var tokens = SyntaxTree.ParseTokens(text);

            var token = Assert.Single(tokens);

            if (token == null)
            {
                throw new Exception("Token does not exist.");
            }

            Assert.Equal(syntaxType, token.Type);
            Assert.Equal(text, token.Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsData))]
        public void Lexer_Lex_TokenPairs_WithSeparators(
            SyntaxType syntaxType1, string text1,
            SyntaxType syntaxType2, string text2)
        {
            var text = text1 + text2;
            var tokens = SyntaxTree.ParseTokens(text).ToList();

            Assert.Equal(2, tokens.Count);
            Assert.Equal(syntaxType1, tokens[0].Type);
            Assert.Equal(text1, tokens[0].Text);
            Assert.Equal(syntaxType2, tokens[1].Type);
            Assert.Equal(text2, tokens[1].Text);
        }

        [Theory]
        [MemberData(nameof(GetTokenPairsWithSeparatorData))]
        public void Lexer_Lex_TokenPairs(
            SyntaxType syntaxType1, string text1,
            SyntaxType separatorType, string separatorText,
            SyntaxType syntaxType2, string text2)
        {
            var text = text1 + separatorText + text2;
            var tokens = SyntaxTree.ParseTokens(text).ToList();

            Assert.Equal(3, tokens.Count);
            Assert.Equal(syntaxType1, tokens[0].Type);
            Assert.Equal(text1, tokens[0].Text);
            Assert.Equal(separatorType, tokens[1].Type);
            Assert.Equal(separatorText, tokens[1].Text);
            Assert.Equal(syntaxType2, tokens[2].Type);
            Assert.Equal(text2, tokens[2].Text);
        }

        public static IEnumerable<object[]> GetTokensData()
        {
            return GetTokens().Concat(GetSeparators()).Select(t => new object[] { t.syntaxType, t.text });
        }

        public static IEnumerable<object[]> GetTokenPairsData()
        {
            return GetTokenPairs().Select(t => new object[] { t.syntaxType1, t.text1, t.syntaxType2, t.text2 });
        }

        public static IEnumerable<object[]> GetTokenPairsWithSeparatorData()
        {
            return GetTokenPairsWithSeparator().Select(t => new object[] { t.syntaxType1, t.text1, t.separatorType, t.separatorText, t.syntaxType2, t.text2 });
        }

        private static IEnumerable<(SyntaxType syntaxType, string text)> GetTokens()
        {
            var fixedTokens = Enum.GetValues(typeof(SyntaxType))
                .Cast<SyntaxType>()
                .Select(t => (sytnaxType: t, text: SyntaxFacts.GetText(t)))
                .Where(t => t.text != null);

            var dynamicTokens = new[]
            {
                (SyntaxType.NumberToken, "1"),
                (SyntaxType.NumberToken, "123"),
                (SyntaxType.IdentifierToken, "a"),
                (SyntaxType.IdentifierToken, "abc")
            };

            return fixedTokens.Concat(dynamicTokens);
        }

        private static IEnumerable<(SyntaxType syntaxType, string text)> GetSeparators()
        {
            return new[]
            {
                (SyntaxType.WhiteSpaceToken, " "),
                (SyntaxType.WhiteSpaceToken, "   "),
                (SyntaxType.WhiteSpaceToken, "\r"),
                (SyntaxType.WhiteSpaceToken, "\n"),
                (SyntaxType.WhiteSpaceToken, "\r\n"),
                (SyntaxType.WhiteSpaceToken, "\r")
            };
        }

        private static bool RequiresSeparator(SyntaxType syntaxType1, SyntaxType syntaxType2)
        {
            var isSyntaxType1Keyword = syntaxType1.ToString().EndsWith("Keyword");
            var isSyntaxType2Keyword = syntaxType2.ToString().EndsWith("Keyword");

            if (syntaxType1 == SyntaxType.IdentifierToken && syntaxType2 == SyntaxType.IdentifierToken)
            {
                return true;
            }

            if (isSyntaxType1Keyword && isSyntaxType2Keyword)
            {
                return true;
            }

            if (isSyntaxType1Keyword && syntaxType2 == SyntaxType.IdentifierToken)
            {
                return true;
            }

            if (isSyntaxType2Keyword && syntaxType1 == SyntaxType.IdentifierToken)
            {
                return true;
            }

            if (isSyntaxType2Keyword && syntaxType1 == SyntaxType.IdentifierToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.NumberToken && syntaxType2 == SyntaxType.NumberToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.BangToken && syntaxType2 == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.BangToken && syntaxType2 == SyntaxType.EqualsEqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.EqualsToken && syntaxType2 == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.EqualsToken && syntaxType2 == SyntaxType.EqualsEqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.LessThanToken && syntaxType2 == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.LessThanToken && syntaxType2 == SyntaxType.EqualsEqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.GreaterThanToken && syntaxType2 == SyntaxType.EqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.GreaterThanToken && syntaxType2 == SyntaxType.EqualsEqualsToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.AmpersandToken && syntaxType2 == SyntaxType.AmpersandToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.AmpersandToken && syntaxType2 == SyntaxType.AmpersandAmpersandToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.PipeToken && syntaxType2 == SyntaxType.PipeToken)
            {
                return true;
            }

            if (syntaxType1 == SyntaxType.PipeToken && syntaxType2 == SyntaxType.PipePipeToken)
            {
                return true;
            }

            return false;
        }

        private static IEnumerable<(SyntaxType syntaxType1, string text1,
                                    SyntaxType syntaxType2, string text2)> GetTokenPairs()
        {
            foreach (var (syntaxType1, text1) in GetTokens())
            {
                foreach (var (syntaxType2, text2) in GetTokens())
                {
                    if (!RequiresSeparator(syntaxType1, syntaxType2))
                    {
                        yield return (syntaxType1, text1, syntaxType2, text2);
                    }
                }
            }
        }

        private static IEnumerable<(SyntaxType syntaxType1, string text1,
                                    SyntaxType separatorType, string separatorText,
                                    SyntaxType syntaxType2, string text2)> GetTokenPairsWithSeparator()
        {
            foreach (var (syntaxType1, text1) in GetTokens())
            {
                foreach (var (syntaxType2, text2) in GetTokens())
                {
                    if (RequiresSeparator(syntaxType1, syntaxType2))
                    {
                        foreach (var (separatorType, separatorText) in GetSeparators())
                        {
                            yield return (syntaxType1, text1, separatorType, separatorText, syntaxType2, text2);
                        }
                    }
                }
            }
        }
    }
}