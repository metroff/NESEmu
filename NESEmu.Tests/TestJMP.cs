namespace NESEmu.Tests;

public class TestJMP
{
    //JMP 0x4c
    [Fact]
    public void test_0x4c_jmp_absolute()
    {
        Bus bus = new Bus();
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0x4c, 0x25, 0xa8, 0x00});
        Assert.Equal(0xa825, cpu.PC);
    }
}