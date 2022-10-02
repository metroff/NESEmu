namespace NESEmu
{
    public class CPU
    {
        Bus bus;

        //Registers
        public ushort PC;  //Program counter (16-bit)
        public byte SP;     //Stack pointer
        public byte register_a;     //Accumulator
        public byte register_x;     //Index register X
        public byte register_y;     //Index register Y
        public byte status;//Processor status (flags)
        
        //Helper
        ushort abs_address;
        ushort rel_address;
        byte _op;

        //Flags
        public enum FLAGS {
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

        public CPU(Bus bus) {
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
                new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ZP0, 3), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("TAY", TAY, IMP, 2), new Instruction("LDA", LDA, IMM, 2), new Instruction("TAX", TAX, IMP, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ABS, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // a
                new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ZPX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ABY, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ABX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // b
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // c
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // d
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("INX", INX, IMP, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // e
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7)  // f
            };

            this.bus = bus;
        }

        byte read(ushort addr) {
            return bus.memoryRead(addr);
        }

        public void reset() {
            register_a = 0;
            register_x = 0;
            register_y = 0;
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

        public void interpret(byte[] program) {
            for (int i = 0; i < program.Length; i++)
            {
                bus.memoryWrite((ushort)(0x0600 + i),program[i]);
            }

            reset();

            PC = 0x0600;

            while(true) {
                if (read(PC) == 0)
                    break;
                clock();
            }
        }

        // Utility methods
        void setFlag(FLAGS flag, bool v) {
            if (v)
                status |= (byte) flag;
            else
                status &= unchecked((byte) ~flag);
        }

        // Addressing modes
        byte IMP() {
            return 0;
        }

        byte IMM() {
            abs_address = PC;
            PC++;
            return 0;
        }

        byte ZP0() {
            abs_address = read(PC);
            PC++;
            abs_address &= 0x00FF;
            return 0;
        }

        byte ZPX() {
            abs_address = (ushort) (read(PC) + register_x);
            PC++;
            abs_address &= 0x00FF;
            return 0;
        }

        byte ZPY() {
            abs_address = (ushort) (read(PC) + register_y);
            PC++;
            abs_address &= 0x00FF;
            return 0;
        }

        byte REL() {
            rel_address = read(PC);
            PC++;
            if ((rel_address & 0x80) == 0x80)
                rel_address |= 0xFF00;
            return 0;
        }

        byte ABS() {
            ushort lo = read(PC);
            PC++;
            ushort hi = read(PC);
            PC++;
            abs_address = (ushort) ((hi << 8) | lo);
            return 0;
        }

        byte ABX() {
            ushort lo = read(PC);
            PC++;
            ushort hi = read(PC);
            PC++;

            abs_address = (ushort) ((hi << 8) | lo);

            abs_address += register_x;

            if ((abs_address & 0xFF00) == (hi << 8))
                return 0;
            else
                return 1;
        }

        byte ABY() {
            ushort lo = read(PC);
            PC++;
            ushort hi = read(PC);
            PC++;

            abs_address = (ushort) ((hi << 8) | lo);

            abs_address += register_y;

            if ((abs_address & 0xFF00) == (hi << 8))
                return 0;
            else
                return 1;
        }

        byte IND() {
            ushort ptr_lo = read(PC);
            PC++;
            ushort ptr_hi = read(PC);
            PC++;

            ushort ptr = (ushort) ((ptr_hi << 8) | ptr_lo);

            if (ptr_lo == 0x00FF)
                abs_address = (ushort) ((read((ushort) (ptr & 0xFF00)) << 8) | read(ptr));
            else
                abs_address = (ushort) ((read((ushort) (ptr + 1)) << 8) | read(ptr));

            return 0;
        }

        byte IZX() {
            ushort ptr = read(PC);
            PC++;

            ushort addr_lo = read((ushort) ((ptr + (ushort) register_x) & 0x00FF));
            ushort addr_hi = read((ushort) ((ptr + (ushort) register_x + 1) & 0x00FF));

            abs_address = (ushort) ((addr_hi << 8) | addr_lo);

            return 0;
        }

        byte IZY() {
            ushort ptr = read(PC);
            PC++;

            ushort addr_lo = read((ushort) ((ptr) & 0x00FF));
            ushort addr_hi = read((ushort) ((ptr + 1) & 0x00FF));

            abs_address = (ushort) ((addr_hi << 8) | addr_lo);

            abs_address += register_y;

             if ((abs_address & 0xFF00) == (addr_hi << 8))
                return 0;
            else
                return 1;
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

        byte INX() {
            register_x += 1;

            setFlag(FLAGS.Z, (register_x == 0));
            setFlag(FLAGS.N, ((register_x & 0x80) == 0x80));

            return 0;
        }

        byte LDA() {
            register_a = read(abs_address);

            setFlag(FLAGS.Z, (register_a == 0));
            setFlag(FLAGS.N, ((register_a & 0x80) == 0x80));
            
            return 0;
        }

        byte TAX() {
            register_x = register_a;

            setFlag(FLAGS.Z, (register_x == 0));
            setFlag(FLAGS.N, ((register_x & 0x80) == 0x80));

            return 0;
        }

        byte TAY() {
            register_y = register_a;

            setFlag(FLAGS.Z, (register_y == 0));
            setFlag(FLAGS.N, ((register_y & 0x80) == 0x80));

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