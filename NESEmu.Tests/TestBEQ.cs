namespace NESEmu.Tests;

public class TestBEQ
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //BEQ 0xf0
    [Fact]
    public void test_0xf0_beq_zero_clear()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0xf0, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x60, cpu.register_x);
    }

    [Fact]
    public void test_0xf0_beq_zero_set()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0xf0, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x00, cpu.register_x);
    }
}