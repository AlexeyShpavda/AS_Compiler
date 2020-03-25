namespace AS_Compiler.CommandLine.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxType syntaxType)
        {
            switch (syntaxType)
            {
                case SyntaxType.Plus:
                case SyntaxType.Minus:
                    return 3;
                default:
                    return 0;
            }
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxType syntaxType)
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
    }
}