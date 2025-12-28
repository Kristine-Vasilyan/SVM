namespace SVM.Assembler
{
    public sealed class Lexeme(Token kind, string value)
    {
        public Token Kind { get; } = kind;
        public string Value { get; } = value;

        public override string ToString()
        {
            return Kind switch
            {
                Token.Ident => $"IDENT<{Value}>",
                Token.Operation => $"OP<{Value}>",
                Token.Register => $"REG<{Value}>",
                Token.Number => $"NUM<{Value}>",
                _ => TokenNames.Names[Kind]
            };
        }
    }
}
