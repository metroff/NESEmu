namespace NESEmu
{
    public class CPU
    {
        Bus bus;

        //Registers
        public ushort PC;   //Program counter (16-bit)
        public byte SP;     //Stack pointer
        public byte register_a;     //Accumulator
        public byte register_x;     //Index register X
        public byte register_y;     //Index register Y
        public byte status; //Processor status (flags)
        
        //Helper
        ushort abs_address;
        ushort rel_address;
        byte _opcode;
        uint _cycles;

        //Flags
        public enum FLAGS {
            C = (1 << 0),   //Carry flag
            Z = (1 << 1),  //Zero flag
            I = (1 << 2),  //Interrupt disable
            D = (1 << 3),  //Decimal Mode flag
            B = (1 << 4),  //Break command
            U = (1 << 5),  //Unused flag
            V = (1 << 6),  //Overflow flag
            N = (1 << 7),  //Negative flag
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
                new Instruction("BRK", BRK, IMP, 7), new Instruction("ORA", ORA, IZX, 6), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*SLO", SLO, IZX, 8),new Instruction("*NOP", NOP, ZP0, 3),new Instruction("ORA", ORA, ZP0, 3), new Instruction("ASL", ASL, ZP0, 5), new Instruction("*SLO", SLO, ZP0, 5),new Instruction("PHP", PHP, IMP, 3), new Instruction("ORA", ORA, IMM, 2), new Instruction("ASL", ASL, IMP, 2), new Instruction("*ANC", ANC, IMM, 2),new Instruction("*NOP", NOP, ABS, 4),new Instruction("ORA", ORA, ABS, 4), new Instruction("ASL", ASL, ABS, 6), new Instruction("*SLO", SLO, ABS, 6),// 0
                new Instruction("BPL", BPL, REL, 2), new Instruction("ORA", ORA, IZY, 5), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*SLO", SLO, IZY, 8),new Instruction("*NOP", NOP, ZPX, 4),new Instruction("ORA", ORA, ZPX, 4), new Instruction("ASL", ASL, ZPX, 6), new Instruction("*SLO", SLO, ZPX, 6),new Instruction("CLC", CLC, IMP, 2), new Instruction("ORA", ORA, ABY, 4), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*SLO", SLO, ABY, 7),new Instruction("*NOP", NOP, ABX, 4),new Instruction("ORA", ORA, ABX, 4), new Instruction("ASL", ASL, ABX, 7), new Instruction("*SLO", SLO, ABX, 7),// 1
                new Instruction("JSR", JSR, ABS, 6), new Instruction("AND", AND, IZX, 6), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*RLA", RLA, IZX, 8),new Instruction("BIT", BIT, ZP0, 3), new Instruction("AND", AND, ZP0, 3), new Instruction("ROL", ROL, ZP0, 5), new Instruction("*RLA", RLA, ZP0, 5),new Instruction("PLP", PLP, IMP, 4), new Instruction("AND", AND, IMM, 2), new Instruction("ROL", ROL, IMP, 2), new Instruction("*ANC", ANC, IMM, 2),new Instruction("BIT", BIT, ABS, 4), new Instruction("AND", AND, ABS, 4), new Instruction("ROL", ROL, ABS, 6), new Instruction("*RLA", RLA, ABS, 6),// 2
                new Instruction("BMI", BMI, REL, 2), new Instruction("AND", AND, IZY, 5), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*RLA", RLA, IZY, 8),new Instruction("*NOP", NOP, ZPX, 4),new Instruction("AND", AND, ZPX, 4), new Instruction("ROL", ROL, ZPX, 6), new Instruction("*RLA", RLA, ZPX, 6),new Instruction("SEC", SEC, IMP, 2), new Instruction("AND", AND, ABY, 7), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*RLA", RLA, ABY, 7),new Instruction("*NOP", NOP, ABX, 4),new Instruction("AND", AND, ABX, 4), new Instruction("ROL", ROL, ABX, 7), new Instruction("*RLA", RLA, ABX, 7),// 3
                new Instruction("RTI", RTI, IMP, 6), new Instruction("EOR", EOR, IZX, 6), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*SRE", SRE, IZX, 8),new Instruction("*NOP", NOP, ZP0, 3),new Instruction("EOR", EOR, ZP0, 3), new Instruction("LSR", LSR, ZP0, 5), new Instruction("*SRE", SRE, ZP0, 5),new Instruction("PHA", PHA, IMP, 3), new Instruction("EOR", EOR, IMM, 2), new Instruction("LSR", LSR, IMP, 2), new Instruction("*ALR", ALR, IMM, 2),new Instruction("JMP", JMP, ABS, 3), new Instruction("EOR", EOR, ABS, 4), new Instruction("LSR", LSR, ABS, 6), new Instruction("*SRE", SRE, ABS, 6),// 4
                new Instruction("BVC", BVC, REL, 2), new Instruction("EOR", EOR, IZY, 5), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*SRE", SRE, IZY, 8),new Instruction("*NOP", NOP, ZPX, 4),new Instruction("EOR", EOR, ZPX, 4), new Instruction("LSR", LSR, ZPX, 6), new Instruction("*SRE", SRE, ZPX, 6),new Instruction("CLI", CLI, IMP, 2), new Instruction("EOR", EOR, ABY, 4), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*SRE", SRE, ABY, 7),new Instruction("*NOP", NOP, ABX, 4),new Instruction("EOR", EOR, ABX, 4), new Instruction("LSR", LSR, ABX, 7), new Instruction("*SRE", SRE, ABX, 7),// 5
                new Instruction("RTS", RTS, IMP, 6), new Instruction("ADC", ADC, IZX, 6), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*RRA", RRA, IZX, 8),new Instruction("*NOP", NOP, ZP0, 3),new Instruction("ADC", ADC, ZP0, 3), new Instruction("ROR", ROR, ZP0, 5), new Instruction("*RRA", RRA, ZP0, 5),new Instruction("PLA", PLA, IMP, 4), new Instruction("ADC", ADC, IMM, 2), new Instruction("ROR", ROR, IMP, 2), new Instruction("*ARR", ARR, IMM, 2),new Instruction("JMP", JMP, IND, 5), new Instruction("ADC", ADC, ABS, 4), new Instruction("ROR", ROR, ABS, 6), new Instruction("*RRA", RRA, ABS, 6),// 6
                new Instruction("BVS", BVS, REL, 2), new Instruction("ADC", ADC, IZY, 5), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*RRA", RRA, IZY, 8),new Instruction("*NOP", NOP, ZPX, 4),new Instruction("ADC", ADC, ZPX, 4), new Instruction("ROR", ROR, ZPX, 6), new Instruction("*RRA", RRA, ZPX, 6),new Instruction("SEI", SEI, IMP, 2), new Instruction("ADC", ADC, ABY, 4), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*RRA", RRA, ABY, 7),new Instruction("*NOP", NOP, ABX, 4),new Instruction("ADC", ADC, ABX, 4), new Instruction("ROR", ROR, ABX, 7), new Instruction("*RRA", RRA, ABX, 7),// 7
                new Instruction("*NOP", NOP, IMM, 2),new Instruction("STA", STA, IZX, 6), new Instruction("*NOP", NOP, IMM, 2),new Instruction("*SAX", SAX, IZX, 6),new Instruction("STY", STY, ZP0, 3), new Instruction("STA", STA, ZP0, 3), new Instruction("STX", STX, ZP0, 3), new Instruction("*SAX", SAX, ZP0, 3),new Instruction("DEY", DEY, IMP, 2), new Instruction("*NOP", NOP, IMM, 2),new Instruction("TXA", TXA, IMP, 2), new Instruction("*XAA", XAA, IMM, 2),new Instruction("STY", STY, ABS, 4), new Instruction("STA", STA, ABS, 4), new Instruction("STX", STX, ABS, 4), new Instruction("*SAX", SAX, ABS, 4),// 8
                new Instruction("BCC", BCC, REL, 2), new Instruction("STA", STA, IZY, 6), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*AHX", AHX, IZY, 6),new Instruction("STY", STY, ZPX, 4), new Instruction("STA", STA, ZPX, 4), new Instruction("STX", STX, ZPY, 4), new Instruction("*SAX", SAX, ZPY, 4),new Instruction("TYA", TYA, IMP, 2), new Instruction("STA", STA, ABY, 5), new Instruction("TXS", TXS, IMP, 2), new Instruction("*TAS", TAS, ABY, 5),new Instruction("*SHY", SHY, ABX, 5),new Instruction("STA", STA, ABX, 5), new Instruction("*SHX", SHX, ABY, 5),new Instruction("*AHX", AHX, ABY, 5),// 9
                new Instruction("LDY", LDY, IMM, 2), new Instruction("LDA", LDA, IZX, 6), new Instruction("LDX", LDX, IMM, 2), new Instruction("*LAX", LAX, IZX, 6),new Instruction("LDY", LDY, ZP0, 3), new Instruction("LDA", LDA, ZP0, 3), new Instruction("LDX", LDX, ZP0, 3), new Instruction("*LAX", LAX, ZP0, 3),new Instruction("TAY", TAY, IMP, 2), new Instruction("LDA", LDA, IMM, 2), new Instruction("TAX", TAX, IMP, 2), new Instruction("*LXA", LXA, IMM, 2),new Instruction("LDY", LDY, ABS, 4), new Instruction("LDA", LDA, ABS, 4), new Instruction("LDX", LDX, ABS, 4), new Instruction("*LAX", LAX, ABS, 4),// a
                new Instruction("BCS", BCS, REL, 2), new Instruction("LDA", LDA, IZY, 5), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*LAX", LAX, IZY, 5),new Instruction("LDY", LDY, ZPX, 4), new Instruction("LDA", LDA, ZPX, 4), new Instruction("LDX", LDX, ZPY, 4), new Instruction("*LAX", LAX, ZPY, 4),new Instruction("CLV", CLV, IMP, 2), new Instruction("LDA", LDA, ABY, 4), new Instruction("TSX", TSX, IMP, 2), new Instruction("*LAS", LAS, ABY, 4),new Instruction("LDY", LDY, ABX, 4), new Instruction("LDA", LDA, ABX, 4), new Instruction("LDX", LDX, ABY, 4), new Instruction("*LAX", LAX, ABY, 4),// b
                new Instruction("CPY", CPY, IMM, 2), new Instruction("CMP", CMP, IZX, 6), new Instruction("*NOP", NOP, IMM, 2),new Instruction("*DCP", DCP, IZX, 8),new Instruction("CPY", CPY, ZP0, 3), new Instruction("CMP", CMP, ZP0, 3), new Instruction("DEC", DEC, ZP0, 5), new Instruction("*DCP", DCP, ZP0, 5),new Instruction("INY", INY, IMP, 2), new Instruction("CMP", CMP, IMM, 2), new Instruction("DEX", DEX, IMP, 2), new Instruction("*AXS", AXS, IMM, 2),new Instruction("CPY", CPY, ABS, 4), new Instruction("CMP", CMP, ABS, 4), new Instruction("DEC", DEC, ABS, 6), new Instruction("*DCP", DCP, ABS, 6),// c
                new Instruction("BNE", BNE, REL, 2), new Instruction("CMP", CMP, IZY, 5), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*DCP", DCP, IZY, 8),new Instruction("*NOP", NOP, ZPX, 4),new Instruction("CMP", CMP, ZPX, 4), new Instruction("DEC", DEC, ZPX, 6), new Instruction("*DCP", DCP, ZPX, 6),new Instruction("CLD", CLD, IMP, 2), new Instruction("CMP", CMP, ABY, 4), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*DCP", DCP, ABY, 7),new Instruction("*NOP", NOP, ABX, 4),new Instruction("CMP", CMP, ABX, 4), new Instruction("DEC", DEC, ABX, 7), new Instruction("*DCP", DCP, ABX, 7),// d
                new Instruction("CPX", CPX, IMM, 2), new Instruction("SBC", SBC, IZX, 6), new Instruction("*NOP", NOP, IMM, 2),new Instruction("*ISB", ISB, IZX, 8),new Instruction("CPX", CPX, ZP0, 3), new Instruction("SBC", SBC, ZP0, 3), new Instruction("INC", INC, ZP0, 5), new Instruction("*ISB", ISB, ZP0, 5),new Instruction("INX", INX, IMP, 2), new Instruction("SBC", SBC, IMM, 2), new Instruction("NOP", NOP, IMP, 2), new Instruction("*SBC", USBC,IMM, 2),new Instruction("CPX", CPX, ABS, 4), new Instruction("SBC", SBC, ABS, 4), new Instruction("INC", INC, ABS, 6), new Instruction("*ISB", ISB, ABS, 6),// e
                new Instruction("BEQ", BEQ, REL, 2), new Instruction("SBC", SBC, IZY, 5), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*ISB", ISB, IZY, 8),new Instruction("*NOP", NOP, ZPX, 4),new Instruction("SBC", SBC, ZPX, 4), new Instruction("INC", INC, ZPX, 6), new Instruction("*ISB", ISB, ZPX, 6),new Instruction("SED", SED, IMP, 2), new Instruction("SBC", SBC, ABY, 4), new Instruction("*NOP", NOP, IMP, 2),new Instruction("*ISB", ISB, ABY, 7),new Instruction("*NOP", NOP, ABX, 4),new Instruction("SBC", SBC, ABX, 4), new Instruction("INC", INC, ABX, 7), new Instruction("*ISB", ISB, ABX, 7) // f
            };

            this.bus = bus;
        }

        byte read(ushort addr) {
            return bus.memoryRead(addr);
        }

        void write(ushort addr, byte data) {
            bus.memoryWrite(addr, data);
        }

        void pushStack(byte data) {
            write((ushort)(0x0100 | SP), data);
            SP--;
        }

        byte pullStack() {
            SP++;
            byte data = read((ushort)(0x0100 | SP));
            return data;
        }

        public void reset() {
            register_a = 0;
            register_x = 0;
            register_y = 0;
            // status = 0x00 | (byte) FLAGS.I;
            status = (byte) FLAGS.I;
            status = (byte) (status | (byte) FLAGS.U);
            _cycles = 0;
            SP = 0xFD;
            PC = (ushort)(bus.memoryRead(0xFFFC) | (bus.memoryRead(0xFFFD) << 8));
        }

        public uint clock() {
            if (bus.pollNmiInterruptStatus()) {
                NMI();
            }

            _opcode = read(PC);

            // DissasembleCPU();

            PC++;

            _cycles = _instructions[_opcode].cycles;

            byte address_cycles = _instructions[_opcode].addressMode();
            byte op_cycles = _instructions[_opcode].operation();

            _cycles += (byte)(address_cycles & op_cycles);

            bus.tick((byte)_cycles);

            return _cycles;
        }

        public void interpret(byte[] program) {
            load(program);
            reset();
            PC = 0x0600;

            while(true) {
                clock();
                if (read(PC) == 0)
                    break;
            }
        }

        public void load(byte[] program) {
            for (int i = 0; i < program.Length; i++)
            {
                bus.memoryWrite((ushort)(0x0600 + i),program[i]);
            }
            // bus.memoryWrite(0xFFFC, 0x00);
            // bus.memoryWrite(0xFFFD, 0x86);
        }

        // Utility methods
        void setFlag(FLAGS flag, bool v) {
            if (v)
                status |= (byte) flag;
            else
                status &= unchecked((byte) ~flag);
        }

        byte getFlag(FLAGS flag) {
            return (byte) ((status & (byte) flag) == 0 ? 0 : 1);
        }

        void setFlagZN(byte value) {
            setFlag(FLAGS.Z, value == 0);
            setFlag(FLAGS.N, (value & 0x80) == 0x80);
        }

        void branch(bool condition) {
            if (condition) {
                _cycles++;
                ushort jump_address = (ushort) (PC + rel_address);

                if ((PC & 0xFF00) != (jump_address & 0xFF00)) {
                    _cycles++;
                }

                PC = jump_address;
            }
        }

        void NMI() {
            pushStack((byte)((PC >> 8) & 0x00FF));
            pushStack((byte)(PC & 0x00FF));

            setFlag(FLAGS.B, false);
            setFlag(FLAGS.U, true);
            setFlag(FLAGS.I, true);

            pushStack(status);
            bus.tick(2);

            PC = (ushort)((ushort)read(0xFFFA) | ((ushort)read(0xFFFA + 1) << 8));
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
            ushort memoryContents = read(abs_address);
            ushort result = (ushort) ((ushort) register_a + memoryContents + (ushort) getFlag(FLAGS.C));

            setFlag(FLAGS.C, (result > 0xff));
            setFlag(FLAGS.Z, ((result & 0x00FF) == 0));
            setFlag(FLAGS.V, ((register_a ^ result) & (memoryContents ^ result) & 0x80) != 0);
            setFlag(FLAGS.N, (result & 0x80) == 0x80);

            register_a = (byte)(result & 0x00FF);

            return 1;
        }

        byte AND() {
            register_a &= read(abs_address);

            setFlagZN(register_a);

            return 1;
        }

        byte ASL() {
            if (_instructions[_opcode].addressMode == IMP) {
                setFlag(FLAGS.C, (register_a & 0x80) == 0x80);
                register_a <<= 1;
                setFlagZN(register_a);
            } else {
                byte data = read(abs_address);
                setFlag(FLAGS.C, (data & 0x80) == 0x80);
                data <<= 1;
                write(abs_address, data);
                setFlag(FLAGS.Z, (data == 0));
                setFlag(FLAGS.N, ((data & 0x80) == 0x80));
            } 
                
            return 0;
        }

        byte BCC() {
            branch(getFlag(FLAGS.C) == 0);
            return 0;   
        }

        byte BCS() {
            branch(getFlag(FLAGS.C) != 0);
            return 0; 
        }

        byte BEQ() {
            branch(getFlag(FLAGS.Z) != 0);
            return 0;
        }

        byte BIT() {
            byte memData = read(abs_address);
            byte temp = (byte) (register_a & memData);

            setFlag(FLAGS.Z, temp == 0);
            setFlag(FLAGS.V, (memData & 0x40) == 0x40);
            setFlag(FLAGS.N, (memData & 0x80) == 0x80);

            return 0;
        }

        byte BMI() {
            branch(getFlag(FLAGS.N) != 0);
            return 0;
        }

        byte BNE() {
            branch(getFlag(FLAGS.Z) == 0);
            return 0;
        }

        byte BPL() {
            branch(getFlag(FLAGS.N) == 0);
            return 0;
        }

        byte BRK() {
            PC++;

            status |= (byte) FLAGS.B;
            pushStack((byte)((PC >> 8) & 0x00FF));
            pushStack((byte)(PC & 0x00FF));
            pushStack(status);

            PC = (ushort)((read(0xFFFF) << 8) | read(0xFFFE));
            return 0;
        }

        byte BVC() {
            branch(getFlag(FLAGS.V) == 0);
            return 0;
        }

        byte BVS() {
            branch(getFlag(FLAGS.V) != 0);
            return 0;
        }

        byte CLC() {
            setFlag(FLAGS.C, false);
            return 0;
        }

        byte CLD() {
            setFlag(FLAGS.D, false);
            return 0;
        }

        byte CLI() {
            setFlag(FLAGS.I, false);
            return 0;
        }

        byte CLV() {
            setFlag(FLAGS.V, false);
            return 0;
        }

        byte CMP() {
            byte data = read(abs_address);
            setFlag(FLAGS.C, register_a >= data);
            setFlag(FLAGS.Z, register_a == data);
            setFlag(FLAGS.N, ((register_a - data) & 0x80) == 0x80);
            return 1;
        }

        byte CPX() {
            byte data = read(abs_address);
            setFlag(FLAGS.C, register_x >= data);
            setFlag(FLAGS.Z, register_x == data);
            setFlag(FLAGS.N, ((register_x - data) & 0x80) == 0x80);
            return 0;
        }

        byte CPY() {
            byte data = read(abs_address);
            setFlag(FLAGS.C, register_y >= data);
            setFlag(FLAGS.Z, register_y == data);
            setFlag(FLAGS.N, ((register_y - data) & 0x80) == 0x80);
            return 0;
        }

        byte DEC() {
            byte data = read(abs_address);
            data--;
            setFlagZN(data);
            write(abs_address, data);
            return 0;
        }

        byte DEX() {
            register_x--;
            setFlagZN(register_x);
            return 0;
        }

        byte DEY() {
            register_y--;
            setFlagZN(register_y);
            return 0;
        }

        byte EOR() {
            register_a ^= read(abs_address);

            setFlagZN(register_a);

            return 1;
        }

        byte INC() {
            byte data = read(abs_address);
            data += 1;
            setFlagZN(data);
            write(abs_address, data);
            return 0;
        }

        byte INX() {
            register_x += 1;

            setFlagZN(register_x);

            return 0;
        }

        byte INY() {
            register_y += 1;

            setFlagZN(register_y);

            return 0;
        }

        byte JMP() {
            PC = abs_address;
            return 0;
        }

        byte JSR() {
            PC--;
            pushStack((byte)((PC >> 8) & 0x00FF));
            pushStack((byte)(PC & 0x00FF));
            PC = abs_address;
            return 0;
        }

        byte LDA() {
            register_a = read(abs_address);

            setFlagZN(register_a);
            
            return 1;
        }

        byte LDX() {
            register_x = read(abs_address);

            setFlagZN(register_x);

            return 1;
        }
        
        byte LDY() {
            register_y = read(abs_address);

            setFlagZN(register_y);

            return 1;
        }

        byte LSR() {
            if (_instructions[_opcode].addressMode == IMP) {
                setFlag(FLAGS.C, (register_a & 0x01) == 0x01);
                register_a >>= 1;
                setFlagZN(register_a);
            } else {
                byte data = read(abs_address);
                setFlag(FLAGS.C, (data & 0x01) == 0x01);
                data >>= 1;
                write(abs_address, data);
                setFlag(FLAGS.Z, (data == 0));
                setFlag(FLAGS.N, ((data & 0x80) == 0x80));
            } 
                
            return 0;
        }

        byte NOP() {
            return 0;
        }

        byte ORA() {
            register_a |= read(abs_address);

            setFlagZN(register_a);

            return 1;
        }

        byte PHA() {
            pushStack(register_a);
            return 0;
        }

        byte PHP() {
            pushStack((byte) (status | (byte) FLAGS.B));
            return 0;
        }

        byte PLA() {
            register_a = pullStack();
            setFlagZN(register_a);
            return 0;
        }

        byte PLP() {
            status = pullStack();
            setFlag(FLAGS.B, false);
            setFlag(FLAGS.U, true);
            return 0;
        }

        byte ROL() {
            byte carryOld = getFlag(FLAGS.C);

            if (_instructions[_opcode].addressMode == IMP) {
                setFlag(FLAGS.C, (register_a & 0x80) == 0x80);
                register_a <<= 1;
                register_a |= carryOld;
                setFlagZN(register_a);
            } else {
                byte data = read(abs_address);
                setFlag(FLAGS.C, (data & 0x80) == 0x80);
                data <<= 1;
                data |= carryOld;
                write(abs_address, data);
                setFlag(FLAGS.Z, (data == 0));
                setFlag(FLAGS.N, ((data & 0x80) == 0x80));
            } 

            return 0;
        }

        byte ROR() {
            byte carryOld = getFlag(FLAGS.C);

            if (_instructions[_opcode].addressMode == IMP) {
                setFlag(FLAGS.C, (register_a & 0x01) == 0x01);
                register_a >>= 1;
                register_a |= (byte) ((carryOld == 1) ? 0x80 : 0x00);
                setFlagZN(register_a);
            } else {
                byte data = read(abs_address);
                setFlag(FLAGS.C, (data & 0x01) == 0x01);
                data >>= 1;
                data |= (byte) ((carryOld == 1) ? 0x80 : 0x00);
                write(abs_address, data);
                setFlag(FLAGS.Z, (data == 0));
                setFlag(FLAGS.N, ((data & 0x80) == 0x80));
            } 

            return 0;
        }

        byte RTI() {
            status = pullStack();
            setFlag(FLAGS.B, false);
            setFlag(FLAGS.U, true);
            PC = (ushort)(pullStack() | ((ushort)pullStack() << 8));
            return 0;
        }

        byte RTS() {
            PC = (ushort)(pullStack() | (pullStack() << 8));
            PC++;
            return 0;
        }

        byte SBC() {
            ushort memoryContents = read(abs_address);
            ushort value = (ushort) (memoryContents ^ 0x00FF);
            ushort result = (ushort) ((ushort) register_a + value + (ushort) getFlag(FLAGS.C));

            setFlag(FLAGS.C, (result > 0xff));
            setFlag(FLAGS.Z, ((result & 0x00FF) == 0));
            setFlag(FLAGS.V, ((register_a ^ result) & (value ^ result) & 0x80) != 0);
            setFlag(FLAGS.N, (result & 0x80) == 0x80);

            register_a = (byte)(result & 0x00FF);

            return 1;
        }
        
        byte SEC() {
            setFlag(FLAGS.C, true);
            return 0;
        }

        byte SED() {
            setFlag(FLAGS.D, true);
            return 0;
        }

        byte SEI() {
            setFlag(FLAGS.I, true);
            return 0;
        }

        byte STA() {
            write(abs_address, register_a);
            return 0;
        }

        byte STX() {
            write(abs_address, register_x);
            return 0;
        }

        byte STY() {
            write(abs_address, register_y);
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

        byte TSX() {
            register_x = SP;
            setFlagZN(register_x);
            return 0;
        }

        byte TXA() {
            register_a = register_x;
            setFlagZN(register_a);
            return 0;
        }

        byte TXS() {
            SP = register_x;
            return 0;
        }

        byte TYA() {
            register_a = register_y;
            setFlagZN(register_a);
            return 0;
        }

        // Illegal opcodes
        byte DCP() {
            byte value = read(abs_address);
            value -= 1;
            write(abs_address, value);
            if (value <= register_a)
                setFlag(FLAGS.C, true);

            setFlagZN((byte) (register_a - value));
            return 0;
        }

        byte ISB() {
            byte value = read(abs_address);
            value += 1;
            write(abs_address, value);

            ushort result = (ushort) ((ushort) register_a - value - (1 - (ushort) getFlag(FLAGS.C)));

            setFlag(FLAGS.C, (result <= register_a));
            setFlag(FLAGS.V, ((register_a ^ result) & (value ^ result) & 0x80) != 0);

            register_a = (byte)(result & 0x00FF);
            setFlagZN(register_a);

            return 0;
        }

        byte LAX() {
            byte value = read(abs_address);
            register_a = value;
            setFlagZN(value);
            register_x = value;
            return 1;
        }

        byte SAX() {
            write(abs_address, (byte)(register_a & register_x));
            return 0;
        }

        byte SLO() {
            byte value = read(abs_address);
            setFlag(FLAGS.C, (value & 0x80) == 0x80);
            value <<= 1;
            register_a |= value;
            write(abs_address, value);
            setFlagZN(register_a);
            return 0;
        }

        byte USBC() {
            ushort memoryContents = read(abs_address);
            ushort value = (ushort) (memoryContents ^ 0x00FF);
            ushort result = (ushort) ((ushort) register_a + value + (ushort) getFlag(FLAGS.C));

            setFlag(FLAGS.C, (result > 0xff));
            setFlag(FLAGS.Z, ((result & 0x00FF) == 0));
            setFlag(FLAGS.V, ((register_a ^ result) & (value ^ result) & 0x80) != 0);
            setFlag(FLAGS.N, (result & 0x80) == 0x80);

            register_a = (byte)(result & 0x00FF);
            return 0;
        }

        byte RLA() {
            byte value = read(abs_address);
            byte temp = getFlag(FLAGS.C);
            setFlag(FLAGS.C, (value & 0x80) == 0x80);
            value = (byte)((value << 1) | temp);
            write(abs_address, value);
            register_a &= value;
            setFlagZN(register_a);
            return 0;
        }

        byte SRE() {
            byte value = read(abs_address);
            setFlag(FLAGS.C, (value & 0x1) == 0x1);
            value >>= 1;
            write(abs_address, value);
            register_a ^= value;
            setFlagZN(register_a);
            return 0;
        }

        byte RRA() {
            byte value = read(abs_address);
            byte temp = getFlag(FLAGS.C);
            setFlag(FLAGS.C, (value & 0x1) == 0x1);
            value = (byte)((value >> 1) | (temp << 7));
            write(abs_address, value);
            ushort result = (ushort) ((ushort) register_a + value + (ushort) getFlag(FLAGS.C));
            setFlag(FLAGS.C, (result > 0xff));
            setFlag(FLAGS.V, ((register_a ^ result) & (value ^ result) & 0x80) != 0);
            register_a = (byte) (result & 0xff);
            setFlagZN(register_a);
            return 0;
        }

        byte ANC() {
            register_a &= read(abs_address);
            setFlagZN(register_a);
            setFlag(FLAGS.C, getFlag(FLAGS.N) == (byte) FLAGS.N);
            return 0;
        }

        byte ALR() {
            register_a &= read(abs_address);
            setFlag(FLAGS.C, (register_a & 0x1) == 0x1);
            register_a >>= 1;
            setFlagZN(register_a);
            return 0;
        }

        byte ARR() {
            register_a &= read(abs_address);
            register_a = (byte)((register_a >> 1) | (getFlag(FLAGS.C) << 7));
            setFlagZN(register_a);
            setFlag(FLAGS.C, ((register_a >> 6) & 0x1) == 0x1);
            setFlag(FLAGS.V, (((register_a >> 5) & 0x1) ^ getFlag(FLAGS.C)) == 0x1);
            return 0;
        }

        byte AXS() {
            ushort result = (ushort)((register_x & register_a) - read(abs_address));
            register_x = (byte) (result & 0xff);
            setFlagZN(register_x);
            setFlag(FLAGS.C, result >= 0);
            return 0;
        }

        byte XAA() {
            register_a = register_x;
            setFlagZN(register_a);
            byte data = read(abs_address);
            register_a &= data;
            setFlagZN(register_a);
            return 0;
        }

        byte TAS() {
            byte data = (byte)(register_a & register_x);
            SP = data;
            data = (byte)(((byte)(abs_address >> 8) + 1) & SP);
            write(abs_address, data);
            return 0;
        }

        byte AHX() {
            byte data = (byte) (register_a & register_x & (byte) (abs_address >> 8));
            write(abs_address, data);
            return 0;
        }

        byte LXA() {
            LDA();
            TAX();
            return 0;
        }

        byte LAS() {
            byte data = read(abs_address);
            data = (byte)(data & SP);
            register_a = data;
            register_x = data;
            SP = data;
            setFlagZN(data);
            return 1;
        }

        byte SHX() {
            byte data = (byte) (register_x & ((byte) (abs_address >> 8) + 1));
            write(abs_address, data);
            return 0;
        }

        byte SHY() {
            byte data = (byte) (register_y & ((byte) (abs_address >> 8) + 1));
            write(abs_address, data);
            return 0;
        }

        byte XXX() {
            throw new Exception("Unimplemented instruction " + _opcode.ToString("x") + _instructions[_opcode].name);
        }

        void DissasembleCPU() {
            ushort _addr = PC;
            string pcString = string.Format("{0:X4}", _addr);
            byte opcode = read(_addr); _addr++;
            string memory = "";
            switch (_instructions[opcode].addressMode) {
                case var v when v == IMP:
                {
                    // if (opcode == 0x0a | opcode == 0x4a | opcode == 0x2a | opcode == 0x6a)
                    if (opcode == 0x0a || opcode == 0x4a || opcode == 0x2a || opcode == 0x6a)
                        memory = string.Format("      {0,4} A                         ", _instructions[opcode].name);
                    else
                        memory = string.Format("      {0,4}                           ", _instructions[opcode].name);
                }
                    break;
                case var v when v == IMM:
                {
                    int value = read(_addr);
                    memory = string.Format("{0:X2}    {1,4} #${2,-24:X2}", value, _instructions[opcode].name, value);
                }
                    break;
                case var v when v == ZP0:
                {
                    ushort addr = read(_addr);
                    ushort value = read(addr);
                    memory = string.Format("{0:X2}    {1,4} ${2:X2} = {3,-20:X2}", addr, _instructions[opcode].name, addr, value);
                }
                    break;
                case var v when v == ZPX:
                {   
                    byte temp = read(_addr); _addr++;
                    ushort addr = (ushort) (temp + register_x);
                    addr &= 0x00FF;
                    byte value = read(addr);
                    memory = string.Format("{0:X2}    {1,4} ${2:X2},X @ {3:X2} = {4,-13:X2}", temp, _instructions[opcode].name, temp, addr, value);

                }
                    break;
                case var v when v == ZPY:
                {
                    byte temp = read(_addr); _addr++;
                    ushort addr = (ushort) (temp + register_y);
                    addr &= 0x00FF;
                    byte value = read(addr);
                    memory = string.Format("{0:X2}    {1,4} ${2:X2},Y @ {3:X2} = {4,-13:X2}", temp, _instructions[opcode].name, temp, addr, value);
                }
                    break;
                case var v when v == REL:
                {
                    ushort value = read(_addr); _addr++;
                    ushort temp_addr = value;
                    if ((temp_addr & 0x80) == 0x80)
                        temp_addr |= 0xFF00;
                    ushort jump_address = (ushort) (_addr + temp_addr);
                    memory = string.Format("{0:X2}    {1,4} ${2,-25:X4}", value, _instructions[opcode].name, jump_address);
                }
                    break;
                case var v when v == ABS:
                {
                    int lo = read(_addr); _addr++;
                    int hi = read(_addr); _addr++;
                    byte value = read((ushort)((hi << 8) | lo));
                    // if (opcode != 0x4c & opcode != 0x20)
                    if (opcode != 0x4c && opcode != 0x20)
                        memory = string.Format("{0:X2} {1:X2} {2,4} ${3:X4} = {4,-18:X2}", lo, hi, _instructions[opcode].name, (hi << 8) | lo, value);
                    else
                        memory = string.Format("{0:X2} {1:X2} {2,4} ${3,-25:X4}", lo, hi, _instructions[opcode].name, (hi << 8) | lo);
                }
                    break;
                case var v when v == ABX:
                {
                    ushort lo = read(_addr); _addr++;
                    ushort hi = read(_addr); _addr++;

                    ushort temp_addr = (ushort) ((hi << 8) | lo);
                    ushort addr = (ushort) (temp_addr + register_x);

                    byte value = read(addr);

                    memory = string.Format("{0:X2} {1:X2} {2,4} ${3:X4},X @ {4:X4} = {5,-9:X2}", lo, hi, _instructions[opcode].name, (hi << 8) | lo, addr, value);
                }
                    break;
                case var v when v == ABY:
                {
                    ushort lo = read(_addr); _addr++;
                    ushort hi = read(_addr); _addr++;

                    ushort addr = (ushort) ((hi << 8) | lo);

                    addr += register_y;
                    byte value = read(addr);

                    memory = string.Format("{0:X2} {1:X2} {2,4} ${3:X4},Y @ {4:X4} = {5,-9:X2}", lo, hi, _instructions[opcode].name, (hi << 8) | lo, addr, value);
                }
                    break;
                case var v when v == IND:
                {
                    ushort ptr_lo = read(_addr); _addr++;
                    ushort ptr_hi = read(_addr); _addr++;

                    ushort ptr = (ushort) ((ptr_hi << 8) | ptr_lo);

                    ushort addr;

                    if (ptr_lo == 0x00FF)
                        addr = (ushort) ((read((ushort) (ptr & 0xFF00)) << 8) | read(ptr));
                    else
                        addr = (ushort) ((read((ushort) (ptr + 1)) << 8) | read(ptr));

                    memory = string.Format("{0:X2} {1:X2} {2,4} (${3:X4}) = {4,-16:X4}", ptr_lo, ptr_hi, _instructions[opcode].name, (ptr_hi << 8) | ptr_lo, addr);
                }
                    break;
                case var v when v == IZX:
                {
                    ushort ptr = read(_addr); _addr++;

                    ushort lo = read((ushort) ((ptr + (ushort) register_x) & 0x00FF));
                    ushort hi = read((ushort) ((ptr + (ushort) register_x + 1) & 0x00FF));

                    ushort addr = (ushort) ((hi << 8) | lo);
                    byte value = read(addr);
                    memory = string.Format("{0:X2}    {1,4} (${0:X2},X) @ {2:X2} = {3:X4} = {4,-4:X2}", ptr, _instructions[opcode].name, (ptr + register_x) & 0xff, addr, value);
                }
                    break;
                case var v when v == IZY:
                {
                    ushort ptr = read(_addr); _addr++;

                    ushort addr_lo = read((ushort) ((ptr) & 0x00FF));
                    ushort addr_hi = read((ushort) ((ptr + 1) & 0x00FF));

                    ushort addr_y = (ushort) ((addr_hi << 8) | addr_lo);
                    ushort addr = (ushort) (addr_y + register_y);
                    byte value = read(addr);

                    memory = string.Format("{0:X2}    {1,4} (${0:X2}),Y = {2:X4} @ {3:X4} = {4:X2}", ptr, _instructions[opcode].name, addr_y, addr, value);
                }
                    break;
            }

            string registers = string.Format("A:{0:X2} X:{1:X2} Y:{2:X2} P:{3:X2} SP:{4:X2}", register_a, register_x, register_y, status, SP);
            System.Console.WriteLine("{0}  {1:X2} {2}  {3}", pcString, opcode, memory, registers);
        }
    }
}