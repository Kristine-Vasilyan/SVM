using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        LeftBracket,
        RightBracket,
        Plus,
        Minus,
        Eos
    }

    public class Lexeme
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
                //mapov 
                Token.Ident => $"IDENT<{Value}>",
                Token.Register => $"REG<{Value}>",
                Token.Number => $"NUM<{Value}>",
                Token.Operation => $"OP<{Value}>",
                _ => Kind.ToString()
            };
        }
    }

    public class Scanner
    {
        private readonly StringReader _source;
        private int _line = 1;

        private readonly Dictionary<char, Token> _metaSymbols = new()
        {
            { ':', Token.Colon },
            { '[', Token.LeftBracket },
            { ']', Token.RightBracket },
            { '+', Token.Plus },
            { '-', Token.Minus },
            { '\n', Token.NewLine },
        };

        public Scanner(string source)
        {
            _source = new StringReader(source);
        }

        public Lexeme ScanOne()
        {
            int chInt = _source.Read();
            if (chInt == -1)
                return new Lexeme(Token.Eos, "EOS");

            char ch = (char)chInt;

            while (char.IsWhiteSpace(ch) && ch != '\n')
            {
                chInt = _source.Read();
                if (chInt == -1) return new Lexeme(Token.Eos, "EOS");
                ch = (char)chInt;
            }

            if (ch == ';')
            {
                while (ch != '\n' && chInt != -1)
                {
                    chInt = _source.Read();
                    if (chInt == -1) return new Lexeme(Token.Eos, "EOS");
                    ch = (char)chInt;
                }
            }

            if (_metaSymbols.ContainsKey(ch))
            {
                if (_metaSymbols[ch] == Token.NewLine)
                    _line++;
                return new Lexeme(_metaSymbols[ch], ch.ToString());
            }

            if (char.IsLetter(ch))
            {
                var sb = new StringBuilder();
                sb.Append(ch);
                while (true)
                {
                    int next = _source.Peek();
                    if (next == -1) break;
                    char nc = (char)next;
                    if (!char.IsLetterOrDigit(nc)) break;
                    sb.Append((char)_source.Read());
                }
                string text = sb.ToString();
                // TODO: differentiate between operation and identifier
                return new Lexeme(Token.Ident, text);
            }

            if (char.IsDigit(ch))
            {
                var sb = new StringBuilder();
                sb.Append(ch);
                while (true)
                {
                    int next = _source.Peek();
                    if (next == -1) break;
                    char nc = (char)next;
                    if (!char.IsDigit(nc)) break;
                    sb.Append((char)_source.Read());
                }
                return new Lexeme(Token.Number, sb.ToString());
            }

            return new Lexeme(Token.Unknown, ch.ToString());
        }
    }
}
