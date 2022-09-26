namespace NESEmu
{
    class CPU
    {
        //Registers
        ushort PC;  //Program counter (16-bit)
        byte S;     //Stack pointer
        byte A;     //Accumulator
        byte X;     //Index register X
        byte Y;     //Index register Y
        byte status;//Processor status (flags)
        //Flags

        enum FLAGS {
            N = (1 << 0),  //Negative flag
            V = (1 << 1),  //Overflow flag
            U = (1 << 2),  //Unused flag
            B = (1 << 3),  //Break command
            D = (1 << 4),  //Decimal Mode flag
            I = (1 << 5),  //Interrupt disable
            Z = (1 << 6),  //Zero flag
            C = (1 << 7)   //Carry flag
        };
        
        delegate byte OperationDel();
        delegate byte AddressModeDel();

        struct Instruction {
            public string name;
            public byte cycles;
            public OperationDel operation;
            public AddressModeDel addressMode;
            public Instruction(string name, OperationDel operation, AddressModeDel addressMode, byte cycles) {
                this.name = name;
                this.operation = operation;
                this.addressMode = addressMode;
                this.cycles = cycles;
            }
        }
        Instruction[] _instructions;

        public CPU() {
            _instructions = new Instruction[256] {
                //0,                                 1,                                   2,                                   3,                                   4,                                   5,                                   6,                                   7,                                   8,                                   9,                                   a,                                   b,                                   c,                                   d,                                   e,                                   f
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 0
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 1
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 2
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 3
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 4
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 5
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 6
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 7
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 8
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 9
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // a
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // b
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // c
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // d
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // e
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7)  // f
            };
        }

        public void reset() {
            A = 0;
            status = 0x00 | (byte) FLAGS.U;
            PC = 0;
        }

        public void step() {
            reset();
            Console.WriteLine(status);
            byte opcode = 0x00;

            DissasembleCPU(opcode);
            _instructions[opcode].operation();
        }

        byte ADC() {
            XXX();
            return 0;
        }

        byte BRK() {
            XXX();
            return 0;
        }

        byte XXX() {
            throw new Exception("Unimplemented instruction");
        }

        void DissasembleCPU(byte opcode) {
            Console.WriteLine(_instructions[opcode].name);
        }
    }
}