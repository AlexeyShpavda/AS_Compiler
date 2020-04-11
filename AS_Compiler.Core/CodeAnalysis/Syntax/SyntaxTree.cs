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