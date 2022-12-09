namespace NESEmu
{
    public class Bus
    {
        byte[] cpuRam;

        CPU cpu;
        Rom rom;

        public Bus(Rom rom) {
            cpu = new CPU(this);
            cpuRam = new byte[2048];
            this.rom = rom;
        }

        public byte memoryRead(ushort address) {
            if (address >= 0x0000 && address <= 0x07ff) {
                return cpuRam[address & 0x07ff];
            } 
            else if (address >= 0x2000 && address <= 0x3fff) {
                // address & 0x2007
                throw new Exception("PPU is not supported");
            }
            else if (address >= 0x8000 && address <= 0xffff) {
                return readPrgRom(address);
            }
            else {
                System.Console.WriteLine("Ignoring memory read at {0}", address);
                return 0;
            }
        }

        public void memoryWrite(ushort address, byte data) {
            if (address >= 0x0000 && address <= 0x07ff) {
                cpuRam[address & 0x07ff] = data;
            } 
            else if (address >= 0x2000 && address <= 0x3fff) {
                // [address & 0x2007] = data
                throw new Exception("PPU is not supported");
            }
            else if (address >= 0x8000 && address <= 0xffff) {
                throw new Exception("Atempting to write to memory at " + address.ToString());
            }
            else {
                System.Console.WriteLine("Ignoring memory write at {0}", address);
            }
            
        }

        public byte readPrgRom(ushort address) {
            address -= 0x8000;
            if (rom.prgRom.Count == 0x4000 && address >= 0x4000) {
                address = (ushort) (address & 0x3fff);
            }
            return rom.prgRom[address];
        }

        public void run() {
            while(true) {
                cpu.clock();
            }
        }
    }
}