namespace NESEmu.Tests;

public class TestTYA
{
    Rom rom = TestRom.testRom();

    //TYA 0x98
    [Fact]
    public void test_0x98_tya_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa0, 0x69, 0x98, 0x00});
        Assert.Equal(0x69, cpu.register_a);
    }

    [Fact]
    public void test_0x98_tya_zero_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa0, 0x00, 0x98, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0x98_tya_negative_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa0, 0x80, 0x9a, 0x98, 0x00});
        Assert.Equal(0x80, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}