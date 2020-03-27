namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public class Lexer
    {
        private readonly string _text;
        private int _position;

        public Lexer(string text)
        {
            _text = text;
        }

        private char Current => Peek(0);
        private char LookAhead => Peek(1);
        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

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
            
            var start = _position;

            if (char.IsDigit(Current))
            {
                while (char.IsDigit(Current))
                {
                    Next();
                }

                var length = _position - start;
                var text = _text.Substring(start, length);

                if (!int.TryParse(text, out var value))
                {
                    Diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));
                }

                return new SyntaxToken(SyntaxType.NumberToken, start, text, value);
            }

            if (char.IsWhiteSpace(Current))
            {
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
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxType.AmpersandAmpersandToken, start, "&&", null); 
                    }
                    break;
                case '|':
                    if (LookAhead == '|')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxType.PipePipeToken, start, "||", null);
                    }
                    break;
                case '=':
                    if (LookAhead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxType.EqualsEqualsToken, start, "==", null);
                    }
                    break;
                case '!':
                    if (LookAhead == '=')
                    {
                        _position += 2;
                        return new SyntaxToken(SyntaxType.BangEqualsToken, start, "!=", null);
                    }
                    else
                    {
                        _position += 1;
                        return new SyntaxToken(SyntaxType.BangToken, start, "!", null);
                    }
            }

            Diagnostics.ReportBadCharacter(_position, Current);
            return new SyntaxToken(SyntaxType.UnknownToken, _position++, _text.Substring(_position - 1, 1), null);
        }
    }
}