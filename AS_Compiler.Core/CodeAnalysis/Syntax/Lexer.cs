using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public class Lexer
    {
        private readonly SourceText _text;

        private int _position;

        private int _start;
        private SyntaxType _type;
        private object _value;

        public Lexer(SourceText text)
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

        public SyntaxToken NextSyntaxToken()
        {
            _start = _position;
            _type = SyntaxType.UnknownToken;
            _value = null;

            switch (Current)
            {
                case '\0':
                    _type = SyntaxType.EndOfFileToken;
                    break;
                case '+':
                    _type = SyntaxType.PlusToken;
                    _position++;
                    break;
                case '-':
                    _type = SyntaxType.MinusToken;
                    _position++;
                    break;
                case '*':
                    _type = SyntaxType.StarToken;
                    _position++;
                    break;
                case '/':
                    _type = SyntaxType.SlashToken;
                    _position++;
                    break;
                case '(':
                    _type = SyntaxType.OpeningParenthesisToken;
                    _position++;
                    break;
                case ')':
                    _type = SyntaxType.ClosingParenthesisToken;
                    _position++;
                    break;
                case '{':
                    _type = SyntaxType.OpeningBraceToken;
                    _position++;
                    break;
                case '}':
                    _type = SyntaxType.ClosingBraceToken;
                    _position++;
                    break;
                case '&':
                    if (LookAhead == '&')
                    {
                        _type = SyntaxType.AmpersandAmpersandToken;
                        _position += 2;
                    }
                    break;
                case '|':
                    if (LookAhead == '|')
                    {
                        _type = SyntaxType.PipePipeToken;
                        _position += 2;
                    }
                    break;
                case '=':
                    _position++;
                    if (Current == '=')
                    {
                        _type = SyntaxType.EqualsEqualsToken;
                        _position++;
                    }
                    else
                    {
                        _type = SyntaxType.EqualsToken;
                    }
                    break;
                case '!':
                    _position++;
                    if (Current == '=')
                    {
                        _type = SyntaxType.BangEqualsToken;
                        _position++;
                    }
                    else
                    {
                        _type = SyntaxType.BangToken;
                    }
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    ReadNumberToken();
                    break;
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    ReadWhiteSpaceToken();
                    break;
                default:
                    if (char.IsLetter(Current))
                    {
                        ReadIdentifierOrKeyword();
                    }
                    else if (char.IsWhiteSpace(Current))
                    {
                        ReadWhiteSpaceToken();
                    }
                    else
                    {
                        Diagnostics.ReportBadCharacter(_position, Current);
                        _position++;
                    }
                    break;
            }

            var length = _position - _start;
            var text = SyntaxFacts.GetText(_type) ?? _text.ToString(_start, length);

            return new SyntaxToken(_type, _start, text, _value);
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);

            if (!int.TryParse(text, out var value))
            {
                Diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));
            }

            _value = value;
            _type = SyntaxType.NumberToken;
        }

        private void ReadWhiteSpaceToken()
        {
            while (char.IsWhiteSpace(Current))
                _position++;

            _type = SyntaxType.WhiteSpaceToken;
        }

        private void ReadIdentifierOrKeyword()
        {
            while (char.IsLetter(Current))
                _position++; 
            
            var length = _position - _start;
            var text = _text.ToString(_start, length);
            _type = SyntaxFacts.GetKeywordType(text);
        }
    }
}