namespace NESEmu.Tests;

public class TestADC
{
    //ADC 0x69
    [Fact]
    public void test_0x69_adc_immediate_add()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x20, 0x69, 0x30, 0x00});
        Assert.Equal(0x50, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    
    public void test_0x69_adc_negative_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x20, 0x69, 0x60, 0x00});
        Assert.Equal(0x80, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == (byte)CPU.FLAGS.V);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0x69_adc_carry_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xff, 0x69, 0xf6, 0x00});
        Assert.Equal(0xf5, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    [Fact]
    public void test_0x69_adc_overflow_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xBE, 0x69, 0xBE, 0x00});
        Assert.Equal(0x7C, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == (byte)CPU.FLAGS.V);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0x69_adc_zero_flag()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xFB, 0x69, 0x05, 0x00});
        Assert.Equal(0x00, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.C) == (byte)CPU.FLAGS.C);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == 0);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}