using SVM.OperetionCodes;

namespace SVM
{
    internal class Program
    {
        static void Main()
        {
            ///1
            //var program = new List<byte>();

            //program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            //program.AddRange(BitConverter.GetBytes(5));

            //program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            //program.AddRange(BitConverter.GetBytes(3));

            //program.Add((byte)OperationCode.Add);
            //program.Add((byte)OperationCode.Print);
            //program.Add((byte)OperationCode.Halt);

            //Machine vm = new Machine();
            //vm.Load(program.ToArray());
            //vm.Run();

            ///2
            //var program = new List<byte>();

            //program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            //program.AddRange(BitConverter.GetBytes(10));

            //program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            //program.AddRange(BitConverter.GetBytes(2));

            //program.Add((byte)OperationCode.Sub);

            //program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            //program.AddRange(BitConverter.GetBytes(3));

            //program.Add((byte)OperationCode.Mul);
            //program.Add((byte)OperationCode.Print);
            //program.Add((byte)OperationCode.Halt);

            //var vm = new Machine();
            //vm.Load(program.ToArray());

            //using var sw = new StringWriter();
            //Console.SetOut(sw);

            //vm.Run();

            ///3
            var program = new List<byte>();
            program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            program.AddRange(BitConverter.GetBytes(0));

            program.Add((byte)OperationCode.Jz);
            program.AddRange(BitConverter.GetBytes((ushort)14));

            program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            program.AddRange(BitConverter.GetBytes(99));
            program.Add((byte)OperationCode.Print);

            program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            program.AddRange(BitConverter.GetBytes(42));
            program.Add((byte)OperationCode.Print);

            program.Add((byte)OperationCode.Halt);

            var vm = new Machine();
            vm.Load(program.ToArray());

            using var sw = new StringWriter();
            Console.SetOut(sw);

            vm.Run();

            //4
            //var program = new List<byte>();

            //program.Add((byte)OperationCode.Call);
            //program.AddRange(BitConverter.GetBytes((ushort)6));

            //program.Add((byte)OperationCode.Print);
            //program.Add((byte)OperationCode.Halt);

            //program.Add((byte)(((byte)OperationCode.Push) | OperationCodes.AddressingMode.Immediate));
            //program.AddRange(BitConverter.GetBytes(10));

            //program.Add((byte)OperationCode.Ret);

            //var vm = new Machine();
            //vm.Load(program.ToArray());

            //using var sw = new StringWriter();
            //Console.SetOut(sw);

            //vm.Run();

        }
    }
}
