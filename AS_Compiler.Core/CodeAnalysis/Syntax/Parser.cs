using System.Collections.Generic;
using System.Collections.Immutable;
using System.Xml;
using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public class Parser
    {
        private readonly SourceText _text;
        private readonly ImmutableArray<SyntaxToken> _syntaxTokens;
        private int _position;

        public Parser(SourceText text)
        {
            var syntaxTokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken syntaxToken;

            do
            {
                syntaxToken = lexer.NextSyntaxToken();

                if (syntaxToken.Type != SyntaxType.WhiteSpaceToken && syntaxToken.Type != SyntaxType.UnknownToken)
                {
                    syntaxTokens.Add(syntaxToken);
                }
            } while (syntaxToken.Type != SyntaxType.EndOfFileToken);

            _text = text;
            _syntaxTokens = syntaxTokens.ToImmutableArray();
            Diagnostics.AddRange(lexer.Diagnostics);
        }

        public DiagnosticBag Diagnostics { get; } = new DiagnosticBag();

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;

            return index >= _syntaxTokens.Length ? _syntaxTokens[_syntaxTokens.Length - 1] : _syntaxTokens[index];
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

            Diagnostics.ReportUnexpectedToken(Current.TextSpan, Current.Type, syntaxType);
            return new SyntaxToken(syntaxType, Current.Position, null, null);
        }

        public CompilationUnitSyntax ParseCompilationUnit()
        {
            var statement = ParseStatement();
            var endOfFileToken = MatchToken(SyntaxType.EndOfFileToken);

            return new CompilationUnitSyntax(statement, endOfFileToken);
        }

        private StatementSyntax ParseStatement()
        {
            switch (Current.Type)
            {
                case SyntaxType.OpeningBraceToken:
                    return ParseBlockStatement();
                case SyntaxType.LetKeyword:
                case SyntaxType.VarKeyword:
                    return ParseVariableDeclaration();
                case SyntaxType.IfKeyword:
                    return ParseIfStatement();
                case SyntaxType.WhileKeyword:
                    return ParseWhileStatement();
                default:
                    return ParseExpressionStatement();
            }
        }

        private StatementSyntax ParseWhileStatement()
        {
            var keyword = MatchToken(SyntaxType.WhileKeyword);
            var condition = ParseExpression();
            var body = ParseStatement();

            return new WhileStatementSyntax(keyword, condition, body);
        }

        private StatementSyntax ParseIfStatement()
        {
            var keyword = MatchToken(SyntaxType.IfKeyword);
            var condition = ParseExpression();
            var statement = ParseStatement();
            var elseClause = ParseElseClause();

            return new IfStatementSyntax(keyword, condition, statement, elseClause);
        }

        private ElseClauseSyntax ParseElseClause()
        {
            if (Current.Type != SyntaxType.ElseKeyword)
            {
                return null;
            }

            var keyword = NextToken();
            var statement = ParseStatement();

            return new ElseClauseSyntax(keyword, statement);
        }

        private StatementSyntax ParseVariableDeclaration()
        {
            var expected = Current.Type == SyntaxType.LetKeyword ? SyntaxType.LetKeyword : SyntaxType.VarKeyword;
            var keyword = MatchToken(expected);
            var identifier = MatchToken(SyntaxType.IdentifierToken);
            var equalsToken = MatchToken(SyntaxType.EqualsToken);
            var initializer = ParseExpression();

            return new VariableDeclarationSyntax(keyword, identifier, equalsToken, initializer);
        }

        private StatementSyntax ParseBlockStatement()
        {
            var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

            var openingBraceToken = MatchToken(SyntaxType.OpeningBraceToken);

            while (Current.Type != SyntaxType.EndOfFileToken && Current.Type != SyntaxType.ClosingBraceToken)
            {
                var statement = ParseStatement();
                statements.Add(statement);
            }

            var closingBraceToken = MatchToken(SyntaxType.ClosingBraceToken);

            return new BlockStatementSyntax(openingBraceToken, statements.ToImmutable(), closingBraceToken);
        }

        private StatementSyntax ParseExpressionStatement()
        {
            var expression = ParseExpression();

            return new ExpressionStatementSyntax(expression);
        }

        public ExpressionSyntax ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        public ExpressionSyntax ParseAssignmentExpression()
        {
            if (Peek(0).Type == SyntaxType.IdentifierToken
                && Peek(1).Type == SyntaxType.EqualsToken)
            {
                var identifierToken = NextToken();
                var operatorToken = NextToken();
                var right = ParseAssignmentExpression();
                return new AssignmentExpressionSyntax(identifierToken, operatorToken, right);
            }

            return ParseBinaryExpression();
        }

        private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
        {
            ExpressionSyntax left;
            var unaryOperatorPrecedence = Current.Type.GetUnaryOperatorPrecedence();

            if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
            {
                var operatorToken = NextToken();
                var operand = ParseBinaryExpression(unaryOperatorPrecedence);
                left = new UnaryExpressionSyntax(operatorToken, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                var precedence = Current.Type.GetBinaryOperatorPrecedence();
                if (precedence == 0 || precedence <= parentPrecedence)
                {
                    break;
                }

                var operatorToken = NextToken();
                var right = ParseBinaryExpression(precedence);
                left = new BinaryExpressionSyntax(left, operatorToken, right);
            }

            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Type)
            {
                case SyntaxType.OpeningParenthesisToken:
                    return ParseParenthesizedExpression();
                case SyntaxType.TrueKeyword:
                case SyntaxType.FalseKeyword:
                    return ParseBooleanLiteral();
                case SyntaxType.NumberToken:
                    return ParseNumberLiteral();
                default:
                    return ParseNameExpression();
            }
        }

        private ExpressionSyntax ParseParenthesizedExpression()
        {
            var left = MatchToken(SyntaxType.OpeningParenthesisToken);
            var expression = ParseExpression();
            var right = MatchToken(SyntaxType.ClosingParenthesisToken);

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }

        private ExpressionSyntax ParseBooleanLiteral()
        {
            var isTrue = Current.Type == SyntaxType.TrueKeyword;
            var keywordToken = isTrue ? MatchToken(SyntaxType.TrueKeyword) : MatchToken(SyntaxType.FalseKeyword);

            return new LiteralExpressionSyntax(keywordToken, isTrue);
        }

        private ExpressionSyntax ParseNumberLiteral()
        {
            var numberToken = MatchToken(SyntaxType.NumberToken);

            return new LiteralExpressionSyntax(numberToken);
        }

        private ExpressionSyntax ParseNameExpression()
        {
            var identifierToken = MatchToken(SyntaxType.IdentifierToken);

            return new NameExpressionSyntax(identifierToken);
        }
    }
}