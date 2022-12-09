namespace NESEmu.Tests;

public class TestBVS
{
    Rom rom = TestRom.testRom();

    //BVS 0x70
    [Fact]
    public void test_0x70_bvs_overflow_clear()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0x70, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x60, cpu.register_x);
    }

    [Fact]
    public void test_0x70_bvs_overflow_set()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x20, 0x69, 0x60, 0xa9, 0x80, 0x70, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x80, cpu.register_x);
    }
}