namespace NESEmu.Tests;

public class TestPLA
{
    //PLA 0x68
    [Fact]
    public void test_0x68_pla_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x25, 0x48, 0xa9, 0x0, 0x68, 0x00});
        Assert.Equal(0x25, cpu.register_a);
    }

    [Fact]
    public void test_0x68_pla_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x85, 0x48, 0xa9, 0x0, 0x68, 0x00});
        Assert.Equal(0x85, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0x68_pla_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0x48, 0xa9, 0x56, 0x68, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}