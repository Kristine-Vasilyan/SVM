using SVM.OperetionCodes;

namespace SVM.Assembler
{

    public sealed class Assembler
    {
        public byte[] Assemble(TextReader reader)
        {
            var scanner = new Scanner(reader);
            var builder = new Builder();
            var parser = new Parser(scanner, builder);

            parser.Parse();
            builder.Validate();

            return builder.Bytes();
        }
    }
}
