namespace NESEmu.Tests;

public class TestCLC
{
    //CLC 0x18
    [Fact]
    public void test_0x18_clc_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xff, 0x69, 0xf6, 0x18, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
    }
}