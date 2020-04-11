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
        [InlineData("~1", -2)]
        [InlineData("1 + 3", 4)]
        [InlineData("3 - 1", 2)]
        [InlineData("2 * 2", 4)]
        [InlineData("9 / 3", 3)]
        [InlineData("(10)", 10)]
        [InlineData("4 == 3", false)]
        [InlineData("1 == 1", true)]
        [InlineData("10 != 1", true)]
        [InlineData("1 != 1", false)]
        [InlineData("1 < 2", true)]
        [InlineData("1 < 1", false)]
        [InlineData("1 <= 1", true)]
        [InlineData("1 <= 2", true)]
        [InlineData("2 <= 1", false)]
        [InlineData("1 > 2", false)]
        [InlineData("2 > 1", true)]
        [InlineData("1 >= 1", true)]
        [InlineData("2 >= 1", true)]
        [InlineData("1 >= 2", false)]
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 3", 1)]
        [InlineData("1 & 0", 0)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("0 ^ 1", 1)]
        [InlineData("1 ^ 3", 2)]
        [InlineData("true && true", true)]
        [InlineData("true && false", false)]
        [InlineData("false && true", false)]
        [InlineData("false && false", false)]
        [InlineData("true || true", true)]
        [InlineData("true || false", true)]
        [InlineData("false || true", true)]
        [InlineData("false || false", false)]
        [InlineData("false == false", true)]
        [InlineData("true == false", false)]
        [InlineData("false != false", false)]
        [InlineData("true != false", true)]
        [InlineData("true & true", true)]
        [InlineData("true & false", false)]
        [InlineData("false & true", false)]
        [InlineData("false & false", false)]
        [InlineData("true | true", true)]
        [InlineData("true | false", true)]
        [InlineData("false | true", true)]
        [InlineData("false | false", false)]
        [InlineData("true ^ true", false)]
        [InlineData("true ^ false", true)]
        [InlineData("false ^ true", true)]
        [InlineData("false ^ false", false)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("{ var a = 1 }", 1)]
        [InlineData("{ var a = 1 (a + 9) }", 10)]
        [InlineData("{ var a = 0 (a = 10) * a }", 100)]
        [InlineData("{ var a = 0 if a == 0 a = 1 a }", 1)]
        [InlineData("{ var a = 0 if a == 1 a = 1 a }", 0)]
        [InlineData("{ var a = 0 if a == 0 a = 1 else a = 2 a }", 1)]
        [InlineData("{ var a = 0 if a == 1 a = 1 else a = 2 a }", 2)]
        [InlineData("{ var i = 1 var sum = 0 while i <= 5 { sum = sum + i i = i + 1 } sum }", 15)]
        [InlineData("{ var sum = 0 for i = 1 to 5 { sum = sum + i } sum }", 15)]
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
        public void Evaluator_BlockStatement_NoInfiniteLoop()
        {
            const string text = @"
                {
                [)][]
            ";

            const string diagnostics = @"
                Unexpected token <ClosingParenthesisToken>, expected <IdentifierToken>.
                Unexpected token <EndOfFileToken>, expected <ClosingBraceToken>.
            ";

            AssertHasDiagnostics(text, diagnostics);
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
        public void Evaluator_NameExpression_Reports_Undefined()
        {
            const string text = @"[x] + 1";

            const string diagnostics = @"
                Variable 'x' does not exist.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_NameExpression_Reports_NoErrorForInsertedToken()
        {
            const string text = @"[]";

            const string diagnostics = @"
                Unexpected token <EndOfFileToken>, expected <IdentifierToken>.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_Undefined()
        {
            const string text = @"[x] = 1";

            const string diagnostics = @"
                Variable 'x' does not exist.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_AssignmentExpression_Reports_CannotAssign()
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
        public void Evaluator_AssignmentExpression_Reports_CannotConvert()
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
        public void Evaluator_IfStatement_Reports_CannotConvert()
        {
            const string text = @"
                {
                    var x = 1
                    if [10]
                        x = 10
                }
            ";

            const string diagnostics = @"
                Cannot convert type 'System.Int32' to 'System.Boolean'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_WhileStatement_Reports_CannotConvert()
        {
            const string text = @"
                {
                    var x = 1
                    while [10]
                        x = 10
                }
            ";

            const string diagnostics = @"
                Cannot convert type 'System.Int32' to 'System.Boolean'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
        {
            const string text = @"
                {
                    var sum = 0
                    for i = [false] to 10
                        sum = sum + i
                }
            ";

            const string diagnostics = @"
                Cannot convert type 'System.Boolean' to 'System.Int32'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
        {
            const string text = @"
                {
                    var sum = 0
                    for i = 1 to [false]
                        sum = sum + i
                }
            ";

            const string diagnostics = @"
                Cannot convert type 'System.Boolean' to 'System.Int32'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_UnaryExpression_Reports_Undefined()
        {
            const string text = @"[+]false";

            const string diagnostics = @"
                Unary operator '+' is not defined for type 'System.Boolean'.
            ";

            AssertHasDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_BinaryExpression_Reports_Undefined()
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