using System;

namespace AS_Compiler.CommandLine
{
    internal class Program
    {
        private static void Main()
        {
            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                var lexer = new Lexer(line);
                while (true)
                {
                    var syntaxToken = lexer.NextSyntaxToken();

                    if (syntaxToken.Type == SyntaxType.EndOfFile)
                    {
                        break;
                    }

                    Console.Write($"{syntaxToken.Type}: '{syntaxToken.Text}'");
                    if(syntaxToken.Value != null)
                    {
                        Console.Write($" {syntaxToken.Value}");
                    }

                    Console.WriteLine();
                }
            }
        }
    }
}
