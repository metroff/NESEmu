namespace NESEmu.Tests;

public class TestPHP
{
    //PHP 0x08
    [Fact]
    public void test_0x08_php_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x08, 0x00});
        Assert.Equal(0x4, bus.memoryRead(0x01fd));
    }
}