namespace NESEmu.Tests;

public class TestBIT
{
    //BIT 0x24
    [Fact]
    public void test_0x24_bit_zero_page()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x2d);
        cpu.interpret(new byte[] {0xa9, 0x5a, 0x24, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x24_bit_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x00);
        cpu.interpret(new byte[] {0xa9, 0x5a, 0x24, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x24_bit_overflow_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x40);
        cpu.interpret(new byte[] {0xa9, 0x5a, 0x24, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == (byte)CPU.FLAGS.V);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x24_bit_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x80);
        cpu.interpret(new byte[] {0xa9, 0x8a, 0x24, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}