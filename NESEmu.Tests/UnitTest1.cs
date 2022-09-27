namespace NESEmu.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        NESEmu.Console console = new NESEmu.Console();
        CPU cpu = new CPU(console);
        cpu.reset();
        console.load(new byte[] {0xa9, 0x05, 0x00});
        cpu.interpret();
        Assert.Equal(0x05, cpu.A);
        Assert.True((cpu.status & 0b0000_0010) == 0b00);
        Assert.True((cpu.status & 0b1000_0000) == 0);
    }
}