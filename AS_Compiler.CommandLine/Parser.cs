using System.Collections.Generic;

namespace AS_Compiler.CommandLine
{
    public class Parser
    {
        private readonly SyntaxToken[] _syntaxTokens;
        private int _position;

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
        }

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;

            return index >= _syntaxTokens.Length ? _syntaxTokens[^1] : _syntaxTokens[index];
        }

        private SyntaxToken Current => Peek(0);
    }
}
