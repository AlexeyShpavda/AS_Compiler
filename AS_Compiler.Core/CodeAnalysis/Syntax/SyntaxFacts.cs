using System;
using System.Collections.Generic;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxType syntaxType)
        {
            return syntaxType switch
            {
                SyntaxType.PlusToken => 6,
                SyntaxType.MinusToken => 6,
                SyntaxType.BangToken => 6,

                _ => 0
            };
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxType syntaxType)
        {
            switch (syntaxType)
            {
                case SyntaxType.StarToken:
                case SyntaxType.SlashToken:
                    return 5;
                case SyntaxType.PlusToken:
                case SyntaxType.MinusToken:
                    return 4;
                case SyntaxType.EqualsEqualsToken:
                case SyntaxType.BangEqualsToken:
                case SyntaxType.LessThanToken:
                case SyntaxType.LessThanOrEqualsToken:
                case SyntaxType.GreaterThanToken:
                case SyntaxType.GreaterThanOrEqualsToken:
                    return 3;
                case SyntaxType.AmpersandAmpersandToken:
                    return 2;
                case SyntaxType.PipePipeToken:
                    return 1;
                default:
                    return 0;
            }
        }

        internal static SyntaxType GetKeywordType(string text)
        {
            return text switch
            {
                "true" => SyntaxType.TrueKeyword,
                "false" => SyntaxType.FalseKeyword,
                "var" => SyntaxType.VarKeyword,
                "let" => SyntaxType.LetKeyword,
                _ => SyntaxType.IdentifierToken
            };
        }

        public static IEnumerable<SyntaxType> GetUnaryOperatorTypes()
        {
            var types = (SyntaxType[])Enum.GetValues(typeof(SyntaxType));
            foreach (var type in types)
            {
                if (GetUnaryOperatorPrecedence(type) > 0)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<SyntaxType> GetBinaryOperatorTypes()
        {
            var types = (SyntaxType[]) Enum.GetValues(typeof(SyntaxType));
            foreach (var type in types)
            {
                if (GetBinaryOperatorPrecedence(type) > 0)
                {
                    yield return type;
                }
            }
        }

        public static string GetText(SyntaxType syntaxType)
        {
            return syntaxType switch
            {
                SyntaxType.PlusToken => "+",
                SyntaxType.MinusToken => "-",
                SyntaxType.StarToken => "*",
                SyntaxType.SlashToken => "/",
                SyntaxType.EqualsToken => "=",
                SyntaxType.LessThanToken => "<",
                SyntaxType.LessThanOrEqualsToken => "<=",
                SyntaxType.GreaterThanToken => ">",
                SyntaxType.GreaterThanOrEqualsToken => ">=",
                SyntaxType.BangToken => "!",
                SyntaxType.AmpersandAmpersandToken => "&&",
                SyntaxType.PipePipeToken => "||",
                SyntaxType.EqualsEqualsToken => "==",
                SyntaxType.BangEqualsToken => "!=",
                SyntaxType.OpeningParenthesisToken => "(",
                SyntaxType.ClosingParenthesisToken => ")",
                SyntaxType.OpeningBraceToken => "{",
                SyntaxType.ClosingBraceToken => "}",
                SyntaxType.TrueKeyword => "true",
                SyntaxType.FalseKeyword => "false",
                SyntaxType.VarKeyword => "var",
                SyntaxType.LetKeyword => "let",
                _ => null
            };
        }
    }
}