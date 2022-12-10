namespace NESEmu.Tests;

public class TestROL
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //ROL 0x2a
    [Fact]
    public void test_0x2a_rol_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x5, 0x2a, 0x00});
        Assert.Equal(0x0a, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x2a_rol_carry_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x85, 0x2a, 0x00});
        Assert.Equal(0x0a, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x2a_rol_zero_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x80, 0x2a, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x2a_rol_negative_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x79, 0x2a, 0x00});
        Assert.Equal(0xf2, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    //ROL 0X26
    [Fact]
    public void test_0X26_rol_zero_page()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x05);
        cpu.interpret(new byte[] {0X26, 0x10, 0x00});
        Assert.Equal(0x0a, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0X26_rol_carry_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x85);
        cpu.interpret(new byte[] {0X26, 0x10, 0x00});
        Assert.Equal(0x0a, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0X26_rol_zero_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x80);
        cpu.interpret(new byte[] {0X26, 0x10, 0x00});
        Assert.Equal(0x00, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0X26_rol_negative_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x79);
        cpu.interpret(new byte[] {0X26, 0x10, 0x00});
        Assert.Equal(0xf2, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}