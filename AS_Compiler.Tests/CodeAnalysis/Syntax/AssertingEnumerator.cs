using System;
using System.Collections.Generic;
using System.Linq;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using Xunit;

namespace AS_Compiler.Tests.CodeAnalysis.Syntax
{
    internal sealed class AssertingEnumerator : IDisposable
    {
        private readonly IEnumerator<SyntaxNode> _enumerator;
        private bool _hasErrors;

        public AssertingEnumerator(SyntaxNode syntaxNode)
        {
            _enumerator = Flatten(syntaxNode).GetEnumerator();
        }

        private bool MarkFailed()
        {
            _hasErrors = true;
            return false;
        }

        public void Dispose()
        {
            if (_hasErrors)
            {
                Assert.False(_enumerator.MoveNext());
            }

            _enumerator.Dispose();
        }

        private static IEnumerable<SyntaxNode> Flatten(SyntaxNode syntaxNode)
        {
            var stack = new Stack<SyntaxNode>();
            stack.Push(syntaxNode);

            while (stack.Count > 0)
            {
                var n = stack.Pop();

                yield return n;

                foreach (var child in n.GetChildren().Reverse())
                {
                    stack.Push(child);
                }
            }
        }

        public void AssertNode(SyntaxType syntaxType)
        {
            try
            {
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(syntaxType, _enumerator.Current.Type);
                Assert.IsNotType<SyntaxToken>(_enumerator.Current);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }

        public void AssertToken(SyntaxType syntaxType, string text)
        {
            try
            {
                Assert.True(_enumerator.MoveNext());
                Assert.Equal(syntaxType, _enumerator.Current.Type);
                var token = Assert.IsType<SyntaxToken>(_enumerator.Current);
                Assert.Equal(text, token.Text);
            }
            catch when (MarkFailed())
            {
                throw;
            }
        }
    }
}