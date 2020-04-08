namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal enum BoundNodeType
    {
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,
        IfStatement,
        WhileStatement,

        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
    }
}