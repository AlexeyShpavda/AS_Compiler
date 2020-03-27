using System.Collections.Generic;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
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

        private char Current => Peek(0);
        private char LookAhead => Peek(1);
        public List<string> Diagnostics => _diagnostics;

        private char Peek(int offset)
        {
            var index = _position + offset;
            return index >= _text.Length ? '\0' : _text[index];
        }

        private void Next()
        {
            _position++;
        }

        public SyntaxToken NextSyntaxToken()
        {
            if (_position >= _text.Length)
            {
                return new SyntaxToken(SyntaxType.EndOfFileToken, _position, "\0", null);
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

                return new SyntaxToken(SyntaxType.NumberToken, start, text, value);
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

                return new SyntaxToken(SyntaxType.WhiteSpaceToken, start, text, null);
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
                    return new SyntaxToken(SyntaxType.PlusToken, _position++, "+", null);
                case '-':
                    return new SyntaxToken(SyntaxType.MinusToken, _position++, "-", null);
                case '*':
                    return new SyntaxToken(SyntaxType.StarToken, _position++, "*", null);
                case '/':
                    return new SyntaxToken(SyntaxType.SlashToken, _position++, "/", null);
                case '(':
                    return new SyntaxToken(SyntaxType.OpeningParenthesisToken, _position++, "(", null);
                case ')':
                    return new SyntaxToken(SyntaxType.ClosingParenthesisToken, _position++, ")", null);
                case '&':
                    if (LookAhead == '&')
                        return new SyntaxToken(SyntaxType.AmpersandAmpersandToken, _position += 2, "&&", null); 
                    break;
                case '|':
                    if (LookAhead == '|')
                        return new SyntaxToken(SyntaxType.PipePipeToken, _position += 2, "||", null);
                    break;
                case '=':
                    if (LookAhead == '=')
                        return new SyntaxToken(SyntaxType.EqualsEqualsToken, _position += 2, "==", null);
                    break;
                case '!':
                    return LookAhead == '='
                        ? new SyntaxToken(SyntaxType.BangEqualsToken, _position += 2, "!=", null)
                        : new SyntaxToken(SyntaxType.BangToken, _position++, "!", null);
            }

            _diagnostics.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxType.UnknownToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}