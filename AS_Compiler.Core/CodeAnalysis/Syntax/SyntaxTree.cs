using System.Collections.Generic;
using System.Collections.Immutable;
using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public sealed class SyntaxTree
    {
        private SyntaxTree(SourceText text)
        {
            var parser = new Parser(text);
            var root = parser.ParseCompilationUnit();

            Text = text;
            Diagnostics = parser.Diagnostics.ToImmutableArray();
            Root = root;
        }

        public SourceText Text { get; }
        public CompilationUnitSyntax Root { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }

        public static SyntaxTree Parse(string text)
        {
            var sourceText = SourceText.From(text);
            return Parse(sourceText);
        }

        public static SyntaxTree Parse(SourceText text)
        {
            return new SyntaxTree(text);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(string text)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(string text, out ImmutableArray<Diagnostic> diagnostics)
        {
            var sourceText = SourceText.From(text);
            return ParseTokens(sourceText, out diagnostics);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text)
        {
            return ParseTokens(text, out _);
        }

        public static ImmutableArray<SyntaxToken> ParseTokens(SourceText text, out ImmutableArray<Diagnostic> diagnostics)
        {
            static IEnumerable<SyntaxToken> LexTokens(Lexer lexer)
            {
                while (true)
                {
                    var token = lexer.NextSyntaxToken();
                    if (token.Type == SyntaxType.EndOfFileToken)
                    {
                        break;
                    }

                    yield return token;
                }
            }

            var lexer = new Lexer(text);
            var result = LexTokens(lexer).ToImmutableArray();
            diagnostics = lexer.Diagnostics.ToImmutableArray();

            return result;
        }
    }
}