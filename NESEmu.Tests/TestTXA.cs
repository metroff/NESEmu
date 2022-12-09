namespace NESEmu.Tests;

public class TestTXA
{
    Rom rom = TestRom.testRom();

    //TXA 0x8a
    [Fact]
    public void test_0x8a_txa_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x69, 0x8a, 0x00});
        Assert.Equal(0x69, cpu.register_a);
    }

    [Fact]
    public void test_0x8a_txa_zero_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x00, 0x8a, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0x8a_txa_negative_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x80, 0x9a, 0x8a, 0x00});
        Assert.Equal(0x80, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}