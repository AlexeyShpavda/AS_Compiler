using System;
using System.Linq;

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

                var parser = new Parser(line);
                var syntaxTree = parser.Parse();

                Print(syntaxTree.Root);

                if (syntaxTree.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var diagnostic in syntaxTree.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }

                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }
        }

        private static void Print(SyntaxNode syntaxNode, string indent = "", bool isLast = true)
        {

            var marker = isLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(syntaxNode.Type);

            if(syntaxNode is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += isLast ? "    " : "│   ";

            var lastChild = syntaxNode.GetChildren().LastOrDefault();

            foreach (var child in syntaxNode.GetChildren())
            {
                Print(child, indent, child == lastChild);
            }
        }
    }
}
