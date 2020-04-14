using System;
using System.Collections.Generic;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxType syntaxType)
        {
            switch (syntaxType)
            {
                case SyntaxType.PlusToken:
                case SyntaxType.MinusToken:
                case SyntaxType.BangToken:
                case SyntaxType.TildeToken:
                    return 6;
                default:
                    return 0;
            }
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
                case SyntaxType.AmpersandToken:
                case SyntaxType.AmpersandAmpersandToken:
                    return 2;
                case SyntaxType.PipeToken:
                case SyntaxType.PipePipeToken:
                case SyntaxType.HatToken:
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
                "if" => SyntaxType.IfKeyword,
                "else" => SyntaxType.ElseKeyword,
                "while" => SyntaxType.WhileKeyword,
                "for" => SyntaxType.ForKeyword,
                "to" => SyntaxType.ToKeyword,
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
            var types = (SyntaxType[])Enum.GetValues(typeof(SyntaxType));
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
                SyntaxType.TildeToken => "~",
                SyntaxType.EqualsToken => "=",
                SyntaxType.LessThanToken => "<",
                SyntaxType.LessThanOrEqualsToken => "<=",
                SyntaxType.GreaterThanToken => ">",
                SyntaxType.GreaterThanOrEqualsToken => ">=",
                SyntaxType.BangToken => "!",
                SyntaxType.HatToken => "^",
                SyntaxType.AmpersandToken => "&",
                SyntaxType.PipeToken => "|",
                SyntaxType.AmpersandAmpersandToken => "&&",
                SyntaxType.PipePipeToken => "||",
                SyntaxType.EqualsEqualsToken => "==",
                SyntaxType.BangEqualsToken => "!=",
                SyntaxType.OpeningParenthesisToken => "(",
                SyntaxType.ClosingParenthesisToken => ")",
                SyntaxType.OpeningBraceToken => "{",
                SyntaxType.ClosingBraceToken => "}",
                SyntaxType.CommaToken => ",",
                SyntaxType.TrueKeyword => "true",
                SyntaxType.FalseKeyword => "false",
                SyntaxType.VarKeyword => "var",
                SyntaxType.LetKeyword => "let",
                SyntaxType.IfKeyword => "if",
                SyntaxType.ElseKeyword => "else",
                SyntaxType.WhileKeyword => "while",
                SyntaxType.ForKeyword => "for",
                SyntaxType.ToKeyword => "to",
                _ => null
            };
        }
    }
}