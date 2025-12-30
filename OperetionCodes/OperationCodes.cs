namespace SVM.OperetionCodes;

public static class OperationCodes
{
    public static string GetName(byte code)
    {
        if (Mnemonics.TryGetValue((OperationCode)code, out var name))
        {
            return name;
        }
        return "UNKNOWN";
    }

    public static readonly OperationCode[] Codes =
    [
            OperationCode.Nop,
            OperationCode.Push,
            OperationCode.Pop,
            OperationCode.Call,
            OperationCode.Ret,
            OperationCode.Jump,
            OperationCode.Jz,
            OperationCode.Halt,

            OperationCode.Input,
            OperationCode.Print,

            OperationCode.Add,
            OperationCode.Sub,
            OperationCode.Mul,
            OperationCode.Div,
            OperationCode.Mod,
            OperationCode.Neg,

            OperationCode.And,
            OperationCode.Or,
            OperationCode.Not,

            OperationCode.Eq,
            OperationCode.Ne,
            OperationCode.Lt,
            OperationCode.Le,
            OperationCode.Gt,
            OperationCode.Ge
        ];

    public static readonly Dictionary<OperationCode, string> Mnemonics =
        new()
        {
                { OperationCode.Nop, "NOP" },
                { OperationCode.Push, "PUSH" },
                { OperationCode.Pop,  "POP" },
                { OperationCode.Call, "CALL" },
                { OperationCode.Ret,  "RET" },
                { OperationCode.Jump, "JUMP" },
                { OperationCode.Jz,   "JZ" },
                { OperationCode.Halt, "HALT" },

                { OperationCode.Input, "INPUT" },
                { OperationCode.Print, "PRINT" },

                { OperationCode.Add, "ADD" },
                { OperationCode.Sub, "SUB" },
                { OperationCode.Mul, "MUL" },
                { OperationCode.Div, "DIV" },
                { OperationCode.Mod, "MOD" },
                { OperationCode.Neg, "NEG" },

                { OperationCode.And, "AND" },
                { OperationCode.Or,  "OR" },
                { OperationCode.Not, "NOT" },

                { OperationCode.Eq, "EQ" },
                { OperationCode.Ne, "NE" },
                { OperationCode.Lt, "LT" },
                { OperationCode.Le, "LE" },
                { OperationCode.Gt, "GT" },
                { OperationCode.Ge, "GE" },

                { OperationCode.Shl, "SHL" },
                { OperationCode.Shr, "SHR" },
                { OperationCode.Rol, "ROL" },
                { OperationCode.Ror, "ROR" }
        };

    public static class AddressingMode
    {
        public const byte Basic = 0x00;
        public const byte Immediate = 0x40;
        public const byte Indirect = 0x80;
    }

    public static class Registers
    {
        public const ushort StackPointer = 0x4000;
        public const ushort FramePointer = 0x8000;
        public const ushort InstructionPointer = 0xC000;
    }

    public struct Instruction(OperationCode opcode, int immediate = 0, ushort indirect = 0)
    {
        public OperationCode Opcode = opcode;
        public int Immediate = immediate;
        public ushort Indirect = indirect;
    }
}

public enum OperationCode : byte
{
    Nop = 0x00,
    Push = 0x01,
    Pop = 0x02,
    Call = 0x03,
    Ret = 0x04,
    Jump = 0x05,
    Jz = 0x06,
    Halt = 0x07,

    Input = 0x08,
    Print = 0x09,

    Add = 0x0A,
    Sub = 0x0B,
    Mul = 0x0C,
    Div = 0x0D,
    Mod = 0x0E,
    Neg = 0x0F,

    And = 0x10,
    Or = 0x11,
    Not = 0x12,

    Eq = 0x13,
    Ne = 0x14,
    Lt = 0x15,
    Le = 0x16,
    Gt = 0x17,
    Ge = 0x18,

    Shl = 0x22,
    Shr = 0x23,
    Rol = 0x24,
    Ror = 0x25
}


