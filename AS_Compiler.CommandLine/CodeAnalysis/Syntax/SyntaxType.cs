namespace AS_Compiler.CommandLine.CodeAnalysis.Syntax
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