namespace NESEmu.Tests;

public class TestBCC
{
    //bcc 0x90
    [Fact]
    public void test_0x90_bcc_carry_clear()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0x90, 0x02 , 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x50, cpu.register_a);
        Assert.Equal(0x50, cpu.register_x);
    }

    [Fact]
    public void test_0x90_bcc_carry_set()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x05, 0x6a, 0xa9, 0x50, 0x90, 0x02 , 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x60, cpu.register_a);
        Assert.Equal(0x60, cpu.register_x);
    }
}