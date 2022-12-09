namespace NESEmu.Tests;

public class TestLSR
{
    Rom rom = TestRom.testRom();

    //LSR 0x4a
    [Fact]
    public void test_0x4a_lsr_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0x4a, 0x00});
        Assert.Equal(0x28, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x4a_lsr_carry_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x89, 0x4a, 0x00});
        Assert.Equal(0x44, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x4a_lsr_zero_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x01, 0x4a, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //LSR 0x46
    [Fact]
    public void test_0x46_lsr_zero_page()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x50);
        cpu.interpret(new byte[] {0x46, 0x10, 0x00});
        Assert.Equal(0x28, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x46_lsr_carry_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x89);
        cpu.interpret(new byte[] {0x46, 0x10, 0x00});
        Assert.Equal(0x44, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x46_lsr_zero_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x01);
        cpu.interpret(new byte[] {0x46, 0x10, 0x00});
        Assert.Equal(0x00, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}