namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal enum BoundNodeType
    {
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
        GotoStatement,
        LabelStatement,
        ConditionalGotoStatement,

        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
        ErrorExpression,
        CallExpression
    }
}