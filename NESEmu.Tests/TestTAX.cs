namespace NESEmu.Tests;

public class TestTAX
{
    Rom rom = TestRom.testRom();

    //TAX 0xaa
    [Fact]
    public void test_0xaa_tax_move_a_to_x()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x0a, 0xaa, 0x00});
        Assert.Equal(0x0A, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xaa_tax_zero_flag(){
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0xaa, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0xaa_tax_negative_flag() {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xff, 0xaa, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }
}