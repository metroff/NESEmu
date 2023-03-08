namespace NESEmu {
    public class PPU 
    {
        public List<byte> chrRom;
        public byte[] palleteTable;
        public byte[] vram;
        public byte[] oamData;
        public Mirroring _mirroring;
        public AddressRegister _addr;
        public ControlRegister _ctrl;
        MaskRegister _mask;
        public ScrollRegister _scroll;
        public StatusRegister _status;
        byte _internalDataBuffer;
        byte oamAddr;

        ushort _scanline;
        uint _cycles;
        public bool nmiInterrupt;

        public PPU(List<Byte> chrRom, Mirroring mirroring) {
            this.chrRom = chrRom;
            this._mirroring = mirroring;
            this.vram = Utility.fillArray(0, 2048);
            this.oamData = Utility.fillArray(0, 64 * 4);
            this.palleteTable = Utility.fillArray(0, 32);
            _addr = new AddressRegister();
            _ctrl = new ControlRegister();
            _mask = new MaskRegister();
            _scroll = new ScrollRegister();
            _status = new StatusRegister();
            _internalDataBuffer = 0;
            oamAddr = 0;
            _cycles = 0;
            _scanline = 0;
            nmiInterrupt = false;
        }

        public bool tick(byte cycles) {
            _cycles += cycles;
            if (_cycles >= 341) {
                if (isSpriteZeroHit(_cycles)) {
                    _status.setSpriteZeroHit(true);
                }
                _cycles -= 341;
                _scanline += 1;

                if (_scanline == 241) {
                    _status.setVblankStatus(true);
                    _status.setSpriteZeroHit(false);
                    if (_ctrl.generateVblankNmi()) {
                        nmiInterrupt = true;
                    }
                }

                if (_scanline >= 262) {
                    _scanline = 0;
                    nmiInterrupt = false;
                    _status.setSpriteZeroHit(false);
                    _status.resetVblankStatus();
                    return true;
                }
            }
            return false;
        }

        public bool pollNmiInterrupt() {
            if (nmiInterrupt) {
                nmiInterrupt = false;
                return true;
            }
            return nmiInterrupt;
        }

        bool isSpriteZeroHit(uint cycle) {
            ushort y = oamData[0];
            ushort x = oamData[3];
            return (y == _scanline) && (x <= cycle) && _mask.showSprites();
        }

        public void addrWrite(byte value) {
            _addr.update(value);
        }

        public void ctrlWrite(byte value) {
            bool beforeNmiStatus = _ctrl.generateVblankNmi();
            _ctrl.update(value);
            if (!beforeNmiStatus && _ctrl.generateVblankNmi() && _status.inVblank()) {
                nmiInterrupt = true;
            }
        }

        public void maskWrite(byte value) {
            _mask.update(value);
        }

        public byte readStatus() {
            byte data = _status.snapshot();
            _status.resetVblankStatus();
            _addr.reset_latch();
            _scroll.resetLatch();
            return data;
        }

        public void writeOamAddress(byte value) {
            oamAddr = value;
        }

        public void writeOamData(byte value) {
            oamData[oamAddr] = value;
            oamAddr += 1;
        }

        public byte readOamData() {
            return oamData[oamAddr];
        }

        public void scrollWrite(byte value) {
            _scroll.write(value);
        }

        void incrementVramAddr() {
            _addr.increment(_ctrl.vram_addr_increment());
        }

        ushort mirrorVramAddress(ushort addr) {
            ushort mirrored_vram = (ushort)(addr & 0x2fff);
            ushort vram_index = (ushort)(mirrored_vram - 0x2000);
            ushort name_table = (ushort)(vram_index / 0x400);

            switch ((_mirroring, name_table)) {
                case (Mirroring.VERTICAL, 2):
                case (Mirroring.VERTICAL, 3):
                    return (ushort)(vram_index - 0x800);
                case (Mirroring.HORIZONTAL, 2):
                    return (ushort)(vram_index - 0x400);
                case (Mirroring.HORIZONTAL, 1):
                    return (ushort)(vram_index - 0x400);
                case (Mirroring.HORIZONTAL, 3):
                    return (ushort)(vram_index - 0x800);
                default:
                    return vram_index;
            }
        }

        public void writeData(byte value) {
            ushort address = _addr.get();
            if (address >= 0x0000 && address <= 0x1fff) {
                throw new Exception(string.Format("Atempting to write to chr rom space {0:X4}", address));
            } else if (address >= 0x2000 && address <= 0x2fff) {
                vram[mirrorVramAddress(address)] = value;
            } else if (address >= 0x3000 && address <= 0x3eff) {
                throw new Exception(string.Format("Unexpected address write, requested = {0:X4}", address));
            } else if (address == 0x3f10 || address == 0x3f14 || address == 0x3f18 || address == 0x3f1c) {
                ushort mirrored_address = (ushort)(address - 0x10);
                palleteTable[(mirrored_address - 0x3f00)] = value;
            } else if (address >= 0x3f00 && address <= 0x3fff) {
                palleteTable[(address - 0x3f00)] = value;
            } else {
                throw new Exception(string.Format("Unexpected mirrored access {0:X4}", address));
            }
            incrementVramAddr();
        }

        public byte readData() {
            ushort address = _addr.get();
            incrementVramAddr();

            if (address >= 0x0000 && address <= 0x1fff) {
                byte result = _internalDataBuffer;
                _internalDataBuffer = chrRom[address];
                return result;
            } else if (address >= 0x2000 && address <= 0x2fff) {
                byte result = _internalDataBuffer;
                _internalDataBuffer = vram[mirrorVramAddress(address)];
                return result;
            } else if (address >= 0x3000 && address <= 0x3eff) {
                throw new Exception(string.Format("Unexpected address access, requested = {0:X4}", address));
            } else if (address >= 0x3f00 && address <= 0x3fff) {
                return palleteTable[(address - 0x3f00)];
            } else {
                throw new Exception(string.Format("Unexpected mirrored access {0:X4}", address));
            }
        }

        public void writeOamDma(byte[] data) {
            foreach (byte x in data) {
                oamData[oamAddr] = x;
                oamAddr += 1;
            }
        }
    }

    public class AddressRegister
    {
        byte first_value;
        byte second_value;
        bool hi_ptr;

        public AddressRegister() {
            first_value = 0;
            second_value = 0;
            hi_ptr = true;
        }

        public void set(ushort data) {
            first_value = (byte)(data >> 8);
            second_value = (byte)(data & 0xff);
        }

        public void update(byte data) {
            if (hi_ptr) {
                first_value = data;
            } else {
                second_value = data;
            }

            if (get() > 0x3fff) {
                set((ushort)(get() & 0x3fff));
            }
            hi_ptr = !hi_ptr;
        }

        public void increment(byte inc) {
            byte lo = second_value;
            second_value += inc;
            if (lo > second_value) {
                first_value += 1;
            }
            if (get() > 0x3fff) {
                set((ushort)(get() & 0x3fff));
            }
        }

        public void reset_latch() {
            hi_ptr = true;
        }

        public ushort get() {
            return (ushort)(((ushort)first_value << 8) | ((ushort)second_value));
        }
    }

    public class ControlRegister 
    {
        enum FLAGS {
            NAMETABLE1 = (1 << 0),
            NAMETABLE2 = (1 << 1),
            VRAM_ADD_INCREMENT = (1 << 2),
            SPRITE_PATTERN_ADDR = (1 << 3),
            BACKGROUND_PATTERN_ADDR = (1 << 4), 
            SPRITE_SIZE = (1 << 5), 
            MASTER_SLAVE_SELECT = (1 << 6), 
            GENERATE_NMI = (1 << 7), 
        };

        byte register;

        public ControlRegister() {
            register = 0x00;
        }

        public ushort nametable_address() {
            switch (register & 0b11) {
                case 0:
                    return 0x2000;
                case 1:
                    return 0x2400;
                case 2:
                    return 0x2800;
                case 3:
                    return 0x2c00;
                default:
                    throw new Exception("Not possible");
            }
        }

        public byte vram_addr_increment() {
            if (getFlag(FLAGS.VRAM_ADD_INCREMENT) != 1)
                return 1;
            else
                return 32;
        }

        public ushort sprite_pattern_address() {
            if (getFlag(FLAGS.SPRITE_PATTERN_ADDR) != 1)
                return 0;
            else
                return 0x1000;
        }

        public ushort bknd_pattern_address() {
            if (getFlag(FLAGS.BACKGROUND_PATTERN_ADDR) != 1)
                return 0;
            else
                return 0x1000;
        }

        public ushort sprite_size() {
            if (getFlag(FLAGS.SPRITE_SIZE) != 1)
                return 8;
            else
                return 16;
        }

        public ushort master_slave_select() {
            if (getFlag(FLAGS.MASTER_SLAVE_SELECT) != 1)
                return 0;
            else
                return 1;
        }

        public bool generateVblankNmi() {
            return getFlag(FLAGS.GENERATE_NMI) == 1;
        }

        public void update(byte data) {
            register = data;
        }

         void setFlag(FLAGS flag, bool v) {
            if (v)
                register |= (byte) flag;
            else
                register &= unchecked((byte) ~flag);
        }

        byte getFlag(FLAGS flag) {
            return (byte) ((register & (byte) flag) == 0 ? 0 : 1);
        }
    }

    public class MaskRegister
    {
        enum FLAGS {
            GREYSCALE = (1 << 0),
            LEFTMOST_8PXL_BACKGROUND = (1 << 1),
            LEFTMOST_8PXL_SPRITE = (1 << 2),
            SHOW_BACKGROUND = (1 << 3),
            SHOW_SPRITES = (1 << 4), 
            EMPHASISE_RED = (1 << 5), 
            EMPHASISE_GREEN = (1 << 6), 
            EMPHASISE_BLUE = (1 << 7), 
        };

        public enum Color {
            Red,
            Green,
            Blue,
        }

        byte register;

        void setFlag(FLAGS flag, bool v) {
            if (v)
                register |= (byte) flag;
            else
                register &= unchecked((byte) ~flag);
        }

        byte getFlag(FLAGS flag) {
            return (byte) ((register & (byte) flag) == 0 ? 0 : 1);
        }

        public MaskRegister() {
            register = 0x00;
        }

        public bool isGrayscale() {
            return getFlag(FLAGS.GREYSCALE) == 1;
        }

        public bool leftmost8pxlBackground() {
            return getFlag(FLAGS.LEFTMOST_8PXL_BACKGROUND) == 1;
        }

        public bool leftmost8pxlSprite() {
            return getFlag(FLAGS.LEFTMOST_8PXL_SPRITE) == 1;
        }

        public bool showBackground() {
            return getFlag(FLAGS.SHOW_BACKGROUND) == 1;
        }

        public bool showSprites() {
            return getFlag(FLAGS.SHOW_SPRITES) == 1;
        }

        public List<Color> emphasise() {
            List<Color> result = new List<Color>();
            if (getFlag(FLAGS.EMPHASISE_RED) == 1) {
                result.Add(Color.Red);
            }
            if (getFlag(FLAGS.EMPHASISE_BLUE) == 1) {
                result.Add(Color.Blue);
            }
            if (getFlag(FLAGS.EMPHASISE_GREEN) == 1) {
                result.Add(Color.Green);
            }
            return result;
        }

        public void update(byte data) {
            register = data;
        }
    }

    public class ScrollRegister
    {
        public byte _scrollX;
        public byte _scrollY;
        bool _latch;

        public ScrollRegister() {
            _scrollX = 0;
            _scrollY = 0;
            _latch = false;
        }

        public void write(byte data) {
            if (!_latch) 
                _scrollX = data;
            else
                _scrollY = data;
                
            _latch = !_latch;
        } 

        public void resetLatch() {
            _latch = false;
        }
    }

    public class StatusRegister
    {
        enum FLAGS {
            SPRITE_OVERFLOW = (1 << 5), 
            SPRITE_ZERO_HIT = (1 << 6), 
            VBLANK_STARTED = (1 << 7), 
        };

        byte register;

        void setFlag(FLAGS flag, bool v) {
            if (v)
                register |= (byte) flag;
            else
                register &= unchecked((byte) ~flag);
        }

        byte getFlag(FLAGS flag) {
            return (byte) ((register & (byte) flag) == 0 ? 0 : 1);
        }

        public void setVblankStatus(bool status) {
            setFlag(FLAGS.VBLANK_STARTED, status);
        }

        public void setSpriteZeroHit(bool status) {
            setFlag(FLAGS.SPRITE_ZERO_HIT, status);
        }

        public void setSpriteOverflow(bool status) {
            setFlag(FLAGS.SPRITE_OVERFLOW, status);
        }

        public void resetVblankStatus() {
            setFlag(FLAGS.VBLANK_STARTED, false);
        }

        public bool inVblank() {
            return getFlag(FLAGS.VBLANK_STARTED) == 1;
        }

        public byte snapshot() {
            return register;
        }
    }

    public class Utility {
        public static byte[] fillArray(byte value, int length) {
            byte[] array = new byte[length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = value;
            }
            return array;
        }
    }
}