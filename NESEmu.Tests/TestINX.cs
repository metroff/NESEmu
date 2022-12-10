namespace NESEmu.Tests;

public class TestINX
{
    Rom rom = TestRom.testRom();
Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad){};

    //INX 0xe8
    [Fact]
    public void test_0xe8_inx_increase_x()
    {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x0a, 0xaa, 0xe8, 0x00});
        Assert.Equal(0x0B, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xe8_inx_zero_flag(){
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xff, 0xaa, 0xe8, 0x00});
        Assert.Equal(0x00, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0xe8_inx_negative_flag() {
        Bus bus = new Bus(rom, callback);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xfe, 0xaa, 0xe8, 0x00});
        Assert.Equal(0xff, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}