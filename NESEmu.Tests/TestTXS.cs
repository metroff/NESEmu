namespace NESEmu.Tests;

public class TestTXS
{
    Rom rom = TestRom.testRom();

    //TXS 0x9a
    [Fact]
    public void test_0x9a_txs_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x69, 0x9a, 0x00});
        Assert.Equal(0x69, cpu.SP);
    }
}