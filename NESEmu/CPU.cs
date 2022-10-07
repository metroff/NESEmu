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
        byte _cycles;

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
                new Instruction("BRK", BRK, IMP, 7), new Instruction("ORA", ORA, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ORA", ORA, ZP0, 3), new Instruction("ASL", ASL, ZP0, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ORA", ORA, IMM, 2), new Instruction("ASL", ASL, IMP, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ORA", ORA, ABS, 4), new Instruction("ASL", ASL, ABS, 6), new Instruction("XXX", XXX, XXX, 7), // 0
                new Instruction("BPL", BPL, REL, 2), new Instruction("ORA", ORA, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ORA", ORA, ZPX, 4), new Instruction("ASL", ASL, ZPX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("CLC", CLC, IMP, 2), new Instruction("ORA", ORA, ABY, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ORA", ORA, ABX, 4), new Instruction("ASL", ASL, ABX, 7), new Instruction("XXX", XXX, XXX, 7), // 1
                new Instruction("XXX", XXX, XXX, 7), new Instruction("AND", AND, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("BIT", BIT, ZP0, 3), new Instruction("AND", AND, ZP0, 3), new Instruction("ROL", ROL, ZP0, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("AND", AND, IMM, 2), new Instruction("ROL", ROL, IMP, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("BIT", BIT, ABS, 4), new Instruction("AND", AND, ABS, 4), new Instruction("ROL", ROL, ABS, 6), new Instruction("XXX", XXX, XXX, 7), // 2
                new Instruction("BMI", BMI, REL, 2), new Instruction("AND", AND, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("AND", AND, ZPX, 4), new Instruction("ROL", ROL, ZPX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("SEC", SEC, IMP, 2), new Instruction("AND", AND, ABY, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("AND", AND, ABX, 4), new Instruction("ROL", ROL, ABX, 7), new Instruction("XXX", XXX, XXX, 7), // 3
                new Instruction("XXX", XXX, XXX, 7), new Instruction("EOR", EOR, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("EOR", EOR, ZP0, 3), new Instruction("LSR", LSR, ZP0, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("EOR", EOR, IMM, 2), new Instruction("LSR", LSR, IMP, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("EOR", EOR, ABS, 4), new Instruction("LSR", LSR, ABS, 6), new Instruction("XXX", XXX, XXX, 7), // 4
                new Instruction("BVC", BVC, REL, 2), new Instruction("EOR", EOR, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("EOR", EOR, ZPX, 4), new Instruction("LSR", LSR, ZPX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("CLI", CLI, IMP, 2), new Instruction("EOR", EOR, ABY, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("EOR", EOR, ABX, 4), new Instruction("LSR", LSR, ABX, 7), new Instruction("XXX", XXX, XXX, 7), // 5
                new Instruction("XXX", XXX, XXX, 7), new Instruction("ADC", ADC, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ADC", ADC, ZP0, 3), new Instruction("ROR", ROR, ZP0, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ADC", ADC, IMM, 2), new Instruction("ROR", ROR, IMP, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ADC", ADC, ABS, 4), new Instruction("ROR", ROR, ABS, 6), new Instruction("XXX", XXX, XXX, 7), // 6
                new Instruction("BVS", BVS, REL, 2), new Instruction("ADC", ADC, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ADC", ADC, ZPX, 4), new Instruction("ROR", ROR, ZPX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("SEI", SEI, IMP, 2), new Instruction("ADC", ADC, ABY, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("ADC", ADC, ABX, 4), new Instruction("ROR", ROR, ABX, 7), new Instruction("XXX", XXX, XXX, 7), // 7
                new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 8
                new Instruction("BCC", BCC, REL, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // 9
                new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ZP0, 3), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("TAY", TAY, IMP, 2), new Instruction("LDA", LDA, IMM, 2), new Instruction("TAX", TAX, IMP, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ABS, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // a
                new Instruction("BCS", BCS, REL, 2), new Instruction("LDA", LDA, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ZPX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CLV", CLV, IMP, 2), new Instruction("LDA", LDA, ABY, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("LDA", LDA, ABX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // b
                new Instruction("CPY", CPY, IMM, 2), new Instruction("CMP", CMP, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CPY", CPY, ZP0, 3), new Instruction("CMP", CMP, ZP0, 3), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CMP", CMP, IMM, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CPY", CPY, ABS, 4), new Instruction("CMP", CMP, ABS, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // c
                new Instruction("BNE", BNE, REL, 2), new Instruction("CMP", CMP, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CMP", CMP, ZPX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CLD", CLD, IMP, 2), new Instruction("CMP", CMP, ABY, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CMP", CMP, ABX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // d
                new Instruction("CPX", CPX, IMM, 2), new Instruction("SBC", SBC, IZX, 6), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CPX", CPX, ZP0, 3), new Instruction("SBC", SBC, ZP0, 3), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("INX", INX, IMP, 2), new Instruction("SBC", SBC, IMM, 2), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("CPX", CPX, ABS, 4), new Instruction("SBC", SBC, ABS, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), // e
                new Instruction("BEQ", BEQ, REL, 2), new Instruction("SBC", SBC, IZY, 5), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("SBC", SBC, ZPX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("SED", SED, IMP, 2), new Instruction("SBC", SBC, ABY, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7), new Instruction("SBC", SBC, ABX, 4), new Instruction("XXX", XXX, XXX, 7), new Instruction("XXX", XXX, XXX, 7)  // f
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
            return read((ushort)(0x0100 | SP));
        }

        public void reset() {
            register_a = 0;
            register_x = 0;
            register_y = 0;
            status = 0x00 | (byte) FLAGS.U;
            PC = 0;
            _cycles = 0;
            SP = 0xFF;
        }

        public void clock() {
            _opcode = read(PC);
            PC++;

            byte cycles = _instructions[_opcode].cycles;

            cycles += _instructions[_opcode].addressMode();

            cycles += _instructions[_opcode].operation();
        }

        public void interpret(byte[] program) {
            for (int i = 0; i < program.Length; i++)
            {
                bus.memoryWrite((ushort)(0x0600 + i),program[i]);
            }

            reset();

            PC = 0x0600;

            while(true) {
                clock();
                if (read(PC) == 0)
                    break;
            }
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

        void setFlagZNRegA() {
            setFlag(FLAGS.Z, register_a == 0);
            setFlag(FLAGS.N, (register_a & 0x80) == 0x80);
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

            setFlagZNRegA();

            return 1;
        }

        byte ASL() {
            if (_instructions[_opcode].addressMode == IMP) {
                setFlag(FLAGS.C, (register_a & 0x80) == 0x80);
                register_a <<= 1;
                setFlagZNRegA();
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

        byte EOR() {
            register_a ^= read(abs_address);

            setFlagZNRegA();

            return 1;
        }

        byte INX() {
            register_x += 1;

            setFlag(FLAGS.Z, (register_x == 0));
            setFlag(FLAGS.N, ((register_x & 0x80) == 0x80));

            return 0;
        }

        byte LDA() {
            register_a = read(abs_address);

            setFlagZNRegA();
            
            return 0;
        }

        byte LSR() {
            if (_instructions[_opcode].addressMode == IMP) {
                setFlag(FLAGS.C, (register_a & 0x01) == 0x01);
                register_a >>= 1;
                setFlagZNRegA();
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

        byte ORA() {
            register_a |= read(abs_address);

            setFlagZNRegA();

            return 1;
        }

        byte ROL() {
            byte carryOld = getFlag(FLAGS.C);

            if (_instructions[_opcode].addressMode == IMP) {
                setFlag(FLAGS.C, (register_a & 0x80) == 0x80);
                register_a <<= 1;
                register_a |= carryOld;
                setFlagZNRegA();
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
                setFlagZNRegA();
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
            throw new Exception("Unimplemented instruction " + _opcode.ToString("x") + _instructions[_opcode].name);
        }

        void DissasembleCPU(byte opcode) {
            System.Console.WriteLine(_instructions[opcode].name);
        }
    }
}