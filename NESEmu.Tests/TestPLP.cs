namespace NESEmu.Tests;

public class TestPLP
{
    Rom rom = TestRom.testRom();

    //PLP 0x28
    [Fact]
    public void test_0x28_plp_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x08, 0xa9, 0x80, 0x28, 0x00});
        Assert.Equal(0x24, cpu.status);
    }
}