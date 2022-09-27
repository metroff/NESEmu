namespace NESEmu
{
    public class Console
    {
        byte[] memory;

        CPU cpu;

        public Console() {
            cpu = new CPU(this);
            memory = new byte[0xffff];
        }

        public byte cpuRead(ushort addr) {
            return memory[addr];
        }

        public void run() {
            while(true) {
                cpu.clock();
            }
        }

        public void load(byte[] program) {
            program.CopyTo(memory, 0);
        }
    }
}