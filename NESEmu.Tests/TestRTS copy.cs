namespace NESEmu.Tests;

public class TestRTI
{
    //RTI 0x40
    [Fact]
    public void test_0x40_rti_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x20, 0x04, 0x06, 0x00, 0x08, 0xa9, 0x80, 0x40});
        Assert.Equal(0x04, cpu.status);
    }
}