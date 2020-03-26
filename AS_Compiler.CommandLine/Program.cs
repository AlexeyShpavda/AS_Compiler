﻿using System;
using System.Linq;
using AS_Compiler.CommandLine.CodeAnalysis;
using AS_Compiler.CommandLine.CodeAnalysis.Binding;
using AS_Compiler.CommandLine.CodeAnalysis.Syntax;

namespace AS_Compiler.CommandLine
{
    internal class Program
    {
        private static void Main()
        {
            var showTree = false;

            while (true)
            {
                Console.Write("> ");
                var line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line))
                {
                    return;
                }

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees.");
                    continue;
                }
                else if (line == "#cls")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var binder = new Binder();
                var boundExpression = binder.BindExpression(syntaxTree.Root);

                var diagnostics = syntaxTree.Diagnostics.Concat(binder.Diagnostics).ToList();

                if (showTree)
                {
                    Print(syntaxTree.Root);
                }

                if (!diagnostics.Any())
                {
                    var e = new Evaluator(boundExpression);
                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;

                    foreach (var diagnostic in syntaxTree.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }

                    Console.ResetColor();
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
