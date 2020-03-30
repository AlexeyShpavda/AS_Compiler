using System;
using System.Collections.Generic;
using AS_Compiler.Core.CodeAnalysis.Syntax;
using Xunit;

namespace AS_Compiler.Tests.CodeAnalysis.Syntax
{
    public class SyntaxFactTests
    {
        [Theory]
        [MemberData(nameof(GetSyntaxTypeData))]
        public void SyntaxFact_GetText_RoundTrips(SyntaxType syntaxType)
        {
            var text = SyntaxFacts.GetText(syntaxType);

            if (text == null)
            {
                return;
            }

            var tokens = SyntaxTree.ParseTokens(text);
            var token = Assert.Single(tokens);

            if (token == null)
            {
                return;
            }

            Assert.Equal(syntaxType, token.Type);
            Assert.Equal(text, token.Text);
        }

        public static IEnumerable<object[]> GetSyntaxTypeData()
        {
            var types = (SyntaxType[]) Enum.GetValues(typeof(SyntaxType));

            foreach (var type in types)
            {
                yield return new object[] {type};
            }
        }
    }
}