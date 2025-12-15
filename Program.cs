using SVM.OperetionCodes;

namespace SVM
{
    internal class Program
    {
        static void Main()
        {
            var program = new List<byte>();

            program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            program.AddRange(BitConverter.GetBytes(5));

            program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            program.AddRange(BitConverter.GetBytes(3));

            program.Add((byte)OperationCode.Add);
            program.Add((byte)OperationCode.Print);
            program.Add((byte)OperationCode.Halt);

            Machine vm = new Machine();
            vm.Load(program.ToArray());
            vm.Run();
        }
    }
}
