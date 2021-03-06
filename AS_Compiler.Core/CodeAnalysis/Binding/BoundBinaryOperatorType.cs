﻿namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal enum BoundBinaryOperatorType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        LogicalAnd,
        LogicalOr,
        Equals,
        NotEquals,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
        BitwiseAnd,
        BitwiseOr,
        BitwiseXor
    }
}