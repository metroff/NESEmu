namespace NESEmu.Tests;

public class TestCLD
{
    //CLD 0xd8
    [Fact]
    public void test_0xd8_cld_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xf8, 0xd8, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.D) == 0);
    }
}