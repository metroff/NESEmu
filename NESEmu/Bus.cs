namespace NESEmu
{
    public class Bus
    {
        byte[] cpuRam;

        CPU cpu;
        Rom rom;
        PPU ppu;

        // uint _cycles;

        public delegate void gameloopDel(ref PPU ppu, ref Joypad joypad);
        public gameloopDel gameloop_callback;
        Joypad joypad1;

        public Bus(Rom rom, gameloopDel gameloop_callback) {
            cpu = new CPU(this);
            cpuRam = new byte[2048];
            this.rom = rom;
            ppu = new PPU(rom.chrRom, rom.screenMirroring);
            // _cycles = 0;
            this.gameloop_callback = gameloop_callback;
            // joypad1 = new Joypad();
            joypad1 = new SDLKeyboard();
        }

        // Prieš pertvarkymą kognityvinis kompleksiškumas yra 17
        // public byte memoryRead(ushort address) {
        //     if (address >= 0x0000 && address <= 0x1fff) {
        //         return cpuRam[address & 0x07ff];
        //     } 
        //     else if (address == 0x2000 || address == 0x2001 || address == 0x2003 || address == 0x2005 || address == 0x2006 || address == 0x4014) {
        //         // throw new Exception(string.Format("Attempt to read from write-only PPU address {0:X}", address));
        //         return 0;
        //     }
        //     else if (address == 0x2002) {
        //         return ppu.readStatus();
        //     }
        //     else if (address == 0x2004) {
        //         return ppu.readOamData();
        //     }
        //     else if (address == 0x2007) {
        //         return ppu.readData();
        //     }
        //     else if (address >= 0x4000 && address <= 0x4015) {
        //         // ignore APU
        //         return 0;
        //     }
        //     else if (address == 0x4016) {
        //         return joypad1.read();
        //     }
        //     else if (address == 0x4017) {
        //         // ignore joypad 2
        //         return 0;
        //     }
        //     else if (address >= 0x2008 && address <= 0x3fff) {
        //         return memoryRead((ushort)(address & 0x2007));
        //     }
        //     else if (address >= 0x8000 && address <= 0xffff) {
        //         return readPrgRom(address);
        //     }
        //     else {
        //         System.Console.WriteLine("Ignoring memory read at {0}", address);
        //         return 0;
        //     }
        // }

        // Prieš pertvarkymą kognityvinis kompleksiškumas yra 2
        public byte memoryRead(ushort address)
        {
            switch (address)
            {
                case >= 0x0000 and <= 0x1fff:
                    return cpuRam[address & 0x07ff];
                case 0x2000:
                case 0x2001:
                case 0x2003:
                case 0x2005:
                case 0x2006:
                case 0x4014:
                    // throw new Exception(string.Format("Attempt to read from write-only PPU address {0:X}", address));
                    return 0;
                case 0x2002:
                    return ppu.readStatus();
                case 0x2004:
                    return ppu.readOamData();
                case 0x2007:
                    return ppu.readData();
                case >= 0x4000 and <= 0x4015:
                    // ignore APU
                    return 0;
                case 0x4016:
                    return joypad1.read();
                case 0x4017:
                    // ignore joypad 2
                    return 0;
                case >= 0x2008 and <= 0x3fff:
                    return memoryRead((ushort)(address & 0x2007));
                case >= 0x8000 and <= 0xffff:
                    return readPrgRom(address);
                default:
                    System.Console.WriteLine("Ignoring memory read at {0}", address);
                    return 0;
            }
        }

        // Prieš pertvarkymą kognityvinis kompleksiškumas yra 24
        // public void memoryWrite(ushort address, byte data) {
        //     if (address >= 0x0000 && address <= 0x1fff) {
        //         cpuRam[address & 0x07ff] = data;
        //     } 
        //     else if (address == 0x2000) {
        //         ppu.ctrlWrite(data);
        //     }
        //     else if (address == 0x2001) {
        //         ppu.maskWrite(data);
        //     }
        //     else if (address == 0x2002) {
        //         throw new Exception("Atempting to write to ppu status register");
        //     }
        //     else if (address == 0x2003) {
        //         ppu.writeOamAddress(data);
        //     }
        //     else if (address == 0x2004) {
        //         ppu.writeOamData(data);
        //     }
        //     else if (address == 0x2005) {
        //         ppu.scrollWrite(data);
        //     }
        //     else if (address == 0x2006) {
        //         ppu.addrWrite(data);
        //     }
        //     else if (address == 0x2007) {
        //         ppu.writeData(data);
        //     }
        //     else if ((address >= 0x4000 && address <= 0x4013) || address == 0x4015) {
        //         // ignore APU
        //     }
        //     else if (address == 0x4016) {
        //         joypad1.write(data);
        //     }
        //     else if (address == 0x4017) {
        //         // ignore joypad 2
        //     }
        //     else if (address == 0x4014) {
        //         byte[] buffer = Utility.fillArray(0, 256);
        //         ushort hi = (ushort)((ushort)data << 8);

        //         for (ushort i = 0; i < 256; i++)
        //         {
        //             buffer[i] = memoryRead((ushort)(hi + i));
        //         }
        //         ppu.writeOamDma(buffer);
        //     }
        //     else if (address >= 0x2008 && address <= 0x3fff) {
        //         memoryWrite((ushort)(address & 0x2007), data);
        //     }
        //     else if (address >= 0x8000 && address <= 0xffff) {
        //         throw new Exception("Atempting to write to ROM memory at " + address.ToString());
        //     }
        //     else {
        //         System.Console.WriteLine("Ignoring memory write at {0:X}", address);
        //     }

        // }

        // Prieš pertvarkymą kognityvinis kompleksiškumas yra 3
        public void memoryWrite(ushort address, byte data)
        {
            switch (address)
            {
                case >= 0x0000 and <= 0x1fff:
                    cpuRam[address & 0x07ff] = data;
                    break;
                case 0x2000:
                    ppu.ctrlWrite(data);
                    break;
                case 0x2001:
                    ppu.maskWrite(data);
                    break;
                case 0x2002:
                    throw new Exception("Atempting to write to ppu status register");
                case 0x2003:
                    ppu.writeOamAddress(data);
                    break;
                case 0x2004:
                    ppu.writeOamData(data);
                    break;
                case 0x2005:
                    ppu.scrollWrite(data);
                    break;
                case 0x2006:
                    ppu.addrWrite(data);
                    break;
                case 0x2007:
                    ppu.writeData(data);
                    break;
                case >= 0x4000 and <= 0x4013:
                case 0x4015:
                    // ignore APU
                    break;
                case 0x4016:
                    joypad1.write(data);
                    break;
                case 0x4017:
                    // ignore joypad 2
                    break;
                case 0x4014:
                {
                    byte[] buffer = Utility.fillArray(0, 256);
                    ushort hi = (ushort)((ushort)data << 8);

                    for (ushort i = 0; i < 256; i++)
                    {
                        buffer[i] = memoryRead((ushort)(hi + i));
                    }
                    ppu.writeOamDma(buffer);
                    break;
                }
                case >= 0x2008 and <= 0x3fff:
                    memoryWrite((ushort)(address & 0x2007), data);
                    break;
                case >= 0x8000 and <= 0xffff:
                    throw new Exception("Atempting to write to ROM memory at " + address.ToString());
                default:
                    System.Console.WriteLine("Ignoring memory write at {0:X}", address);
                    break;
            }
        }

        public byte readPrgRom(ushort address) {
            address -= 0x8000;
            if (rom.prgRom.Count == 0x4000 && address >= 0x4000) {
                address = (ushort) (address & 0x3fff);
            }
            return rom.prgRom[address];
        }

        public void tick(byte cycles) {
            // _cycles += cycles;
            
            bool new_frame = ppu.tick((byte)(cycles * 3));
            if (new_frame)
                gameloop_callback(ref ppu, ref joypad1);
        }

        public bool pollNmiInterruptStatus() {
            return ppu.pollNmiInterrupt();
        }

        public void run() {
            while(true) {
                cpu.clock();
            }
        }
    }
}