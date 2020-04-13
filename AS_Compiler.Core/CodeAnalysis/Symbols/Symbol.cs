namespace AS_Compiler.Core.CodeAnalysis.Symbols
{
    public abstract class Symbol
    {
        internal Symbol(string name)
        {
            Name = name;
        }

        public abstract SymbolType SymbolType { get; }
        public string Name { get; }

        public override string ToString() => Name;
    }
}