﻿namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public enum SyntaxType
    {
        UnknownToken,
        EndOfFileToken,
        WhiteSpaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        OpeningParenthesisToken,
        ClosingParenthesisToken,
        IdentifierToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualsToken,

        TrueKeyword,
        FalseKeyword,

        ParenthesizedExpression,
        LiteralExpression,
        BinaryExpression,
        UnaryExpression
    }
}