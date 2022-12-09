namespace NESEmu.Tests;

public class TestJSR
{
    Rom rom = TestRom.testRom();

    //JSR 0x20
    [Fact]
    public void test_0x20_jsr_absolute()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x20, 0x02, 0x01, 0x00});
        Assert.Equal(0x0102, cpu.PC);
        Assert.Equal(0x06, bus.memoryRead(0x01fd));
        Assert.Equal(0x02, bus.memoryRead(0x01fc));
    }
}