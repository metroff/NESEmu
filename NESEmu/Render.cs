namespace NESEmu
{
    public class Renderer 
    {
        public static byte[] backgroundPallette(ref PPU ppu, byte[] attribute_table, int tile_column, int tile_row) {
            int attr_table_idx = tile_row / 4 * 8 + tile_column / 4;
            byte attr_byte = attribute_table[attr_table_idx];

            

            byte pallete_idx;
            switch (((tile_column % 4) / 2, (tile_row % 4) /2))
            {
                case (0, 0):
                    pallete_idx = (byte) (attr_byte & 0b11);
                    break;
                case (1, 0):
                    pallete_idx = (byte)((attr_byte >> 2) & 0b11);
                    break;
                case (0, 1):
                    pallete_idx = (byte)((attr_byte >> 4) & 0b11);
                    break;
                case (1, 1):
                    pallete_idx = (byte)((attr_byte >> 6) & 0b11);
                    break;
                default:
                    throw new Exception("Pallete index not found.");
            }

            ushort pallete_start = (ushort)(1 + (pallete_idx * 4));
            return new byte[4] {ppu.palleteTable[0], ppu.palleteTable[pallete_start], ppu.palleteTable[pallete_start + 1], ppu.palleteTable[pallete_start + 2]};
        }

        public static byte[] spritePallete(ref PPU ppu, byte pallete_idx) {
            int start = 0x11 + (pallete_idx * 4);
            return new byte[4] {
                0,
                ppu.palleteTable[start],
                ppu.palleteTable[start + 1],
                ppu.palleteTable[start + 2]
            };
        }

        public static void render(ref PPU ppu, ref Frame frame) {
            // ushort bank = ppu._ctrl.bknd_pattern_address();
            // // Background
            // for (int i = 0; i < 0x3c0; i++)
            // {
            //     ushort tile = (ushort) ppu.vram[i];
            //     ushort tile_column = (ushort) (i % 32);
            //     ushort tile_row = (ushort)(i / 32);
            //     List<byte> tiles = ppu.chrRom.GetRange(bank + tile *16, 16);
            //     byte[] pallete = backgroundPallette(ref ppu, tile_column, tile_row);

            //     for (int y = 0; y <= 7; y++)
            //     {
            //         byte upper = tiles[y];
            //         byte lower = tiles[y + 8];

            //         for (int x = 7; x >= 0; x--)
            //         {
            //             byte value = (byte)((1 & lower) << 1 | (1 & upper));
            //             upper >>= 1;
            //             lower >>= 1;

            //             List<byte> rgb;
            //             switch (value)
            //             {
            //                 case 0:
            //                     rgb = Pallete.SYSTEM_PALLETE[ppu.palleteTable[0]];
            //                     break;
            //                 case 1:
            //                     rgb = Pallete.SYSTEM_PALLETE[pallete[1]];
            //                     break;
            //                 case 2:
            //                     rgb = Pallete.SYSTEM_PALLETE[pallete[2]];
            //                     break;
            //                 case 3:
            //                     rgb = Pallete.SYSTEM_PALLETE[pallete[3]];
            //                     break;
            //                 default:
            //                     throw new Exception("Pallette does not exist.");
            //             }
            //             frame.setPixel(tile_column * 8 + x, tile_row * 8 + y, rgb.ToArray());
            //         }
            //     }
            // }
            ushort scroll_x = ppu._scroll._scrollX;
            ushort scroll_y = ppu._scroll._scrollY;

            byte[] main_nametable; 
            byte[] second_nametable;

            switch((ppu._mirroring, ppu._ctrl.nametable_address())) 
            {
                case (Mirroring.VERTICAL, 0x2000):
                case (Mirroring.VERTICAL, 0x2800):
                case (Mirroring.HORIZONTAL, 0x2000):
                case (Mirroring.HORIZONTAL, 0x2400):
                    main_nametable = ppu.vram[0..0x400];
                    second_nametable = ppu.vram[0x400..0x800];
                    break;
                case (Mirroring.VERTICAL, 0x2400):
                case (Mirroring.VERTICAL, 0x2c00):
                case (Mirroring.HORIZONTAL, 0x2800):
                case (Mirroring.HORIZONTAL, 0x2c00):
                    main_nametable = ppu.vram[0x400..0x800];
                    second_nametable = ppu.vram[0..0x400];
                    break;
                default:
                    throw new Exception(string.Format("Not supported mirroring type {0:?}", ppu._mirroring));
            }

            renderNameTable(ref ppu, ref frame, 
                main_nametable, 
                new Rect(scroll_x, scroll_y, 256, 240),
                -scroll_x, -scroll_y);

            if (scroll_x > 0) {
                renderNameTable(ref ppu, ref frame, 
                second_nametable, 
                new Rect(0, 0, scroll_x, 240),
                (256 - scroll_x), 0);
            } else if (scroll_y > 0) {
                renderNameTable(ref ppu, ref frame, 
                second_nametable, 
                new Rect(0, 0, 256, scroll_y),
                0, 240-scroll_y);
            }

            // Sprites
            for (int i = ppu.oamData.Length - 4; i >= 0 ; i-=4)
            {
                ushort tile_idx = ppu.oamData[i + 1];
                int tile_x = ppu.oamData[i + 3];
                int tile_y = ppu.oamData[i];

                bool flip_vertical = ((ppu.oamData[i + 2] >> 7 & 1) == 1) ? true : false;
                bool flip_horizontal = ((ppu.oamData[i + 2] >> 6 & 1) == 1) ? true : false;
                byte pallete_idx = (byte)(ppu.oamData[i + 2] & 0b11);
                byte[] sprite_pallete = spritePallete(ref ppu, pallete_idx);

                ushort sprite_bank = ppu._ctrl.sprite_pattern_address();

                List<byte> tile = ppu.chrRom.GetRange(sprite_bank + tile_idx * 16, 16);

                for (int y = 0; y <= 7; y++)
                {
                    byte upper = tile[y];
                    byte lower = tile[y + 8];

                    for (int x = 7; x >= 0; x--)
                    {
                        byte value = (byte)((1 & lower) << 1 | (1 & upper));
                        upper >>= 1;
                        lower >>= 1;

                        List<byte> rgb;
                        switch (value)
                        {
                            case 0:
                                continue;
                            case 1:
                                rgb = Pallete.SYSTEM_PALLETE[sprite_pallete[1]];
                                break;
                            case 2:
                                rgb = Pallete.SYSTEM_PALLETE[sprite_pallete[2]];
                                break;
                            case 3:
                                rgb = Pallete.SYSTEM_PALLETE[sprite_pallete[3]];
                                break;
                            default:
                                throw new Exception("Pallette does not exist.");
                        }

                        switch ((flip_horizontal, flip_vertical)) {
                            case (false, false):
                                frame.setPixel((uint)(tile_x + x), (uint)(tile_y + y), rgb.ToArray());
                                break;
                            case (true, false):
                                frame.setPixel((uint)(tile_x + 7 - x), (uint)(tile_y + y), rgb.ToArray());
                                break;
                            case (false, true):
                                frame.setPixel((uint)(tile_x + x), (uint)(tile_y + 7 - y), rgb.ToArray());
                                break;
                            case (true, true):
                                frame.setPixel((uint)(tile_x + 7 - x), (uint)(tile_y + 7 - y), rgb.ToArray());
                                break;
                        }
                    }
                }
            }
        }

        public static void renderNameTable(ref PPU ppu, ref Frame frame, byte[] name_table, Rect view_port, int shift_x, int shift_y) {
            ushort bank = ppu._ctrl.bknd_pattern_address();

            byte[] attribute_table = name_table[0x3c0..0x400];

            for (int i = 0; i < 0x3c0; i++)
            {
                int tile_column = i % 32;
                int tile_row = i / 32;
                ushort tile_idx = (ushort)name_table[i];
                List<byte> tile = ppu.chrRom.GetRange(bank + tile_idx * 16, 16);
                byte[] palette = backgroundPallette(ref ppu, attribute_table, tile_column, tile_row);

                for (int y = 0; y <= 7; y++){
                    byte upper = tile[y];
                    byte lower = tile[y + 8];

                    for (int x = 7; x >= 0; x--)
                    {
                        byte value = (byte)((1 & lower) << 1 | (1 & upper));
                        upper >>= 1;
                        lower >>= 1;

                        List<byte> rgb;
                        switch (value)
                        {
                            case 0:
                                rgb = Pallete.SYSTEM_PALLETE[ppu.palleteTable[0]];
                                break;
                            case 1:
                                rgb = Pallete.SYSTEM_PALLETE[palette[1]];
                                break;
                            case 2:
                                rgb = Pallete.SYSTEM_PALLETE[palette[2]];
                                break;
                            case 3:
                                rgb = Pallete.SYSTEM_PALLETE[palette[3]];
                                break;
                            default:
                                throw new Exception("Pallette does not exist.");
                        }
                        int pixel_x = tile_column * 8 + x;
                        int pixel_y = tile_row * 8 + y; 

                        if (pixel_x >= view_port.x1 && pixel_x < view_port.x2 && pixel_y >= view_port.y1 && pixel_y < view_port.y2) {
                            frame.setPixel((uint)(shift_x + pixel_x), (uint)(shift_y + pixel_y), rgb.ToArray());
                        }
                }
            }
        }
        }
    }

    public class Rect
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;

        public Rect(int x1, int y1, int x2, int y2) {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }
    }

    public class Pallete
    {
        public static readonly List<List<byte>> SYSTEM_PALLETE = new List<List<byte>> {
            new List<byte> {0x80, 0x80, 0x80}, new List<byte> {0x00, 0x3D, 0xA6}, new List<byte> {0x00, 0x12, 0xB0}, new List<byte> {0x44, 0x00, 0x96}, new List<byte> {0xA1, 0x00, 0x5E},
            new List<byte> {0xC7, 0x00, 0x28}, new List<byte> {0xBA, 0x06, 0x00}, new List<byte> {0x8C, 0x17, 0x00}, new List<byte> {0x5C, 0x2F, 0x00}, new List<byte> {0x10, 0x45, 0x00},
            new List<byte> {0x05, 0x4A, 0x00}, new List<byte> {0x00, 0x47, 0x2E}, new List<byte> {0x00, 0x41, 0x66}, new List<byte> {0x00, 0x00, 0x00}, new List<byte> {0x05, 0x05, 0x05},
            new List<byte> {0x05, 0x05, 0x05}, new List<byte> {0xC7, 0xC7, 0xC7}, new List<byte> {0x00, 0x77, 0xFF}, new List<byte> {0x21, 0x55, 0xFF}, new List<byte> {0x82, 0x37, 0xFA},
            new List<byte> {0xEB, 0x2F, 0xB5}, new List<byte> {0xFF, 0x29, 0x50}, new List<byte> {0xFF, 0x22, 0x00}, new List<byte> {0xD6, 0x32, 0x00}, new List<byte> {0xC4, 0x62, 0x00},
            new List<byte> {0x35, 0x80, 0x00}, new List<byte> {0x05, 0x8F, 0x00}, new List<byte> {0x00, 0x8A, 0x55}, new List<byte> {0x00, 0x99, 0xCC}, new List<byte> {0x21, 0x21, 0x21},
            new List<byte> {0x09, 0x09, 0x09}, new List<byte> {0x09, 0x09, 0x09}, new List<byte> {0xFF, 0xFF, 0xFF}, new List<byte> {0x0F, 0xD7, 0xFF}, new List<byte> {0x69, 0xA2, 0xFF},
            new List<byte> {0xD4, 0x80, 0xFF}, new List<byte> {0xFF, 0x45, 0xF3}, new List<byte> {0xFF, 0x61, 0x8B}, new List<byte> {0xFF, 0x88, 0x33}, new List<byte> {0xFF, 0x9C, 0x12},
            new List<byte> {0xFA, 0xBC, 0x20}, new List<byte> {0x9F, 0xE3, 0x0E}, new List<byte> {0x2B, 0xF0, 0x35}, new List<byte> {0x0C, 0xF0, 0xA4}, new List<byte> {0x05, 0xFB, 0xFF},
            new List<byte> {0x5E, 0x5E, 0x5E}, new List<byte> {0x0D, 0x0D, 0x0D}, new List<byte> {0x0D, 0x0D, 0x0D}, new List<byte> {0xFF, 0xFF, 0xFF}, new List<byte> {0xA6, 0xFC, 0xFF},
            new List<byte> {0xB3, 0xEC, 0xFF}, new List<byte> {0xDA, 0xAB, 0xEB}, new List<byte> {0xFF, 0xA8, 0xF9}, new List<byte> {0xFF, 0xAB, 0xB3}, new List<byte> {0xFF, 0xD2, 0xB0},
            new List<byte> {0xFF, 0xEF, 0xA6}, new List<byte> {0xFF, 0xF7, 0x9C}, new List<byte> {0xD7, 0xE8, 0x95}, new List<byte> {0xA6, 0xED, 0xAF}, new List<byte> {0xA2, 0xF2, 0xDA},
            new List<byte> {0x99, 0xFF, 0xFC}, new List<byte> {0xDD, 0xDD, 0xDD}, new List<byte> {0x11, 0x11, 0x11}, new List<byte> {0x11, 0x11, 0x11}
        };
    }

    public class Frame
    {
        const int WIDTH = 256;
        const int HEIGHT = 240;
        public byte[] data;

        public Frame() {
            data = Utility.fillArray(0, WIDTH * HEIGHT * 3);
        }

        public void setPixel(uint x, uint y, byte[] rgb) {
            uint _base = y * 3 * WIDTH + x * 3;
            if (_base + 2 < data.Length) {
                data[_base] = rgb[0];
                data[_base + 1] = rgb[1];
                data[_base + 2] = rgb[2];
            }
        }
    }
    
}