using System.Collections.Generic;
using System.Linq;

namespace AS_Compiler.CommandLine
{
    public class SyntaxTree
    {
        public SyntaxTree(IEnumerable<string> diagnostics, ExpressionSyntax root, SyntaxToken endOfFileSyntaxToken)
        {
            Diagnostics = diagnostics.ToList();
            Root = root;
            EndOfFileSyntaxToken = endOfFileSyntaxToken;
        }

        public SyntaxToken EndOfFileSyntaxToken { get; }
        public ExpressionSyntax Root { get; }
        public IReadOnlyList<string> Diagnostics { get; }

        public static SyntaxTree Parse(string text)
        {
            var parser = new Parser(text);

            return parser.Parse();
        }
    }
}