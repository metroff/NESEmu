namespace NESEmu.Tests;

public class TestROR
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //ROL 0x6a
    [Fact]
    public void test_0x6a_rol_implied()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x04, 0x6a, 0x00});
        Assert.Equal(0x02, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x6a_rol_carry_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x05, 0x6a, 0x00});
        Assert.Equal(0x02, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x6a_rol_zero_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x01, 0x6a, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //ROL 0X66
    [Fact]
    public void test_0X66_rol_zero_page()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x04);
        cpu.interpret(new byte[] {0X66, 0x10, 0x00});
        Assert.Equal(0x02, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0X66_rol_carry_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x05);
        cpu.interpret(new byte[] {0X66, 0x10, 0x00});
        Assert.Equal(0x02, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0X66_rol_zero_flag()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        bus.memoryWrite(0x0010, 0x01);
        cpu.interpret(new byte[] {0X66, 0x10, 0x00});
        Assert.Equal(0x00, bus.memoryRead(0x0010));
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}