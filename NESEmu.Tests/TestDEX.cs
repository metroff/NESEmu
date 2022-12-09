namespace NESEmu.Tests;

public class TestDEX
{
    Rom rom = TestRom.testRom();

    //DEX 0xca
    [Fact]
    public void test_0xca_dex_zero_page()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x20, 0xaa, 0xca, 0x00});
        Assert.Equal(0x1f, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xca_dex_negative_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0xaa, 0xca, 0x00});
        Assert.Equal(0xff, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0xca_dex_zero_flag()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x01, 0xaa, 0xca, 0x00});
        Assert.Equal(0x00, cpu.register_x);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}