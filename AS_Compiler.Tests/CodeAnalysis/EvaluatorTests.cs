using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using Xunit;

namespace AS_Compiler.Tests.CodeAnalysis
{
    public class EvaluatorTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("1 + 3", 4)]
        [InlineData("3 - 1", 2)]
        [InlineData("2 * 2", 4)]
        [InlineData("9 / 3", 3)]
        [InlineData("(10)", 10)]
        [InlineData("4 == 3", false)]
        [InlineData("1 == 1", true)]
        [InlineData("10 != 1", true)]
        [InlineData("1 != 1", false)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]
        [InlineData("false != false", false)]
        [InlineData("true != false", true)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("(a = 5) * a", 25)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}