namespace NESEmu.Tests;

public class TestPHA
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //PHA 0x48
    [Fact]
    public void test_0x48_pha_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x25, 0x48, 0x00});
        Assert.Equal(0x25, bus.memoryRead(0x01fd));
    }
}