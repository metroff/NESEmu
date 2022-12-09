namespace NESEmu.Tests;

public class TestBPL
{
    Rom rom = TestRom.testRom();

    //BPL 0x10
    [Fact]
    public void test_0x10_bpl_negative_clear()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0x10, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x50, cpu.register_x);
    }

    [Fact]
    public void test_0x10_bpl_negative_set()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x80, 0x10, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x60, cpu.register_x);
    }
}