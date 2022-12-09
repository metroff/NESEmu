namespace NESEmu.Tests;

public class TestJMP
{
    Rom rom = TestRom.testRom();

    //JMP 0x4c
    [Fact]
    public void test_0x4c_jmp_absolute()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x4c, 0x25, 0x01, 0x00});
        Assert.Equal(0x0125, cpu.PC);
    }
}