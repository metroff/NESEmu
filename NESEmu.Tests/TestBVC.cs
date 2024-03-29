namespace NESEmu.Tests;

public class TestBVC
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //BVC 0x50
    [Fact]
    public void test_0x50_bvc_overflow_clear()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0x50, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x50, cpu.register_x);
    }

    [Fact]
    public void test_0x50_bvc_overflow_set()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x20, 0x69, 0x60, 0xa9, 0x80, 0x50, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x60, cpu.register_x);
    }
}