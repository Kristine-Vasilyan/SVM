namespace SVM.Assembler
{
    public sealed class Lexeme
    {
        public Token Kind { get; }
        public string Value { get; }

        public Lexeme(Token kind, string value)
        {
            Kind = kind;
            Value = value;
        }

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
