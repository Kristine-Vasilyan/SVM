using System.Text;

namespace SVM.OperetionCodes
{
    public class Instruction
    {
        public int Address;
        public byte Opcode;
        public int Immediate;
        public ushort Indirect;

        public int Size()
        {
            int size = 1;
            switch (Opcode & 0xC0)
            {
                case AddressingMode.Immediate:
                    size += 4;
                    break;
                case AddressingMode.Indirect:
                    size += 2;
                    break;
            }
            return size;
        }

        public byte[] Bytes()
        {
            byte[] result = new byte[Size()];
            result[0] = Opcode;

            switch (Opcode & 0xC0)
            {
                case AddressingMode.Immediate:
                    BitConverter.GetBytes(Immediate)
                                .CopyTo(result, 1);
                    break;

                case AddressingMode.Indirect:
                    BitConverter.GetBytes(Indirect)
                                .CopyTo(result, 1);
                    break;
            }
            return result;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{Address:X4}");
            foreach (var b in Bytes())
                sb.Append($" {b:X2}");
            return sb.ToString();
        }
    }

}
