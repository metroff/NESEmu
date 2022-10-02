namespace NESEmu.Tests;

public class TestOpcodeCombination
{
    [Fact]
    public void test_5op_combination()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xc0, 0xaa, 0xe8, 0x00});
        Assert.Equal(0xc1, cpu.register_x);
    }
}