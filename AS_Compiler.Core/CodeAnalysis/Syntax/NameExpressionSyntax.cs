﻿namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken identifierToken)
        {
            IdentifierToken = identifierToken;
        }

        public override SyntaxType Type => SyntaxType.NameExpression;
        public SyntaxToken IdentifierToken { get; }
    }
}