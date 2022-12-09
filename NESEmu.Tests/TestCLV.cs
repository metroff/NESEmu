namespace NESEmu.Tests;

public class TestCLV
{
    Rom rom = TestRom.testRom();
    
    //CLV 0xb8
    [Fact]
    public void test_0xb8_clv_implied()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0xBE, 0x69, 0xBE, 0x78, 0xb8, 0x00});
        Assert.True((cpu.status & (byte)CPU.FLAGS.V) == 0);
    }
}