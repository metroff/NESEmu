namespace NESEmu.Tests;

public class TestSEI
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //SEI 0x78
    [Fact]
    public void test_0x78_sei_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x78, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.I) == (byte)CPU.FLAGS.I);
    }
}