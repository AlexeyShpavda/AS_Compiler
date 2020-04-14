using System.Text;
using AS_Compiler.Core.CodeAnalysis.Symbols;
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
        private char Lookahead => Peek(1);
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
                case ',':
                    _type = SyntaxType.CommaToken;
                    _position++;
                    break;
                case '~':
                    _type = SyntaxType.TildeToken;
                    _position++;
                    break;
                case '^':
                    _type = SyntaxType.HatToken;
                    _position++;
                    break;
                case '&':
                    _position++;
                    if (Current == '&')
                    {
                        _type = SyntaxType.AmpersandAmpersandToken;
                        _position++;
                    }
                    else
                    {
                        _type = SyntaxType.AmpersandToken;
                    }
                    break;
                case '|':
                    _position++;
                    if (Current == '|')
                    {
                        _type = SyntaxType.PipePipeToken;
                        _position++;
                    }
                    else
                    {
                        _type = SyntaxType.PipeToken;
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
                case '<':
                    _position++;
                    if (Current == '=')
                    {
                        _type = SyntaxType.LessThanOrEqualsToken;
                        _position++;
                    }
                    else
                    {
                        _type = SyntaxType.LessThanToken;
                    }
                    break;
                case '>':
                    _position++;
                    if (Current == '=')
                    {
                        _type = SyntaxType.GreaterThanOrEqualsToken;
                        _position++;
                    }
                    else
                    {
                        _type = SyntaxType.GreaterThanToken;
                    }
                    break;
                case '"':
                    ReadString();
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

        private void ReadString()
        {
            _position++;
            var stringBuilder = new StringBuilder();

            var done = false;

            while (!done)
            {
                switch (Current)
                {
                    case '\0':
                    case '\r':
                    case '\n':
                        var textSpan = new TextSpan(_start, 1);
                        Diagnostics.ReportUnterminatedString(textSpan);
                        done = true;
                        break;
                    case '"':
                        if(Lookahead == '"')
                        {
                            stringBuilder.Append(Current);
                            _position += 2;
                        }
                        else
                        {
                            _position++;
                            done = true;
                        }
                        break;
                    default:
                        stringBuilder.Append(Current);
                        _position++;
                        break;
                }
            }

            _type = SyntaxType.StringToken;
            _value = stringBuilder.ToString();
        }

        private void ReadNumberToken()
        {
            while (char.IsDigit(Current))
                _position++;

            var length = _position - _start;
            var text = _text.ToString(_start, length);

            if (!int.TryParse(text, out var value))
            {
                Diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, TypeSymbol.Int);
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