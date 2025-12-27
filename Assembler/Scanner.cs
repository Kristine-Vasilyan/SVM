using SVM.OperetionCodes;
using System.Text;

namespace SVM.Assembler
{
    public sealed class Scanner
    {
        private readonly TextReader source;
        private int line = 1;
        private int peek = -1;

        public Scanner(TextReader reader)
        {
            source = reader;
        }

        private static readonly Dictionary<char, Token> MetaSymbols = new()
    {
        { ':', Token.Colon },
        { '[', Token.LeftBr },
        { ']', Token.RightBr },
        { '+', Token.Plus },
        { '-', Token.Minus },
        { '\n', Token.NewLine }
    };

        public Lexeme ScanOne()
        {
            char ch = ReadChar();

            if (IsSpace(ch))
            {
                ReadCharsWhile(IsSpace);
                ch = ReadChar();
            }

            if (ch == ';')
            {
                ReadCharsWhile(c => c != '\n');
                ch = ReadChar();
            }

            if (ch == '\0')
                return new Lexeme(Token.Eos, "EOS");

            if (char.IsLetter(ch))
            {
                UnreadChar(ch);
                string text = ReadCharsWhile(IsAlphaNumeric);

                if (IsOperation(text))
                    return new Lexeme(Token.Operation, text);

                if (IsRegister(text))
                    return new Lexeme(Token.Register, text);

                return new Lexeme(Token.Ident, text);
            }

            if (char.IsDigit(ch))
            {
                UnreadChar(ch);
                string text = ReadCharsWhile(char.IsDigit);
                return new Lexeme(Token.Number, text);
            }

            if (MetaSymbols.TryGetValue(ch, out var tok))
            {
                if (tok == Token.NewLine)
                    line++;

                return new Lexeme(tok, ch.ToString());
            }

            return new Lexeme(Token.Unknown, ch.ToString());
        }

        private static bool IsAlphaNumeric(char c) => char.IsLetterOrDigit(c);

        private static bool IsSpace(char c) => c == ' ' || c == '\t' || c == '\r';

        private string ReadCharsWhile(Func<char, bool> pred)
        {
            var sb = new StringBuilder();
            char ch = ReadChar();

            while (ch != '\0' && pred(ch))
            {
                sb.Append(ch);
                ch = ReadChar();
            }

            UnreadChar(ch);
            return sb.ToString();
        }

        private char ReadChar()
        {
            int ch;
            if (peek != -1)
            {
                ch = peek;
                peek = -1;
            }
            else
            {
                ch = source.Read();
            }

            return ch == -1 ? '\0' : (char)ch;
        }

        private void UnreadChar(char ch)
        {
            peek = ch;
        }

        private static bool IsOperation(string text) => OperationCodes.Mnemonics.ContainsValue(text);

        private static bool IsRegister(string text) => text == "IP" || text == "SP" || text == "FP";
    }
}
