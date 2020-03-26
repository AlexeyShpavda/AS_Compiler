﻿namespace AS_Compiler.CommandLine.CodeAnalysis.Syntax
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
                    return 5;
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
                    return 4;
                case SyntaxType.PlusToken:
                case SyntaxType.MinusToken:
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
                _ => SyntaxType.IdentifierToken
            };
        }
    }
}