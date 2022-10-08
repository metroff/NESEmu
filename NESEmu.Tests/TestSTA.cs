namespace NESEmu.Tests;

public class TestSTA
{
    //STA 0x85
    [Fact]
    public void test_0x85_sta_zero_page()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x69, 0x85, 0x20, 0x00});
        Assert.Equal(0x69, bus.memoryRead(0x0020));
    }
}