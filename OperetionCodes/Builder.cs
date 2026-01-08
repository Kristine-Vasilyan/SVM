namespace SVM.OperetionCodes
{
    public class Builder
    {
        private readonly List<Instruction> instructions = [];
        private readonly Dictionary<string, int> labels = [];
        private readonly Dictionary<Instruction, string> unresolved = [];

        private int offset = 0;

        public Dictionary<int, string> Breakpoints { get; } = [];

        public void AddBreakpointPlaceholder()
        {
            Breakpoints[offset] = "<pending>";
        }

        public byte[] Bytes()
        {
            using var ms = new MemoryStream();
            foreach (var instr in instructions)
            {
                ms.Write(instr.Bytes());
            }
            return ms.ToArray();
        }

        public void SetLabel(string name)
        {
            if (!labels.ContainsKey(name))
                labels[name] = offset;
        }

        public void AddBasic(byte opcode)
        {
            var instr = new Instruction
            {
                Opcode = (byte)(opcode | AddressingMode.Basic)
            };
            Add(instr);
        }

        public void AddWithNumeric(byte opcode, int number)
        {
            var instr = new Instruction
            {
                Opcode = (byte)(opcode | AddressingMode.Immediate),
                Immediate = number
            };
            Add(instr);
        }

        public void AddWithAddress(byte opcode, ushort register, short displacement)
        {
            var instr = new Instruction
            {
                Opcode = (byte)(opcode | AddressingMode.Indirect),
                Indirect = (ushort)(register | (ushort)displacement)
            };

            Add(instr);
        }

        public void AddWithLabel(byte opcode, string label)
        {
            var instr = new Instruction
            {
                Opcode = (byte)(opcode | AddressingMode.Indirect)
            };

            unresolved[instr] = label;
            Add(instr);
        }

        private void Add(Instruction instr)
        {
            int instrIp = offset;

            if (Breakpoints.TryGetValue(instrIp, out var name) && name == "<pending>")
            {
                Breakpoints[instrIp] = OperationCodes.GetName(instr.Opcode);
            }

            instr.Address = offset;
            offset += instr.Size();
            instructions.Add(instr);
        }

        public bool Validate()
        {
            foreach (var pair in unresolved)
            {
                var instr = pair.Key;
                var label = pair.Value;

                if (!labels.TryGetValue(label, out int value))
                {
                    throw new Exception($"Unknown label: {label}");
                }

                instr.Indirect = (ushort)value;
            }
            return true;
        }

        public void PushI(int value) => AddWithNumeric((byte)OperationCode.Push, value);

        public void PushA(ushort register, short displacement) => AddWithAddress((byte)OperationCode.Push, register, displacement);

        public void PopA(ushort register, short displacement) => AddWithAddress((byte)OperationCode.Pop, register, displacement);

        // ===== Control flow =====

        public void Call(string label) => AddWithLabel((byte)OperationCode.Call, label);

        public void Ret() => AddBasic((byte)OperationCode.Ret);

        public void Jump(string label) => AddWithLabel((byte)OperationCode.Jump, label);

        public void Jz(string label) => AddWithLabel((byte)OperationCode.Jz, label);

        // ===== I/O =====

        public void Halt() => AddBasic((byte)OperationCode.Halt);

        public void Input() => AddBasic((byte)OperationCode.Input);

        public void Print() => AddBasic((byte)OperationCode.Print);

        // ===== Arithmetic =====

        public void Add() => AddBasic((byte)OperationCode.Add);

        public void Sub() => AddBasic((byte)OperationCode.Sub);

        public void Mul() => AddBasic((byte)OperationCode.Mul);

        public void Div() => AddBasic((byte)OperationCode.Div);

        public void Mod() => AddBasic((byte)OperationCode.Mod);

        public void Neg() => AddBasic((byte)OperationCode.Neg);

        // ===== Logical =====

        public void And() => AddBasic((byte)OperationCode.And);

        public void Or() => AddBasic((byte)OperationCode.Or);

        public void Not() => AddBasic((byte)OperationCode.Not);

        // ===== Comparison =====

        public void Eq() => AddBasic((byte)OperationCode.Eq);

        public void Ne() => AddBasic((byte)OperationCode.Ne);

        public void Lt() => AddBasic((byte)OperationCode.Lt);

        public void Le() => AddBasic((byte)OperationCode.Le);

        public void Gt() => AddBasic((byte)OperationCode.Gt);

        public void Ge() => AddBasic((byte)OperationCode.Ge);

        public void Shl() => AddBasic((byte)OperationCode.Shl);

        public void Shr() => AddBasic((byte)OperationCode.Shr);

        public void Rol() => AddBasic((byte)OperationCode.Rol);

        public void Ror() => AddBasic((byte)OperationCode.Ror);
    }
}
