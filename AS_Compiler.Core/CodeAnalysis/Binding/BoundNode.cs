using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Console = System.Console;

namespace AS_Compiler.Core.CodeAnalysis.Binding
{
    internal abstract class BoundNode
    {
        public abstract BoundNodeType BoundNodeType { get; }

        public IEnumerable<BoundNode> GetChildren()
        {
            // The order of properties corresponds to properties declaration order in Type
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
                {
                    var child = (BoundNode)property.GetValue(this);

                    if (child != null)
                    {
                        yield return child;
                    }
                }
                else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<BoundNode>)property.GetValue(this);

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

        private static void Print(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
        {
            var isConsoleOut = writer == Console.Out;
            var marker = isLast ? "└──" : "├──";

            writer.Write(indent);
            writer.Write(marker);

            if (isConsoleOut)
            {
                Console.ForegroundColor = GetColor(node);
            }

            var text = GetText(node);
            writer.Write(text);

            var isFirstProperty = true;

            foreach (var (name, value) in node.GetProperties())
            {
                if (isFirstProperty)
                {
                    isFirstProperty = false;
                }
                else
                {
                    if (isConsoleOut)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }

                    writer.Write(",");
                }

                writer.Write(" ");

                if (isConsoleOut)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                writer.Write(name);

                if (isConsoleOut)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }

                writer.Write(" = ");

                if (isConsoleOut)
                {
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                }

                writer.Write(value);
            }

            if (isConsoleOut)
            {
                Console.ResetColor();
            }

            writer.WriteLine();

            indent += isLast ? "   " : "│  ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                Print(writer, child, indent, child == lastChild);
            }
        }

        private IEnumerable<(string Name, object Value)> GetProperties()
        {
            // The order of properties corresponds to properties declaration order in Type
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var property in properties)
            {
                if (property.Name == nameof(BoundNodeType)
                    || property.Name == nameof(BoundBinaryExpression.Operator)
                    || typeof(BoundNode).IsAssignableFrom(property.PropertyType)
                    || typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }

                var value = property.GetValue(this);
                if (value != null)
                {
                    yield return (property.Name, value);
                }
            }
        }

        private static string GetText(BoundNode node)
        {
            switch (node)
            {
                case BoundBinaryExpression b:
                    return b.Operator.Type + "Expression";
                case BoundUnaryExpression u:
                    return u.Operator.Type + "Expression";
                default:
                    return node.BoundNodeType.ToString();
            }
        }

        private static ConsoleColor GetColor(BoundNode node)
        {
            switch (node)
            {
                case BoundExpression _:
                    return ConsoleColor.DarkCyan;
                case BoundStatement _:
                    return ConsoleColor.Cyan;
                default:
                    return ConsoleColor.DarkRed;
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