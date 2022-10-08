namespace NESEmu.Tests;

public class TestTSX
{
    //TSX 0xba
    [Fact]
    public void test_0xba_tsx_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x69, 0x9a, 0xa2, 0x21, 0xba, 0x00});
        Assert.Equal(0x69, cpu.register_x);
    }

    [Fact]
    public void test_0xba_tsx_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x00, 0x9a, 0xa2, 0x21, 0xba, 0x00});
        Assert.Equal(0x00, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0xba_tsx_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x80, 0x9a, 0xa2, 0x21, 0xba, 0x00});
        Assert.Equal(0x80, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}