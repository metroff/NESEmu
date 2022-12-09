namespace NESEmu.Tests;

public class TestBMI
{
    Rom rom = TestRom.testRom();

    //BMI 0x30
    [Fact]
    public void test_0x30_bmi_negative_clear()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x50, 0x30, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x60, cpu.register_x);
    }

    [Fact]
    public void test_0x30_bmi_negative_set()
    {
        Bus bus = new Bus(rom);
        CPU cpu = new CPU(bus);
        cpu.interpret(new byte[] {0xa9, 0x80, 0x30, 0x02, 0xa9, 0x60, 0xaa, 0x00});
        Assert.Equal(0x80, cpu.register_x);
    }
}