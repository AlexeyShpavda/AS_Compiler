namespace AS_Compiler.CommandLine.CodeAnalysis
{
    public enum SyntaxType
    {
        Unknown,
        EndOfFile,
        WhiteSpace,
        Number,
        Plus,
        Minus,
        Star,
        Slash,
        OpeningParenthesis,
        ClosingParenthesis,

        ParenthesizedExpression,
        LiteralExpression,
        BinaryExpression
    }
}