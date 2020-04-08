namespace AS_Compiler.Core.CodeAnalysis.Syntax
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
        LessThanToken,
        LessThanOrEqualsToken,
        GreaterThanToken,
        GreaterThanOrEqualsToken,

        TrueKeyword,
        FalseKeyword,
        VarKeyword,
        LetKeyword,
        IfKeyword,
        ElseKeyword,
        WhileKeyword,

        CompilationUnit,
        ElseClause,

        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,
        IfStatement,
        WhileStatement,

        ParenthesizedExpression,
        LiteralExpression,
        NameExpression,
        BinaryExpression,
        UnaryExpression,
        AssignmentExpression,
    }
}