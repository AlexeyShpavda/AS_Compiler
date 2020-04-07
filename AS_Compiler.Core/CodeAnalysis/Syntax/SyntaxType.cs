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
        OpeningBraceToken,
        ClosingBraceToken,
        IdentifierToken,
        EqualsToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualsEqualsToken,
        BangEqualsToken,

        TrueKeyword,
        FalseKeyword,
        VarKeyword,
        LetKeyword,

        CompilationUnit,

        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,

        ParenthesizedExpression,
        LiteralExpression,
        NameExpression,
        BinaryExpression,
        UnaryExpression,
        AssignmentExpression,
    }
}