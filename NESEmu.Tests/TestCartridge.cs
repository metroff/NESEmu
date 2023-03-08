namespace NESEmu.Tests;

public class TestRom {
    public static byte[] createRom(byte[] header, byte[] trainer, byte[] prgRom, byte[] chrRom) {
        List<byte> list = new List<byte>(header.Length + trainer.Length + prgRom.Length + chrRom.Length);
        list.AddRange(header);
        list.AddRange(trainer);
        list.AddRange(prgRom);
        list.AddRange(chrRom);
        return list.ToArray();
    }

    public static Rom testRom() {
        byte[] header = new byte[] {0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0x31, 00, 00, 00, 00, 00, 00, 00, 00, 00};
        byte[] trainer = new byte[0];
        byte[] prgRom = fillArray(1, 2*16384);
        byte[] chrRom = fillArray(2, 8192);

        Rom rom = new Rom(createRom(header, trainer, prgRom, chrRom));
        return rom;
    }

    public static byte[] fillArray(byte value, int length) {
        byte[] array = new byte[length];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = value;
        }
        return array;
    }
}

public class TestCartridge
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //BVS 0x70
    [Fact]
    public void test_cartridge()
    {
        byte[] header = new byte[] {0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0x31, 00, 00, 00, 00, 00, 00, 00, 00, 00};
        byte[] trainer = new byte[0];
        byte[] prgRom = TestRom.fillArray(1, 2*16384);
        byte[] chrRom = TestRom.fillArray(2, 8192);

        Rom rom = new Rom(TestRom.createRom(header, trainer, prgRom, chrRom));

        Assert.Equal(TestRom.fillArray(2, 8192), rom.chrRom);
        Assert.Equal(TestRom.fillArray(1, 2*16384), rom.prgRom);
        Assert.Equal(3, rom.mapper);
        Assert.Equal(Mirroring.VERTICAL, rom.screenMirroring);
    }

    [Fact]
    public void test_cartridge_with_trainer()
    {
        byte[] header = new byte[] {0x4E, 0x45, 0x53, 0x1A, 0x02, 0x01, 0x31 | 0b100, 00, 00, 00, 00, 00, 00, 00, 00, 00};
        byte[] trainer = TestRom.fillArray(0, 512);
        byte[] prgRom = TestRom.fillArray(1, 2*16384);
        byte[] chrRom = TestRom.fillArray(2, 8192);

        Rom rom = new Rom(TestRom.createRom(header, trainer, prgRom, chrRom));

        Assert.Equal(TestRom.fillArray(2, 8192), rom.chrRom);
        Assert.Equal(TestRom.fillArray(1, 2*16384), rom.prgRom);
        Assert.Equal(3, rom.mapper);
        Assert.Equal(Mirroring.VERTICAL, rom.screenMirroring);
    }

    [Fact]
    public void test_cartridge_nes2_not_supported()
    {
        byte[] header = new byte[] {0x4E, 0x45, 0x53, 0x1A, 0x01, 0x01, 0x31, 0x8, 00, 00, 00, 00, 00, 00, 00, 00};
        byte[] trainer = new byte[0];
        byte[] prgRom = TestRom.fillArray(1, 16384);
        byte[] chrRom = TestRom.fillArray(2, 8192);

        try {
            Rom rom = new Rom(TestRom.createRom(header, trainer, prgRom, chrRom));
        } catch (Exception e) {
            Assert.Equal("NES 2.0 format not supported." ,e.Message);
        }
        
    }
}