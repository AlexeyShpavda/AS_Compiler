﻿using System.Collections.Generic;

namespace AS_Compiler.CommandLine.CodeAnalysis.Syntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        public LiteralExpressionSyntax(SyntaxToken literalSyntaxToken)
        {
            LiteralSyntaxToken = literalSyntaxToken;
        }

        public override SyntaxType Type => SyntaxType.LiteralExpression;
        public SyntaxToken LiteralSyntaxToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return LiteralSyntaxToken;
        }
    }
}