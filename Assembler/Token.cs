namespace SVM.Assembler
{
    public enum Token
    {
        Unknown,
        Ident,
        Register,
        Number,
        Operation,
        NewLine,
        Colon,
        LeftBr,
        RightBr,
        Plus,
        Minus,
        Eos,
        Breakpoint,
        Macro,
        EndMacro
    }

    public static class TokenNames
    {
        public static readonly Dictionary<Token, string> Names = new()
    {
        { Token.Unknown, "Unknown" },
        { Token.Ident, "Identifier" },
        { Token.Operation, "Operation" },
        { Token.Register, "Register" },
        { Token.Number, "Number" },
        { Token.NewLine, "\\n" },
        { Token.Colon, ":" },
        { Token.LeftBr, "[" },
        { Token.RightBr, "]" },
        { Token.Plus, "+" },
        { Token.Minus, "-" },
        { Token.Eos, "Eos" },
        { Token.Breakpoint, "#breakpoint" }
    };
    }
}
