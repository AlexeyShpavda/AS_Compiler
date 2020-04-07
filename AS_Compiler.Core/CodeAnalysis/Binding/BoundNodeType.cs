namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal enum BoundNodeType
    {
        BlockStatement,
        VariableDeclaration,
        ExpressionStatement,

        LiteralExpression,
        VariableExpression,
        AssignmentExpression,
        UnaryExpression,
        BinaryExpression,
    }
}