﻿using System;
using System.Collections.Generic;
using System.Linq;
using AS_Compiler.Core.CodeAnalysis;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.CommandLine
{
    internal class CompilerReadEvalPrintLoop : ReadEvalPrintLoop
    {
        private bool _showTree;
        private bool _showProgram;
        private Compilation _previous;
        private readonly Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();

        protected override void RenderLine(string line)
        {
            var tokens = SyntaxTree.ParseTokens(line);

            foreach (var token in tokens)
            {
                var isKeyword = token.Type.ToString().EndsWith("Keyword");
                var isNumber = token.Type == SyntaxType.NumberToken;

                if (isKeyword)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (!isNumber)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                Console.Write(token.Text);
                Console.ResetColor();
            }
        }

        protected override void EvaluateMetaCommand(string input)
        {
            switch (input)
            {
                case "#showTree":
                    _showTree = !_showTree;
                    Console.WriteLine(_showTree ? "Showing parse trees." : "Not showing parse trees.");
                    break;
                case "#showProgram":
                    _showProgram = !_showProgram;
                    Console.WriteLine(_showProgram ? "Showing bound tree." : "Not showing bound tree.");
                    break;
                case "#cls":
                    Console.Clear();
                    break;
                case "#reset":
                    _previous = null;
                    _variables.Clear();
                    break;
                default:
                    base.EvaluateMetaCommand(input);
                    break;
            }
        }

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            var syntaxTree = SyntaxTree.Parse(text);

            return !syntaxTree.Root.Statement.GetLastToken().IsMissing;
        }

        protected override void EvaluateSubmission(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = _previous == null
                ? new Compilation(syntaxTree)
                : _previous.ContinueWith(syntaxTree);

            if (_showTree)
            {
                syntaxTree.Root.WriteTo(Console.Out);
            }

            if (_showProgram)
            {
                compilation.EmitTree(Console.Out);
            }

            var result = compilation.Evaluate(_variables);

            var diagnostics = result.Diagnostics;

            if (!diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(result.Value);
                Console.ResetColor();
                _previous = compilation;
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

                Console.WriteLine();
            }
        }
    }
}