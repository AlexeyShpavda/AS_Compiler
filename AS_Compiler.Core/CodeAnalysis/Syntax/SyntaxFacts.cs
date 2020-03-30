﻿using System;
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
            return syntaxType switch
            {
                SyntaxType.StarToken => 5,
                SyntaxType.SlashToken => 5,

                SyntaxType.PlusToken => 4,
                SyntaxType.MinusToken => 4,

                SyntaxType.EqualsEqualsToken => 3,
                SyntaxType.BangEqualsToken => 3,

                SyntaxType.AmpersandAmpersandToken => 2,

                SyntaxType.PipePipeToken => 1,

                _ => 0
            };
        }

        internal static SyntaxType GetKeywordType(string text)
        {
            return text switch
            {
                "true" => SyntaxType.TrueKeyword,
                "false" => SyntaxType.FalseKeyword,
                _ => SyntaxType.IdentifierToken
            };
        }

        public static IEnumerable<SyntaxType> GetBinaryOperators()
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
            switch (syntaxType)
            {
                case SyntaxType.PlusToken:
                    return "+";
                case SyntaxType.MinusToken:
                    return "-";
                case SyntaxType.StarToken:
                    return "*";
                case SyntaxType.SlashToken:
                    return "/";
                case SyntaxType.EqualsToken:
                    return "=";
                case SyntaxType.BangToken:
                    return "!";
                case SyntaxType.AmpersandAmpersandToken:
                    return "&&";
                case SyntaxType.PipePipeToken:
                    return "||";
                case SyntaxType.EqualsEqualsToken:
                    return "==";
                case SyntaxType.BangEqualsToken:
                    return "!=";
                case SyntaxType.OpeningParenthesisToken:
                    return "(";
                case SyntaxType.ClosingParenthesisToken:
                    return ")";
                case SyntaxType.TrueKeyword:
                    return "true";
                case SyntaxType.FalseKeyword:
                    return "false";
                default:
                    return null;
            }
        }
    }
}