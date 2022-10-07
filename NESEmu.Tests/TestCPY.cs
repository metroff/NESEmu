namespace NESEmu.Tests;

public class TestCPY
{
    //CPY 0xc0
    [Fact]
    public void test_0xc0_cpy_carry_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xa9, 0x25, 0xa8, 0xc0, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xc0_cpy_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xa9, 0x10, 0xa8, 0xc0, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0xc0_cpy_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xa9, 0x20, 0xa8, 0xc0, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}