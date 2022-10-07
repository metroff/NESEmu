namespace NESEmu.Tests;

public class TestDEC
{
    //DEC 0xc6
    [Fact]
    public void test_0xc6_dec_zero_page()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xc6, 0x20, 0x00});
        Assert.Equal(0x1f, bus.memoryRead(0x0020));
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xc6_dec_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x00);
        cpu.interpret(new byte[] {0xc6, 0x20, 0x00});
        Assert.Equal(0xff, bus.memoryRead(0x0020));
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0xc6_dec_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x01);
        cpu.interpret(new byte[] {0xc6, 0x20, 0x00});
        Assert.Equal(0x00, bus.memoryRead(0x0020));
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}