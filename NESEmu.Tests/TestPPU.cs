namespace NESEmu.Tests
{
    public class TestPPU
    {
        [Fact]
        public void test_ppu_vram_writes() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.addrWrite(0x23);
            ppu.addrWrite(0x05);
            ppu.writeData(0x66);

            Assert.Equal(0x66, ppu.vram[0x0305]);
        }

        [Fact]
        public void test_ppu_vram_reads() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.ctrlWrite(0);
            ppu.vram[0x0305] = 0x66;

            ppu.addrWrite(0x23);
            ppu.addrWrite(0x05);

            ppu.readData(); //load_into_buffer
            Assert.Equal(0x2306, ppu._addr.get());
            Assert.Equal(0x66, ppu.readData());
        }

        [Fact]
        public void test_ppu_vram_reads_cross_page() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.ctrlWrite(0);
            ppu.vram[0x01ff] = 0x66;
            ppu.vram[0x0200] = 0x77;

            ppu.addrWrite(0x21);
            ppu.addrWrite(0xff);

            ppu.readData(); //load_into_buffer
            Assert.Equal(0x66, ppu.readData());
            Assert.Equal(0x77, ppu.readData());
        }

        [Fact]
        public void test_ppu_vram_reads_step_32() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.ctrlWrite(0b100);
            ppu.vram[0x01ff] = 0x66;
            ppu.vram[0x01ff + 32] = 0x77;
            ppu.vram[0x01ff + 64] = 0x88;

            ppu.addrWrite(0x21);
            ppu.addrWrite(0xff);

            ppu.readData(); //load_into_buffer
            Assert.Equal(0x66, ppu.readData());
            Assert.Equal(0x77, ppu.readData());
            Assert.Equal(0x88, ppu.readData());
        }

        [Fact]
        public void test_vram_horizontal_mirror() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.addrWrite(0x24);
            ppu.addrWrite(0x05);

            ppu.writeData(0x66); //write to a

            ppu.addrWrite(0x28);
            ppu.addrWrite(0x05);

            ppu.writeData(0x77); //write to B

            ppu.addrWrite(0x20);
            ppu.addrWrite(0x05);

            ppu.readData(); //load into buffer
            Assert.Equal(0x66, ppu.readData()); //read from A

            ppu.addrWrite(0x2C);
            ppu.addrWrite(0x05);

            ppu.readData(); //load into buffer
            Assert.Equal(0x77, ppu.readData()); //read from b
        }

        // Vertical: https://wiki.nesdev.com/w/index.php/Mirroring
        //   [0x2000 A ] [0x2400 B ]
        //   [0x2800 a ] [0x2C00 b ]
        [Fact]
        public void test_vram_vertical_mirror() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.VERTICAL);

            ppu.addrWrite(0x20);
            ppu.addrWrite(0x05);

            ppu.writeData(0x66); //write to A

            ppu.addrWrite(0x2C);
            ppu.addrWrite(0x05);

            ppu.writeData(0x77); //write to b

            ppu.addrWrite(0x28);
            ppu.addrWrite(0x05);

            ppu.readData(); //load into buffer
            Assert.Equal(0x66, ppu.readData()); //read from a

            ppu.addrWrite(0x24);
            ppu.addrWrite(0x05);

            ppu.readData(); //load into buffer
            Assert.Equal(0x77, ppu.readData()); //read from B
        }

        [Fact]
        public void test_read_status_resets_latch() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.vram[0x0305] = 0x66;

            ppu.addrWrite(0x21);
            ppu.addrWrite(0x23);
            ppu.addrWrite(0x05);

            ppu.readData(); //load_into_buffer
            Assert.NotEqual(0x66, ppu.readData());

            ppu.readStatus();

            ppu.addrWrite(0x23);
            ppu.addrWrite(0x05);

            ppu.readData(); //load_into_buffer
            Assert.Equal(0x66, ppu.readData());
        }

        [Fact]
        public void test_ppu_vram_mirroring() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.ctrlWrite(0);
            ppu.vram[0x0305] = 0x66;

            ppu.addrWrite(0x63); //0x6305 -> 0x2305
            ppu.addrWrite(0x05);

            ppu.readData(); //load into_buffer
            Assert.Equal(0x66, ppu.readData());
        }

        [Fact]
        public void test_read_status_resets_vblank() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu._status.setVblankStatus(true);

            byte status = ppu.readStatus();

            Assert.Equal(1, (status >> 7));
            Assert.Equal(0, ppu._status.snapshot() >> 7);
        }

        [Fact]
        public void test_oam_read_write() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);
            ppu.writeOamAddress(0x10);
            ppu.writeOamData(0x66);
            ppu.writeOamData(0x77);

            ppu.writeOamAddress(0x10);
            Assert.Equal(0x66, ppu.readOamData());

            ppu.writeOamAddress(0x11);
            Assert.Equal(0x77, ppu.readOamData());
        }

        [Fact]
        public void test_oam_dma() {
            PPU ppu = new PPU(new List<byte>(Utility.fillArray(0, 2048)), Mirroring.HORIZONTAL);

            byte[] data = Utility.fillArray(0x66, 256);
            data[0] = 0x77;
            data[255] = 0x88;

            ppu.writeOamAddress(0x10);
            ppu.writeOamDma(data);

            ppu.writeOamAddress(0xf); //wrap around
            Assert.Equal(0x88, ppu.readOamData());

            ppu.writeOamAddress(0x10);
            ppu.writeOamAddress(0x77);
            ppu.writeOamAddress(0x11);
            ppu.writeOamAddress(0x66);
        }
    }
}