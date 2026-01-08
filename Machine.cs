using SVM.OperetionCodes;

namespace SVM;

public class Machine
{
    private readonly byte[] memory;
    int ip;
    int sp;
    int fp;
    public bool DebugMode { get; set; } = false;
    public bool TraceMode { get; set; } = false;

    public Dictionary<int, string> Breakpoints = [];
    private Dictionary<int, string> InstructionNames = [];

    const int MemorySize = 1024 * 16;

    public Machine()
    {
        ip = 0;
        sp = 0;
        fp = 0;
        memory = new byte[MemorySize];
    }

    public void Load(byte[] loadData, Dictionary<int, string> instructionNames = null, Dictionary<int, string> breakpoints = null)
    {
        if (loadData.Length > memory.Length)
        {
            throw new Exception("Program too large for memory.");
        }

        Array.Copy(loadData, memory, loadData.Length);

        ip = 0;
        sp = loadData.Length;
        fp = sp;

        InstructionNames = instructionNames != null ? new Dictionary<int, string>(instructionNames) : [];

        Breakpoints = breakpoints != null ? new Dictionary<int, string>(breakpoints) : [];
    }

    public bool Step()
    {
        int currentIp = ip;
        byte command = memory[ip++];
        byte mode = (byte)(command & 0xC0);
        OperationCode opcode = (OperationCode)(command & 0x3F);

        switch (opcode)
        {
            case OperationCode.Nop:
                break;

            case OperationCode.Push:
                Push(mode);
                break;

            case OperationCode.Pop:
                Pop();
                break;

            case OperationCode.Neg:
                Negation();
                break;

            case OperationCode.Not:
                Not();
                break;

            case OperationCode.Mul:
                Binary((a, b) => a * b);
                break;

            case OperationCode.Div:
                Binary((a, b) => a / b);
                break;

            case OperationCode.Mod:
                Binary((a, b) => a % b);
                break;

            case OperationCode.And:
                Binary((a, b) => a & b);
                break;

            case OperationCode.Or:
                Binary((a, b) => a | b);
                break;

            case OperationCode.Eq:
                Comparison((a, b) => a == b);
                break;

            case OperationCode.Ne:
                Comparison((a, b) => a != b);
                break;

            case OperationCode.Lt:
                Comparison((a, b) => a < b);
                break;

            case OperationCode.Le:
                Comparison((a, b) => a <= b);
                break;

            case OperationCode.Gt:
                Comparison((a, b) => a > b);
                break;

            case OperationCode.Ge:
                Comparison((a, b) => a >= b);
                break;

            case OperationCode.Add:
                Binary((a, b) => a + b);
                break;

            case OperationCode.Sub:
                Binary((a, b) => a - b);
                break;

            case OperationCode.Jump:
                Jump();
                break;

            case OperationCode.Jz:
                Jz();
                break;

            case OperationCode.Call:
                Call();
                break;

            case OperationCode.Ret:
                Ret();
                break;

            case OperationCode.Input:
                Input();
                break;

            case OperationCode.Print:
                Print();
                break;

            case OperationCode.Halt:
                if (TraceMode)
                {
                    OperationCodes.Mnemonics.TryGetValue(opcode, out var name);
                    Console.WriteLine($"TRACE IP={ip:X4} {name} SP={sp} FP={fp}");
                }
                return false;

            case OperationCode.Shl:
                Shl();
                break;

            case OperationCode.Shr:
                Shr();
                break;

            case OperationCode.Rol:
                Rol();
                break;

            case OperationCode.Ror:
                Ror();
                break;

            default:
                throw new Exception($"Unknown opcode: {opcode}");
        }

        if (TraceMode)
        {
            OperationCodes.Mnemonics.TryGetValue(opcode, out var name);
            Console.WriteLine($"TRACE IP={ip:X4} {name} SP={sp} FP={fp}");
        }

        if (DebugMode && Breakpoints != null && Breakpoints.TryGetValue(ip, out string bpName))
        {
            Console.WriteLine($"--- BREAKPOINT at {ip:X4} ({bpName}) ---");
            DumpState();
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }

        return true;
    }

    private void Push(byte mode)
    {
        int value;

        switch (mode)
        {
            case OperationCodes.AddressingMode.Immediate:
                {
                    value = ReadInt32(ip);
                    ip += 4;
                    break;
                }

            case OperationCodes.AddressingMode.Indirect:
                {
                    ushort raddr = ReadUInt16(ip);
                    ip += 2;
                    int address = ResolveRelativeAddress(raddr);
                    value = ReadInt32(address);
                    break;
                }

            default:
                throw new Exception("Invalid PUSH addressing mode");
        }

        BasicPush(value);
    }

    private void Pop()
    {
        ushort raddr = ReadUInt16(ip);
        ip += 2;
        int address = ResolveRelativeAddress(raddr);
        int value = BasicPop();
        WriteInt32(address, value);
    }

    private ushort ReadUInt16(int addr)
    {
        return BitConverter.ToUInt16(memory, addr);
    }

    private int ResolveRelativeAddress(ushort relative)
    {
        int address = (short)((relative << 2) >> 2);
        ushort register = (ushort)(relative & 0xC000);

        switch (register)
        {
            case OperationCodes.Registers.InstructionPointer:
                address += ip;
                break;
            case OperationCodes.Registers.StackPointer:
                address += sp;
                break;
            case OperationCodes.Registers.FramePointer:
                address += fp;
                break;
        }

        return address;
    }

    private int ReadInt32(int addr)
    {
        return BitConverter.ToInt32(memory, addr);
    }

    private void WriteInt32(int addr, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        Array.Copy(bytes, 0, memory, addr, 4);
    }

    private void BasicPush(int value)
    {
        WriteInt32(sp, value);
        sp += 4;
    }

    private int BasicPop()
    {
        if (sp - 4 < 0)
        {
            throw new Exception("Stack underflow");
        }

        sp -= 4;
        return ReadInt32(sp);
    }

    private void Binary(Func<int, int, int> op)
    {
        int b = BasicPop();
        int a = BasicPop();
        BasicPush(op(a, b));
    }

    private void Print()
    {
        int value = BasicPop();
        Console.WriteLine(value);
    }

    public void Run()
    {
        while (Step()) { }
    }

    ushort ReadWord(int address)
    {
        return BitConverter.ToUInt16(memory, address);
    }

    private void Jump()
    {
        ushort address = ReadWord(ip);
        ip = address;
    }

    private void Jz()
    {
        ushort address = ReadWord(ip);
        ip += 2;

        int value = BasicPop();

        if (value == 0)
        {
            ip = address;
        }
    }

    private void Negation()
    {
        int value = BasicPop();
        BasicPush(-value);
    }

    private void Not()
    {
        int value = BasicPop();
        BasicPush(~value);
    }

    private void Comparison(Func<int, int, bool> op)
    {
        int right = BasicPop();
        int left = BasicPop();
        BasicPush(op(left, right) ? 1 : 0);
    }

    private void Call()
    {
        ushort address = ReadWord(ip);
        ip += 2;

        BasicPush(ip);
        BasicPush(fp);

        fp = sp;
        ip = address;
    }

    private void Ret()
    {
        int value = BasicPop();

        sp = fp;
        fp = (short)BasicPop();
        ip = (short)BasicPop();

        BasicPush(value);
    }

    private void Input()
    {
        int value = int.Parse(Console.ReadLine()!);
        BasicPush(value);
    }

    private void DumpState()
    {
        Console.WriteLine($"IP: {ip}, SP: {sp}, FP: {fp}");
        Console.WriteLine("Top of stack:");
        for (int i = Math.Max(sp - 16, 0); i < sp; i += 4)
        {
            if (i + 4 <= memory.Length)
            {
                int val = BitConverter.ToInt32(memory, i);
                Console.WriteLine($"[{i:X4}] = {val}");
            }
        }

        Console.WriteLine("Next instruction bytes:");
        for (int i = ip; i < Math.Min(ip + 8, memory.Length); i++)
        {
            Console.Write($"{memory[i]:X2} ");
        }
        Console.WriteLine();
    }

    private void Shl()
    {
        int value = BasicPop();
        BasicPush(value << 1);
    }

    private void Shr()
    {
        int value = BasicPop();
        BasicPush((int)((uint)value >> 1));
    }

    private void Rol()
    {
        int value = BasicPop();
        int result = (value << 1) | (int)((uint)value >> 31);
        BasicPush(result);
    }

    private void Ror()
    {
        int value = BasicPop();
        int result = (int)((uint)value >> 1) | (value << 31);
        BasicPush(result);
    }
}
