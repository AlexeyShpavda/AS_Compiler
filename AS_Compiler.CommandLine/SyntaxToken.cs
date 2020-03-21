namespace AS_Compiler.CommandLine
{
    public class SyntaxToken
    {
        public SyntaxType Type { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public SyntaxToken(SyntaxType syntaxKind, int position, string text, object value)
        {
            Type = syntaxKind;
            Position = position;
            Text = text;
            Value = value;
        }
    }
}