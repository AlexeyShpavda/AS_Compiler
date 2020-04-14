using System.Collections.Immutable;

namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public sealed class FunctionSymbol : Symbol
    {
        public FunctionSymbol(string name, ImmutableArray<ParameterSymbol> parameters, TypeSymbol type)
            : base(name)
        {
            Parameters = parameters;
            Type = type;
        }

        public override SymbolType SymbolType => SymbolType.Function;
        public ImmutableArray<ParameterSymbol> Parameters { get; }
        public TypeSymbol Type { get; }
    }
}