namespace NESEmu.Tests;

public class TestCPX
{
    Rom rom = TestRom.testRom();

    //CPX 0xe0
    [Fact]
    public void test_0xe0_cpx_carry_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xa9, 0x25, 0xaa, 0xe0, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xe0_cpx_negative_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xa9, 0x10, 0xaa, 0xe0, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0xe0_cpx_zero_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0020, 0x20);
        cpu.interpret(new byte[] {0xa9, 0x20, 0xaa, 0xe0, 0x20, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}