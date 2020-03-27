namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public static class SyntaxFacts
    {
        public static int GetUnaryOperatorPrecedence(this SyntaxType syntaxType)
        {
            return syntaxType switch
            {
                SyntaxType.PlusToken => 6,
                SyntaxType.MinusToken => 6,
                SyntaxType.BangToken => 6,

                _ => 0
            };
        }

        public static int GetBinaryOperatorPrecedence(this SyntaxType syntaxType)
        {
            return syntaxType switch
            {
                SyntaxType.StarToken => 5,
                SyntaxType.SlashToken => 5,

                SyntaxType.PlusToken => 4,
                SyntaxType.MinusToken => 4,

                SyntaxType.EqualsEqualsToken => 3,
                SyntaxType.BangEqualsToken => 3,

                SyntaxType.AmpersandAmpersandToken => 2,

                SyntaxType.PipePipeToken => 1,

                _ => 0
            };
        }

        internal static SyntaxType GetKeywordType(string text)
        {
            return text switch
            {
                "true" => SyntaxType.TrueKeyword,
                "false" => SyntaxType.FalseKeyword,
                _ => SyntaxType.IdentifierToken
            };
        }
    }
}