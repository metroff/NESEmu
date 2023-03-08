namespace NESEmu.Tests;

public class TestSTY
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //STY 0x84
    [Fact]
    public void test_0x84_sty_zero_page()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa0, 0x96, 0x84, 0x20, 0x00});
        Assert.Equal(0x96, bus.memoryRead(0x0020));
    }
}