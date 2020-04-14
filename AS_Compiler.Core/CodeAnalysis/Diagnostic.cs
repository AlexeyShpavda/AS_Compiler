﻿using System.Collections;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Symbols;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using AS_Compiler.Core.CodeAnalysis.Text;

namespace AS_Compiler.Core.CodeAnalysis
{
    public sealed class Diagnostic
    {
        public Diagnostic(TextSpan textSpan, string message)
        {
            TextSpan = textSpan;
            Message = message;
        }

        public TextSpan TextSpan { get; }
        public string Message { get; }
        public override string ToString() => Message;
    }

    public sealed class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new List<Diagnostic>();

        public void AddRange(DiagnosticBag diagnostics)
        {
            _diagnostics.AddRange(diagnostics._diagnostics);
        }

        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Report(TextSpan textSpan, string message)
        {
            var diagnostic = new Diagnostic(textSpan, message);
            _diagnostics.Add(diagnostic);
        }

        public void ReportInvalidNumber(TextSpan textSpan, string text, TypeSymbol type)
        {
            var message = $"The number {text} is not valid {type}.";
            Report(textSpan, message);
        }

        public void ReportBadCharacter(int position, char character)
        {
            var textSpan = new TextSpan(position, 1);
            var message = $"ERROR: bad character input '{character}'.";
            Report(textSpan, message);
        }


        public void ReportUnexpectedToken(TextSpan textSpan, SyntaxType actualType, SyntaxType expectedType)
        {
            var message = $"Unexpected token <{actualType}>, expected <{expectedType}>.";
            Report(textSpan, message);
        }

        public void ReportUndefinedUnaryOperator(TextSpan textSpan, string operatorText, TypeSymbol operandType)
        {
            var message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'.";
            Report(textSpan, message);
        }

        public void ReportUndefinedBinaryOperator(TextSpan textSpan, string operatorText, TypeSymbol leftType, TypeSymbol rightType)
        {
            var message = $"Binary operator '{operatorText}' is not defined for type '{leftType}' and '{rightType}'.";
            Report(textSpan, message);
        }

        public void ReportUndefinedName(TextSpan textSpan, string name)
        {
            var message = $"Variable '{name}' does not exist.";
            Report(textSpan, message);
        }

        public void ReportCannotConvert(TextSpan textSpan, TypeSymbol fromType, TypeSymbol toType)
        {
            var message = $"Cannot convert type '{fromType}' to '{toType}'.";
            Report(textSpan, message);
        }

        public void ReportVariableAlreadyDeclared(TextSpan textSpan, string name)
        {
            var message = $"Variable '{name}' was already declared.";
            Report(textSpan, message);
        }

        public void ReportCannotAssign(TextSpan textSpan, string name)
        {
            var message = $"Variable '{name}' is read-only and cannot be assigned.";
            Report(textSpan, message);
        }

        public void ReportUnterminatedString(TextSpan textSpan)
        {
            const string message = "Unterminated string literal.";
            Report(textSpan, message);
        }

        public void ReportUndefinedFunction(TextSpan textSpan, string name)
        {
            var message = $"Function '{name}' does not exist.";
            Report(textSpan, message);
        }

        public void ReportWrongArgumentCount(TextSpan textSpan, string name, int expectedCount, int actualCount)
        {
            var message = $"Function '{name}' requires {expectedCount} arguments, but were given {actualCount}.";
            Report(textSpan, message);
        }


        public void ReportWrongArgumentType(TextSpan textSpan, string name, TypeSymbol parameterType, TypeSymbol argumentType)
        {
            var message = $"Parameter '{name}' requires type '{parameterType}' but was given '{argumentType}'.";
            Report(textSpan, message);
        }

        public void ReportExpressionMustHaveValue(TextSpan textSpan)
        {
            const string message = "Expression must have a value.";
            Report(textSpan, message);
        }
    }
}