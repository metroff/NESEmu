namespace NESEmu.Tests;

public class TestPHP
{
    Rom rom = TestRom.testRom();

    //PHP 0x08
    [Fact]
    public void test_0x08_php_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x08, 0x00});
        Assert.Equal(0x34, bus.memoryRead(0x01fd));
    }
}