namespace NESEmu
{
    public class Bus
    {
        byte[] cpuRam;

        CPU cpu;

        public Bus() {
            cpu = new CPU(this);
            cpuRam = new byte[0x10000];
        }

        public byte memoryRead(ushort address) {
            if (address >= 0x0000 && address <= 0x07ff) {
                return cpuRam[address & 0x07ff];
            }
            return cpuRam[address];
        }

        public void memoryWrite(ushort address, byte data) {
            if (address >= 0x0000 && address <= 0x07ff) {
                cpuRam[address & 0x07ff] = data;
            }
            cpuRam[address] = data;
        }

        public void run() {
            while(true) {
                cpu.clock();
            }
        }
    }
}