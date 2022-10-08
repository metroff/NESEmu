namespace NESEmu.Tests;

public class TestTXS
{
    //TXS 0x9a
    [Fact]
    public void test_0x9a_txs_implied()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa2, 0x69, 0x9a, 0x00});
        Assert.Equal(0x69, bus.memoryRead(0x01ff));
    }
}