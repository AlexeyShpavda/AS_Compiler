using System.Collections.Generic;

namespace AS_Compiler.CommandLine
{
    public class Parser
    {
        private readonly SyntaxToken[] _syntaxTokens;
        private int _position;
        private readonly List<string> _diagnostics = new List<string>();

        public Parser(string text)
        {
            var syntaxTokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken syntaxToken;

            do
            {
                syntaxToken = lexer.NextSyntaxToken();

                if (syntaxToken.Type != SyntaxType.WhiteSpace && syntaxToken.Type != SyntaxType.Unknown)
                {
                    syntaxTokens.Add(syntaxToken);
                }
            } while (syntaxToken.Type != SyntaxType.EndOfFile);

            _syntaxTokens = syntaxTokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;

            return index >= _syntaxTokens.Length ? _syntaxTokens[^1] : _syntaxTokens[index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken Match(SyntaxType syntaxType)
        {
            if (Current.Type == syntaxType)
            {
                return NextToken();
            }

            _diagnostics.Add($"ERROR: Unexpected token <{Current.Type}>, expected <{syntaxType}>");
            return new SyntaxToken(syntaxType, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();

            var endOfFileToken = Match(SyntaxType.EndOfFile);

            return new SyntaxTree(Diagnostics, expression, endOfFileToken);

        }

        public ExpressionSyntax ParseExpression()
        {
            var left = ParsePrimaryExpression();

            while (Current.Type == SyntaxType.Plus
                   || Current.Type == SyntaxType.Minus
                   || Current.Type == SyntaxType.Star
                   || Current.Type == SyntaxType.Slash)
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            var numberToken = Match(SyntaxType.Number);

            return new NumberExpressionSyntax(numberToken);
        }
    }
}
