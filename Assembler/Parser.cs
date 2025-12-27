using SVM.OperetionCodes;

namespace SVM.Assembler
{


    public sealed class Parser
    {
        private readonly Scanner scanner;
        private Lexeme lookahead;

        private readonly Builder builder;

        private static readonly Dictionary<string, OperationCode> Operations =
            OperationCodes.Mnemonics.ToDictionary(p => p.Value, p => p.Key);

        private static readonly Dictionary<string, ushort> Registers = new()
    {
        { "IP", OperationCodes.Registers.InstructionPointer },
        { "SP", OperationCodes.Registers.StackPointer },
        { "FP", OperationCodes.Registers.FramePointer }
    };

        public Parser(Scanner scanner, Builder builder)
        {
            this.scanner = scanner;
            this.builder = builder;
        }

        public void Parse()
        {
            lookahead = scanner.ScanOne();
            ParseNewLines();

            while (!Has(Token.Eos))
            {
                ParseLine();
            }
        }

        private void ParseNewLines()
        {
            while (Has(Token.NewLine))
                lookahead = scanner.ScanOne();
        }

        private void ParseLine()
        {
            if (Has(Token.Ident))
                ParseLabel();

            if (Has(Token.Operation))
                ParseOperation();

            if (Has(Token.NewLine))
            {
                ParseNewLines();
                return;
            }

            throw Report($"Line starts with invalid symbol {lookahead}");
        }

        private void ParseOperation()
        {
            if (!Has(Token.Operation))
                throw Report($"Expected operation, got {lookahead}");

            switch (lookahead.Value)
            {
                case "PUSH":
                    ParsePush();
                    break;

                case "POP":
                    ParsePop();
                    break;

                case "CALL":
                case "JUMP":
                case "JZ":
                    ParseJump();
                    break;

                default:
                    ParseSimple();
                    break;
            }
        }

        private void ParsePush()
        {
            var name = Match(Token.Operation);
            if (name != "PUSH")
                throw Report($"Expected PUSH, got {name}");

            if (Has(Token.Number, Token.Plus, Token.Minus))
            {
                int number = ParseNumber();
                builder.AddWithNumeric(OperationCode.Push, number);
            }
            else if (Has(Token.LeftBr))
            {
                var (reg, disp) = ParseIndirect();
                builder.AddWithAddress(OperationCode.Push, reg, disp);
            }
            else
            {
                throw Report("Invalid PUSH argument");
            }
        }

        private void ParsePop()
        {
            var name = Match(Token.Operation);
            if (name != "POP")
                throw Report($"Expected POP, got {name}");

            if (!Has(Token.LeftBr))
                throw Report("POP expects indirect addressing");

            var (reg, disp) = ParseIndirect();
            builder.AddWithAddress(OperationCode.Pop, reg, disp);
        }

        private void ParseJump()
        {
            var name = Match(Token.Operation);

            if (name != "CALL" && name != "JUMP" && name != "JZ")
                throw Report($"Expected CALL/JUMP/JZ, got {name}");

            var label = Match(Token.Ident);
            builder.AddWithLabel(Operations[name], label);
        }

        private void ParseSimple()
        {
            var name = Match(Token.Operation);
            builder.AddBasic(Operations[name]);
        }

        private int ParseNumber()
        {
            int sign = 1;

            if (Has(Token.Plus))
                Match(Token.Plus);
            else if (Has(Token.Minus))
            {
                Match(Token.Minus);
                sign = -1;
            }

            string num = Match(Token.Number);
            return sign * int.Parse(num, CultureInfo.InvariantCulture);
        }

        private (ushort reg, short disp) ParseIndirect()
        {
            Match(Token.LeftBr);

            string regName = Match(Token.Register);
            ushort reg = Registers[regName];

            short sign = 1;
            if (Has(Token.Plus))
                Match(Token.Plus);
            else if (Has(Token.Minus))
            {
                Match(Token.Minus);
                sign = -1;
            }
            else
                throw Report("Expected '+' or '-'");

            string num = Match(Token.Number);
            short disp = (short)(sign * short.Parse(num, CultureInfo.InvariantCulture));

            Match(Token.RightBr);
            return (reg, disp);
        }

        private void ParseLabel()
        {
            string name = Match(Token.Ident);
            Match(Token.Colon);
            builder.SetLabel(name);
        }

        private string Match(Token expected)
        {
            if (!Has(expected))
                throw Report($"Expected {expected}, got {lookahead}");

            string value = lookahead.Value;
            lookahead = scanner.ScanOne();
            return value;
        }

        private bool Has(params Token[] tokens)
            => tokens.Contains(lookahead.Kind);

        private Exception Report(string message)
            => new Exception($"ERROR [line {scanner.Line}]: {message}");
    }

}
