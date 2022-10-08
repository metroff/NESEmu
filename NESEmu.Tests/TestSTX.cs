namespace NESEmu.Tests;

public class TestSTX
{
    //STX 0x86
    [Fact]
    public void test_0x86_stx_zero_page()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x69, 0x86, 0x20, 0x00});
        Assert.Equal(0x69, bus.memoryRead(0x0020));
    }
}