namespace NESEmu.Tests;

public class TestINC
{
    //INC 0xe6
    [Fact]
    public void test_0xe6_inc_zero_page()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xe6, 0x20, 0x00});
        Assert.Equal(0x21, bus.memoryRead(0x0020));
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xe6_inc_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x7f);
        cpu.interpret(new byte[] {0xe6, 0x20, 0x00});
        Assert.Equal(0x80, bus.memoryRead(0x0020));
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0xe6_inc_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0xff);
        cpu.interpret(new byte[] {0xe6, 0x20, 0x00});
        Assert.Equal(0x00, bus.memoryRead(0x0020));
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}