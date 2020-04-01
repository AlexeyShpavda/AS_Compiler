using System.Collections.Generic;
using System.Collections.Immutable;
using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        public SyntaxTree(SourceText text, ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileSyntaxToken)
        {
            Text = text;
            Diagnostics = diagnostics;
            Root = root;
            EndOfFileSyntaxToken = endOfFileSyntaxToken;
        }

        public SourceText Text { get; }
        public SyntaxToken EndOfFileSyntaxToken { get; }
        public ExpressionSyntax Root { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            var parser = new Parser(text);
            return parser.Parse();
        }

        public static IEnumerable<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText);
        }

        public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
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