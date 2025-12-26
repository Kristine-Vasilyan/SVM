using SVM.OperetionCodes;

namespace SVM
{
    internal class Program
    {
        static void Main()
        {
            var b = new Builder();

            b.PushI(10);
            b.PushI(2);
            b.Div();
            b.Print();
            b.Halt();


            b.Validate();

            var vm = new Machine();
            vm.Load(b.Bytes());
            vm.Run();


        }
    }
}
