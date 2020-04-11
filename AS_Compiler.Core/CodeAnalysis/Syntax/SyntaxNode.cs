using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.Core.CodeAnalysis.Syntax
{
    public abstract class SyntaxNode
    {
        public abstract SyntaxType Type { get; }

        public virtual TextSpan TextSpan
        {
            get
            {
                var first = GetChildren().First().TextSpan;
                var last = GetChildren().Last().TextSpan;

                return TextSpan.FromBounds(first.Start, last.End);
            }
        }

        public IEnumerable<SyntaxNode> GetChildren()
        {
            // The order of properties corresponds to properties declaration order in Type
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(SyntaxNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (SyntaxNode)property.GetValue(this);

                    if (child != null)
                    {
                        yield return child;
                    }
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>)property.GetValue(this);

                    foreach (var child in children)
                    {
                        if (child != null)
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        public void WriteTo(TextWriter writer)
        {
            Print(writer, this);
        }

        private static void Print(TextWriter writer, SyntaxNode syntaxNode, string indent = "", bool isLast = true)
        {
            var isConsoleOut = writer == Console.Out;
            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);

            if (isConsoleOut)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            writer.Write(marker);

            if (isConsoleOut)
            {
                Console.ForegroundColor = syntaxNode is SyntaxToken ? ConsoleColor.DarkCyan : ConsoleColor.Cyan;
            }

            writer.Write(syntaxNode.Type);

            if (syntaxNode is SyntaxToken t && t.Value != null)
            {
                writer.Write(" ");
                writer.Write(t.Value);
            }

            if (isConsoleOut)
            {
                Console.ResetColor();
            }

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = syntaxNode.GetChildren().LastOrDefault();

            foreach (var child in syntaxNode.GetChildren())
            {
                Print(writer, child, indent, child == lastChild);
            }
        }

        public override string ToString()
        {
            using var writer = new StringWriter();
            WriteTo(writer);

            return writer.ToString();
        }
    }
}