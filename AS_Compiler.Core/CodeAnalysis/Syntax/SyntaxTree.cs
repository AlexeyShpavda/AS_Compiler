﻿using System.Collections.Generic;
using System.Collections.Immutable;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileSyntaxToken)
        {
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileSyntaxToken = endOfFileSyntaxToken;
        }

        public SyntaxToken EndOfFileSyntaxToken { get; }
        public ExpressionSyntax Root { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);

            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var lexer = new Lexer(text);
            while (true)
            {
                var token = lexer.NextSyntaxToken();
                if (token.Type == SyntaxType.EndOfFileToken)
                    break;

                yield return token;
            }
        }
    }
}