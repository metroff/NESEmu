namespace NESEmu.Tests;

public class TestTAY
{
    Rom rom = TestRom.testRom();

    //TAY 0xa8
    [Fact]
    public void test_0xa8_tay_move_a_to_y()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x0a, 0xa8, 0x00});
        Assert.Equal(0x0A, cpu.register_y);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xa8_tay_zero_flag(){
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0xa8, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0xa8_tay_negative_flag() {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xff, 0xa8, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}