namespace NESEmu
{
    public class CPU
    {
        Console console;

        //Registers
        public ushort PC;  //Program counter (16-bit)
        public byte SP;     //Stack pointer
        public byte A;     //Accumulator
        public byte X;     //Index register X
        public byte Y;     //Index register Y
        public byte status;//Processor status (flags)
        
        //Helper
        ushort _address;
        byte _op;

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

        public CPU(Console console) {
            _instructions = new Instruction[256] {
                //0,                                 1,                                   2,                                   3,                                   4,                                   5,                                   6,                                   7,                                   8,                                   9,                                   a,                                   b,                                   c,                                   d,                                   e,                                   f
                new Instruction("BRK", BRK, IMP, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 0
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 1
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 2
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 3
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 4
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 5
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 6
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 7
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 8
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 9
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, IMM, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // a
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // b
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // c
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // d
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // e
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7)  // f
            };

            this.console = console;
        }

        byte read(ushort addr) {
            return console.cpuRead(addr);
        }

        public void reset() {
            A = 0;
            status = 0x00 | (byte) FLAGS.U;
            PC = 0;
        }

        public void clock() {
            byte opcode = read(PC);
            _op = opcode;
            PC++;

            byte cycles = _instructions[opcode].cycles;

            cycles += _instructions[opcode].addressMode();

            cycles += _instructions[opcode].operation();
        }

        public void interpret() {
            System.Console.WriteLine("called");
            PC = 0;
            while(true) {
                if (read(PC) == 0)
                    break;

                clock();
                System.Console.WriteLine(A);
            }

            System.Console.WriteLine(A);
        }

        // Addressing modes
        byte IMM() {
            _address = PC;
            PC++;
            return 0;
        }

        byte IMP() {
            return 0;
        }
        // Operations

        byte ADC() {
            XXX();
            return 0;
        }

        byte BRK() {
            status |= (byte) FLAGS.B;
            return 0;
        }

        byte LDA() {
            A = read(_address);

            if (A == 0)
                status |= (byte) FLAGS.Z;
            else
                status &= unchecked((byte) ~FLAGS.Z);

            if ((A & 0x80) == 0x80)
                status |= (byte) FLAGS.N;
            else
                status &= unchecked((byte) ~FLAGS.N);
            
            return 0;
        }

        byte XXX() {
            throw new Exception("Unimplemented instruction " + _op.ToString("x") + _instructions[_op].name);
        }

        void DissasembleCPU(byte opcode) {
            System.Console.WriteLine(_instructions[opcode].name);
        }
    }
}