namespace SVM
{
    internal class Program
    {
        static void Main()
        {
            //var source = """
            //            PUSH 10
            //            PUSH 20
            //            ADD
            //            PRINT
            //            HALT
            //            """;

            var source = """
                        PUSH 7
                        PUSH 3
                        MUL
                        # breakpoint
                        PRINT 
                        HALT
                        """;

            //var source = """
            //            PUSH 0
            //            JZ skip
            //            PUSH 100
            //            PRINT
            //            skip:
            //            PUSH 200
            //            PRINT
            //            HALT
            //            """;


            //var source = """
            //            CALL addTwo
            //            HALT

            //            addTwo:
            //                PUSH 2
            //                PUSH 3
            //                ADD
            //                PRINT
            //                RET
            //            """;

            var assembler = new Assembler.Assembler();
            byte[] program = assembler.Assemble(new StringReader(source));

            var vm = new Machine { DebugMode = true };
            vm.Load(program);
            vm.LoadBreakpoints(assembler.Builder.Breakpoints);
            vm.Run();
        }

    }
}
