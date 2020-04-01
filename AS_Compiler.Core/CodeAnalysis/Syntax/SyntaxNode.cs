using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                    var child = (SyntaxNode) property.GetValue(this);
                    yield return child;
                }
                else if (typeof(IEnumerable<SyntaxNode>).IsAssignableFrom(property.PropertyType))
                {
                    var children = (IEnumerable<SyntaxNode>) property.GetValue(this);

                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }
        }
    }
}