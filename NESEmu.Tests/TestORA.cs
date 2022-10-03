namespace NESEmu.Tests;

public class TestORA
{
    //ORA 0x09
    [Fact]
    public void test_0x09_ora_immediate()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x5a, 0x09, 0x71, 0x00});
        Assert.Equal(0x7b, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x09_ora_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xcd, 0x09, 0xad, 0x00});
        Assert.Equal(0xed, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0x09_ora_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0x09, 0x00, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}