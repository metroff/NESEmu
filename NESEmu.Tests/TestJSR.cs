namespace NESEmu.Tests;

public class TestJSR
{
    //JSR 0x20
    [Fact]
    public void test_0x20_jsr_absolute()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x20, 0x02, 0xa9, 0x00});
        Assert.Equal(0xa902, cpu.PC);
        Assert.Equal(0x06, bus.memoryRead(0x01fd));
        Assert.Equal(0x02, bus.memoryRead(0x01fc));
    }
}