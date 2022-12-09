namespace NESEmu.Tests;

public class TestLDA
{
    Rom rom = TestRom.testRom();

    //LDA 0xa9
    [Fact]
    public void test_0xa9_lda_immediate_load_data()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x05, 0x00});
        Assert.Equal(0x05, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    [Fact]
    public void test_0xa9_lda_zero_flag(){
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == (byte)CPU.FLAGS.Z);
    }

    [Fact]
    public void test_0xa9_lda_negative_flag() {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xff, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == (byte)CPU.FLAGS.N);
    }

    //LDA 0xa5
    [Fact]
    public void test_0xa5_lda_zero_page_load_data()
    {
        Bus bus = new Bus(rom);
        bus.memoryWrite(0x0010, 0x55);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa5, 0x10, 0x00});
        Assert.Equal(0x55, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //LDA 0xb5
    [Fact]
    public void test_0xb5_lda_zero_page_indexed_load_data()
    {
        Bus bus = new Bus(rom);
        bus.memoryWrite(0x0010, 0x55);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x11, 0xaa, 0xb5, 0xff, 0x00});
        Assert.Equal(0x55, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //LDA 0xad
    [Fact]
    public void test_0xad_lda_absolute_load_data()
    {
        Bus bus = new Bus(rom);
        bus.memoryWrite(0x0410, 0x55);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xad, 0x10, 0x04, 0x00});
        Assert.Equal(0x55, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //LDA 0xbd
    [Fact]
    public void test_0xbd_lda_absolute_indexed_load_data()
    {
        Bus bus = new Bus(rom);
        bus.memoryWrite(0x0410, 0x55);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x11, 0xaa, 0xbd, 0xff, 0x03, 0x00});
        Assert.Equal(0x55, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //LDA 0xb9
    [Fact]
    public void test_0xb9_lda_indexed_absolute_load_data()
    {
        Bus bus = new Bus(rom);
        bus.memoryWrite(0x0410, 0x55);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x11, 0xa8, 0xb9, 0xff, 0x03, 0x00});
        Assert.Equal(0x55, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //LDA 0xa1
    [Fact]
    public void test_0xa1_lda_indirect_indexed_load_data()
    {
        Bus bus = new Bus(rom);
        bus.memoryWrite(0x0030, 0x55);
        bus.memoryWrite(0x0010, 0x10);
        bus.memoryWrite(0x0020, 0x30);
        bus.memoryWrite(0x0021, 0x00);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x10, 0xaa, 0xa1, 0x10, 0x00});
        Assert.Equal(0x55, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }

    //LDA 0xb1
    [Fact]
    public void test_0xb1_lda_indexed_indirect_load_data()
    {
        Bus bus = new Bus(rom);
        bus.memoryWrite(0x0038, 0x55);
        bus.memoryWrite(0x0020, 0x30);
        bus.memoryWrite(0x0021, 0x00);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x08, 0xa8, 0xb1, 0x20, 0x00});
        Assert.Equal(0x55, cpu.register_a);
        Assert.True((cpu.status & (byte)CPU.FLAGS.Z) == 0b00);
        Assert.True((cpu.status & (byte)CPU.FLAGS.N) == 0);
    }
}