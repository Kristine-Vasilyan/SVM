using SVM.OperetionCodes;

namespace SVM.Assembler
{
    public sealed class Parser
    {
        private readonly Scanner scanner;
        private readonly Builder builder;
        private Lexeme current;
        private Lexeme previousOperation = null;

        public Parser(Scanner scanner, Builder builder)
        {
            this.scanner = scanner;
            this.builder = builder;
            Advance();
        }

        private void Advance()
        {
            if (current != null && current.Kind == Token.Operation)
            {
                previousOperation = current;
            }
            current = scanner.ScanOne();
        }

        private void Expect(Token kind)
        {
            if (current.Kind != kind)
            {
                throw new Exception($"Expected {kind}, got {current.Kind}");
            }
            Advance();
        }

        public void Parse()
        {
            while (current.Kind != Token.Eos)
            {
                ParseLine();
            }

            builder.Validate();
        }

        private void ParseLine()
        {
            if (current.Kind == Token.NewLine)
            {
                Advance();
                return;
            }

            if (current.Kind == Token.Breakpoint)
            {
                string instrName = "UNKNOWN";
                if (previousOperation != null && previousOperation.Kind == Token.Operation)
                    instrName = previousOperation.Value;

                builder.AddBreakpoint(instrName);
                Advance();
                return;
            }

            if (current.Kind == Token.Ident)
            {
                string name = current.Value;
                Advance();

                if (current.Kind == Token.Colon)
                {
                    Advance();
                    builder.SetLabel(name);
                    return;
                }

                throw new Exception("Unexpected identifier");
            }

            if (current.Kind == Token.Operation)
            {
                ParseInstruction();
                return;
            }

            throw new Exception($"Unexpected token {current}");
        }

        private void ParseInstruction()
        {
            string op = current.Value.ToUpper();
            Advance();

            switch (op)
            {
                case "NOP": builder.AddBasic((byte)OperationCode.Nop); break;
                case "RET": builder.Ret(); break;
                case "HALT": builder.Halt(); break;

                case "ADD": builder.Add(); break;
                case "SUB": builder.Sub(); break;
                case "MUL": builder.Mul(); break;
                case "DIV": builder.Div(); break;
                case "MOD": builder.Mod(); break;

                case "NEG": builder.Neg(); break;

                case "AND": builder.And(); break;
                case "OR": builder.Or(); break;
                case "NOT": builder.Not(); break;

                case "EQ": builder.Eq(); break;
                case "NE": builder.Ne(); break;
                case "LT": builder.Lt(); break;
                case "LE": builder.Le(); break;
                case "GT": builder.Gt(); break;
                case "GE": builder.Ge(); break;

                case "INPUT": builder.Input(); break;
                case "PRINT": builder.Print(); break;

                // stack 
                case "PUSH":
                    ParsePush();
                    break;

                case "POP":
                    ParsePop();
                    break;

                // control flow
                case "CALL":
                    ParseLabelOperand(builder.Call);
                    break;

                case "JUMP":
                    ParseLabelOperand(builder.Jump);
                    break;

                case "JZ":
                    ParseLabelOperand(builder.Jz);
                    break;

                default:
                    throw new Exception($"Unknown instruction {op}");
            }
        }

        // OPERANDS

        private void ParsePush()
        {
            if (current.Kind == Token.Number)
            {
                builder.PushI(int.Parse(current.Value));
                Advance();
                return;
            }

            ParseIndirect(builder.PushA);
        }

        private void ParsePop()
        {
            ParseIndirect(builder.PopA);
        }

        private void ParseLabelOperand(Action<string> emit)
        {
            if (current.Kind != Token.Ident)
                throw new Exception("Expected label");

            emit(current.Value);
            Advance();
        }

        private void ParseIndirect(Action<ushort, short> emit)
        {
            Expect(Token.LeftBr);

            if (current.Kind != Token.Register)
            {
                throw new Exception("Expected register");
            }

            ushort reg = current.Value switch
            {
                "SP" => OperationCodes.Registers.StackPointer,
                "FP" => OperationCodes.Registers.FramePointer,
                "IP" => OperationCodes.Registers.InstructionPointer,
                _ => throw new Exception("Unknown register")
            };

            Advance();

            int sign = 1;
            if (current.Kind == Token.Plus || current.Kind == Token.Minus)
            {
                sign = current.Kind == Token.Minus ? -1 : 1;
                Advance();
            }

            if (current.Kind != Token.Number)
            {
                throw new Exception("Expected displacement");
            }

            short offset = (short)(int.Parse(current.Value) * sign);
            Advance();

            Expect(Token.RightBr);

            emit(reg, offset);
        }
    }
}
