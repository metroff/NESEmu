namespace NESEmu.Tests;

public class TestBNE
{
    //BNE 0xd0
    [Fact]
    public void test_0xd0_bne_zero_clear()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0xd0, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x50, cpu.register_x);
    }

    [Fact]
    public void test_0xd0_bne_zero_set()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x00, 0xd0, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x60, cpu.register_x);
    }
}