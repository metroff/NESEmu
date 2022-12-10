namespace NESEmu.Tests;

public class TestCLI
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //CLI 0x58
    [Fact]
    public void test_0x58_cli_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x78, 0x58, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.I) == 0);
    }
}