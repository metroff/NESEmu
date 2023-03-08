namespace NESEmu {

    public enum Mirroring {
        VERTICAL,
        HORIZONTAL,
        FOUR_SCREEN
    }

    public class Rom 
    {
        byte[] NES_TAG = new byte[] {0x4e, 0x45, 0x53, 0x1a};
        public List<byte> prgRom;
        public List<byte> chrRom;
        public byte mapper;
        public Mirroring screenMirroring;

        public Rom(byte[] raw) {
            for (int i = 0; i < 4; i++) {
                if (!raw[i].Equals(NES_TAG[i])) {
                    throw new Exception("File is not in iNES file format.");
                }
            }

            mapper = (byte)((raw[7] & 0b11110000) | (raw[6] >> 4));

            byte inesVer = (byte) ((raw[7]>>2) & 0b11);
            if (inesVer != 0) {
                throw new Exception("NES 2.0 format not supported.");
            }

            bool fourScreenMirroring = (raw[6] & 0b1000) != 0;
            bool verticalMirroring = (raw[6] & 0b1) != 0;

            if (fourScreenMirroring) {
                screenMirroring = Mirroring.FOUR_SCREEN;
            } else if (verticalMirroring) {
                screenMirroring = Mirroring.VERTICAL;
            } else {
                screenMirroring = Mirroring.HORIZONTAL;
            }

            uint prgRomSize = (uint) raw[4] * 16384;
            uint chrRomSize = (uint) raw[5] * 8192;

            bool skipTrainer = (raw[6] & 0b100) != 0;

            int prgRomStart = 16 + (skipTrainer ? 512 : 0);
            int chrRomStart = (int) (prgRomStart + prgRomSize);

            prgRom = new List<byte>(raw.ToList().GetRange(prgRomStart, ((int) prgRomSize)));
            chrRom = new List<byte>(raw.ToList().GetRange(chrRomStart, ((int) chrRomSize)));
        }
    }

}