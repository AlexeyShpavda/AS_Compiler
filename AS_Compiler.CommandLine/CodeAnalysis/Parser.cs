﻿using System.Collections.Generic;

namespace AS_Compiler.CommandLine.CodeAnalysis
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

        private SyntaxToken MatchToken(SyntaxType syntaxType)
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

            var endOfFileToken = MatchToken(SyntaxType.EndOfFile);

            return new SyntaxTree(Diagnostics, expression, endOfFileToken);
        }

        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            var left = ParsePrimaryExpression();

            while (true)
            {
                var precedence = GetBinaryOperatorPrecedence(Current.Type);
                if (precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }

                var operatorToken = NextToken();
                var right = ParseExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private static int GetBinaryOperatorPrecedence(SyntaxType syntaxType)
        {
            switch (syntaxType)
            {
                case SyntaxType.Star:
                case SyntaxType.Slash:
                    return 2;
                case SyntaxType.Plus:
                case SyntaxType.Minus:
                    return 1;
                default:
                    return 0;
            }
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Type == SyntaxType.OpeningParenthesis)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxType.ClosingParenthesis);

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            var numberToken = MatchToken(SyntaxType.Number);

            return new LiteralExpressionSyntax(numberToken);
        }
    }
}