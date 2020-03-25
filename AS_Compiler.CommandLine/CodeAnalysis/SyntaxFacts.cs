namespace AS_Compiler.CommandLine.CodeAnalysis
{
    public static class SyntaxFacts
    {
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