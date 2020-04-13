namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public sealed class TypeSymbol : Symbol
    {
        public static readonly TypeSymbol String = new TypeSymbol("string");
        public static readonly TypeSymbol Int = new TypeSymbol("int");
        public static readonly TypeSymbol Bool = new TypeSymbol("bool");

        private TypeSymbol(string name)
            : base(name)
        {

        }

        public override SymbolType SymbolType => SymbolType.Type;
    }
}