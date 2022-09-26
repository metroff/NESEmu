

namespace NESEmu
{
    class Program
    {
        static void Main(string[] args) {
            CPU cpu = new CPU();

            cpu.step();
        }
    }
}
