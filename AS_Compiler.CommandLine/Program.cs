using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AS_Compiler.Core.CodeAnalysis;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.CommandLine
{
    internal class Program
    {
        private static void Main()
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol, object>();
            var textBuilder = new StringBuilder();

            while (true)
            {
                Console.Write(textBuilder.Length == 0 ? "> " : "| ");

                var input = Console.ReadLine();
                var isLineBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isLineBlank)
                        break;

                    switch (input)
                    {
                        case "#showTree":
                            showTree = !showTree;
                            Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                            continue;
                        case "#cls":
                            Console.Clear();
                            continue;
                    }
                }

                textBuilder.AppendLine(input);

                var text = textBuilder.ToString();
                var syntaxTree = SyntaxTree.Parse(text);

                if (!isLineBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);

                var diagnostics = result.Diagnostics;

                if (showTree)
                {
                    syntaxTree.Root.WriteTo(Console.Out);
                }

                if (!diagnostics.Any())
                {
                    Console.WriteLine(result.Value);
                }
                else
                {
                    foreach (var diagnostic in diagnostics)
                    {
                        var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.TextSpan.Start);
                        var line = syntaxTree.Text.Lines[lineIndex];
                        var lineNumber = lineIndex + 1;
                        var character = diagnostic.TextSpan.Start - line.Start + 1;

                        Console.WriteLine();

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write($"({lineNumber}, {character}): ");
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.TextSpan.Start);
                        var suffixSpan = TextSpan.FromBounds(diagnostic.TextSpan.End, line.End);

                        var prefix = syntaxTree.Text.ToString(prefixSpan);
                        var error = syntaxTree.Text.ToString(diagnostic.TextSpan);
                        var suffix = syntaxTree.Text.ToString(suffixSpan);

                        Console.Write("    ");
                        Console.Write(prefix);

                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Write(error);
                        Console.ResetColor();

                        Console.Write(suffix);

                        Console.WriteLine();
                    }

                    textBuilder.Clear();
                }
            }
        }
    }
}
