namespace NESEmu.Tests;

public class TestASL
{
    Rom rom = TestRom.testRom();
    Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //ASL 0x0a
    [Fact]
    public void test_0x0a_asl_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x05, 0x0a, 0x00});
        Assert.Equal(0xa, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x0a_asl_carry_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x89, 0x0a, 0x00});
        Assert.Equal(0x12, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x0a_asl_zero_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x80, 0x0a, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x0a_asl_negative_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x7a, 0x0a, 0x00});
        Assert.Equal(0xf4, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    //ASL 0x06
    [Fact]
    public void test_0x06_asl_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x05);
        cpu.interpret(new byte[] {0x06, 0x10, 0x00});
        Assert.Equal(0xa, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]    
    public void test_0x06_asl_carry_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x89);
        cpu.interpret(new byte[] {0x06, 0x10, 0x00});
        Assert.Equal(0x12, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x06_asl_zero_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x80);
        cpu.interpret(new byte[] {0x06, 0x10, 0x00});
        Assert.Equal(0x00, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x06_asl_negative_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x7a);
        cpu.interpret(new byte[] {0x06, 0x10, 0x00});
        Assert.Equal(0xf4, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}