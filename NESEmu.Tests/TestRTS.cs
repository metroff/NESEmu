namespace NESEmu.Tests;

public class TestRTS
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //RTS 0x60
    [Fact]
    public void test_0x60_rts_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x20, 0x04, 0x06, 0x00, 0x60});
        Assert.Equal(0x0603, cpu.PC);
    }
}