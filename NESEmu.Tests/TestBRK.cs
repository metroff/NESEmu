namespace NESEmu.Tests;

public class TestBRK
{
    //BRK 0x00
    [Fact]
    public void test_0x00_brk_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.B) == (byte)CPU.FLAGS.B);
        Assert.Equal(6, bus.memoryRead(0x01fd));
        Assert.Equal(2, bus.memoryRead(0x01fc));
    }
}