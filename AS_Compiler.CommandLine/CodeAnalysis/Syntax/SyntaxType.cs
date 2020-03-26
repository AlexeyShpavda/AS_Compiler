namespace AS_Compiler.CommandLine.CodeAnalysis.Syntax
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

        TrueKeyword,
        FalseKeyword,

        ParenthesizedExpression,
        LiteralExpression,
        BinaryExpression,
        UnaryExpression
    }
}