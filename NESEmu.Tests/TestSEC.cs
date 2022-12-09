namespace NESEmu.Tests;

public class TestSEC
{
    Rom rom = TestRom.testRom();

    //SEC 0x38
    [Fact]
    public void test_0x38_sec_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x38, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
    }
}