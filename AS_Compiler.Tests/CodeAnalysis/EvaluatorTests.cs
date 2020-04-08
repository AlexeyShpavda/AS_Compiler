using System;
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
        [InlineData("{ var a = 0 (a = 10) * a }", 100)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            AssertValue(text, expectedValue);
        }

        private static void AssertValue(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            const string text = @"
                {
                    var x = 10
                    var y = 100
                    {
                        var x = 25
                    }
                    var [x] = 5
                }
            ";

            const string diagnostics = @"
                Variable 'x' was already declared.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Name_Reports_Undefined()
        {
            const string text = @"[x] + 1";

            const string diagnostics = @"
                Variable 'x' does not exist.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Assigned_Reports_Undefined()
        {
            const string text = @"[x] = 1";

            const string diagnostics = @"
                Variable 'x' does not exist.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Assigned_Reports_CannotAssign()
        {
            const string text = @"
                {
                    let x = 1
                    x [=] 0
                }
            ";

            const string diagnostics = @"
                Variable 'x' is read-only and cannot be assigned.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Assigned_Reports_CannotConvert()
        {
            const string text = @"
                {
                    var x = 1
                    x = [true]
                }
            ";

            const string diagnostics = @"
                Cannot convert type 'System.Boolean' to 'System.Int32'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Unary_Reports_Undefined()
        {
            const string text = @"[+]false";

            const string diagnostics = @"
                Unary operator '+' is not defined for type 'System.Boolean'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Binary_Reports_Undefined()
        {
            const string text = @"1 [+] true";

            const string diagnostics = @"
                Binary operator '+' is not defined for type 'System.Int32' and 'System.Boolean'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        private static void AssertHasDiagnostics(string text, string diagnosticText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnIndentLines(diagnosticText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
            {
                throw new Exception("ERROR: Must mark as many spans as there are expected diagnostics");
            }

            Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;
                Assert.Equal(expectedMessage, actualMessage);

                var expectedTextSpan = annotatedText.Spans[i];
                var actualTextSpan = result.Diagnostics[i].TextSpan;
                Assert.Equal(expectedTextSpan, actualTextSpan);
            }
        }
    }
}