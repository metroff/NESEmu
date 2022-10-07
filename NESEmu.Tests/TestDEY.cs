namespace NESEmu.Tests;

public class TestDEY
{
    //DEY 0x88
    [Fact]
    public void test_0x88_dey_zero_page()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x20, 0xa8, 0x88, 0x00});
        Assert.Equal(0x1f, cpu.register_y);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x88_dey_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0xa8, 0x88, 0x00});
        Assert.Equal(0xff, cpu.register_y);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0x88_dey_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x01, 0xa8, 0x88, 0x00});
        Assert.Equal(0x00, cpu.register_y);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}