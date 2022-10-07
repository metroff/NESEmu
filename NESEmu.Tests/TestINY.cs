namespace NESEmu.Tests;

public class TestINY
{
    //INY 0xc8
    [Fact]
    public void test_0xc8_iny_increase_x()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x0a, 0xa8, 0xc8, 0x00});
        Assert.Equal(0x0B, cpu.register_y);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xc8_iny_zero_flag(){
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xff, 0xa8, 0xc8, 0x00});
        Assert.Equal(0x00, cpu.register_y);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0xc8_iny_negative_flag() {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xfe, 0xa8, 0xc8, 0x00});
        Assert.Equal(0xff, cpu.register_y);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}