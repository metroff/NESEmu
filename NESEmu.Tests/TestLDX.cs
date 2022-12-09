namespace NESEmu.Tests;

public class TestLDX
{
    Rom rom = TestRom.testRom();

    //LDX 0xa2
    [Fact]
    public void test_0xa2_ldx_immediate_load_data()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x05, 0x00});
        Assert.Equal(0x05, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xa2_ldx_zero_flag(){
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x00, 0x00});
        Assert.Equal(0x00, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0xa2_ldx_negative_flag() {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0xff, 0x00});
        Assert.Equal(0xff, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}