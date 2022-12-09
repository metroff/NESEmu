namespace NESEmu.Tests;

public class TestSED
{
    Rom rom = TestRom.testRom();

    //SED 0xf8
    [Fact]
    public void test_0xf8_sed_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xf8, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.D) == (byte)CPU.FLAGS.D);
    }
}