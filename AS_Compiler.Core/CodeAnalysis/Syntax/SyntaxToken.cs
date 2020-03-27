using System.Collections.Generic;
using System.Linq;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxType syntaxType, int position, string text, object value)
        {
            Type = syntaxType;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxType Type { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }
        public TextSpan TextSpan => new TextSpan(Position, Text.Length);

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }
}