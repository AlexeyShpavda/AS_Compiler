namespace AS_Compiler.CommandLine
{
    public enum SyntaxType
    {
        Number,
        WhiteSpace,
        Plus,
        Minus,
        Star,
        Slash,
        Parenthesized,
        OpeningParenthesis,
        ClosingParenthesis,
        NumberExpression,
        BinaryExpression,
        Unknown,
        EndOfFile
    }
}