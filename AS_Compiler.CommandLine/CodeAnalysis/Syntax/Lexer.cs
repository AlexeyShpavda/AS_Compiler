using System.Collections.Generic;

namespace AS_Compiler.CommandLine.CodeAnalysis.Syntax
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;
        private readonly List<string> _diagnostics = new List<string>();

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current => _position >= _text.Length ? '\0' : _text[_position];
        public List<string> Diagnostics => _diagnostics;

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextSyntaxToken()
        {
            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxType.EndOfFile, _position, "\0", null);
            }

            if (char.IsDigit(Current))
            {
                var start = _position;

                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                if (!int.TryParse(text, out var value))
                {
                    _diagnostics.Add($"ERROR: The number {_text} is not valid Int32");
                }

                return new SyntaxToken(SyntaxType.Number, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
                var start = _position;

                while (char.IsWhiteSpace(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                return new SyntaxToken(SyntaxType.WhiteSpace, start, text, null);
            }

            if (char.IsLetter(Current))
            {
                var start = _position;

                while (char.IsLetter(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);
                var type = SyntaxFacts.GetKeywordType(text);

                return new SyntaxToken(type, start, text, null);
            }

            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxType.Plus, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxType.Minus, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxType.Star, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxType.Slash, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxType.OpeningParenthesis, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxType.ClosingParenthesis, _position++, ")", null);
                default:
                    _diagnostics.Add($"ERROR: bad character input: '{Current}'");
                    return new SyntaxToken(SyntaxType.Unknown, _position++, _text.Substring(_position - 1, 1), null);
            }
        }
    }
}