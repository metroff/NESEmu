using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SDL2;

namespace NESEmu
{
    class Program
    {
        private const int SCREEN_WIDTH = 32 * 10;
        private const int SCREEN_HEIGHT = 32 * 10;

        static IntPtr gWindow = IntPtr.Zero;

        static IntPtr gRenderer = IntPtr.Zero;

        static IntPtr gTexture = IntPtr.Zero;

        static IntPtr gPixelFormat = IntPtr.Zero;

        static void Main(string[] args) {
            Random rnd = new Random();

            Stopwatch stopWatch = new Stopwatch();

            byte[] game_code = new byte[] {
                0x20, 0x06, 0x06, 0x20, 0x38, 0x06, 0x20, 0x0d, 0x06, 0x20, 0x2a, 0x06, 0x60, 0xa9, 0x02, 0x85,
                0x02, 0xa9, 0x04, 0x85, 0x03, 0xa9, 0x11, 0x85, 0x10, 0xa9, 0x10, 0x85, 0x12, 0xa9, 0x0f, 0x85,
                0x14, 0xa9, 0x04, 0x85, 0x11, 0x85, 0x13, 0x85, 0x15, 0x60, 0xa5, 0xfe, 0x85, 0x00, 0xa5, 0xfe,
                0x29, 0x03, 0x18, 0x69, 0x02, 0x85, 0x01, 0x60, 0x20, 0x4d, 0x06, 0x20, 0x8d, 0x06, 0x20, 0xc3,
                0x06, 0x20, 0x19, 0x07, 0x20, 0x20, 0x07, 0x20, 0x2d, 0x07, 0x4c, 0x38, 0x06, 0xa5, 0xff, 0xc9,
                0x77, 0xf0, 0x0d, 0xc9, 0x64, 0xf0, 0x14, 0xc9, 0x73, 0xf0, 0x1b, 0xc9, 0x61, 0xf0, 0x22, 0x60,
                0xa9, 0x04, 0x24, 0x02, 0xd0, 0x26, 0xa9, 0x01, 0x85, 0x02, 0x60, 0xa9, 0x08, 0x24, 0x02, 0xd0,
                0x1b, 0xa9, 0x02, 0x85, 0x02, 0x60, 0xa9, 0x01, 0x24, 0x02, 0xd0, 0x10, 0xa9, 0x04, 0x85, 0x02,
                0x60, 0xa9, 0x02, 0x24, 0x02, 0xd0, 0x05, 0xa9, 0x08, 0x85, 0x02, 0x60, 0x60, 0x20, 0x94, 0x06,
                0x20, 0xa8, 0x06, 0x60, 0xa5, 0x00, 0xc5, 0x10, 0xd0, 0x0d, 0xa5, 0x01, 0xc5, 0x11, 0xd0, 0x07,
                0xe6, 0x03, 0xe6, 0x03, 0x20, 0x2a, 0x06, 0x60, 0xa2, 0x02, 0xb5, 0x10, 0xc5, 0x10, 0xd0, 0x06,
                0xb5, 0x11, 0xc5, 0x11, 0xf0, 0x09, 0xe8, 0xe8, 0xe4, 0x03, 0xf0, 0x06, 0x4c, 0xaa, 0x06, 0x4c,
                0x35, 0x07, 0x60, 0xa6, 0x03, 0xca, 0x8a, 0xb5, 0x10, 0x95, 0x12, 0xca, 0x10, 0xf9, 0xa5, 0x02,
                0x4a, 0xb0, 0x09, 0x4a, 0xb0, 0x19, 0x4a, 0xb0, 0x1f, 0x4a, 0xb0, 0x2f, 0xa5, 0x10, 0x38, 0xe9,
                0x20, 0x85, 0x10, 0x90, 0x01, 0x60, 0xc6, 0x11, 0xa9, 0x01, 0xc5, 0x11, 0xf0, 0x28, 0x60, 0xe6,
                0x10, 0xa9, 0x1f, 0x24, 0x10, 0xf0, 0x1f, 0x60, 0xa5, 0x10, 0x18, 0x69, 0x20, 0x85, 0x10, 0xb0,
                0x01, 0x60, 0xe6, 0x11, 0xa9, 0x06, 0xc5, 0x11, 0xf0, 0x0c, 0x60, 0xc6, 0x10, 0xa5, 0x10, 0x29,
                0x1f, 0xc9, 0x1f, 0xf0, 0x01, 0x60, 0x4c, 0x35, 0x07, 0xa0, 0x00, 0xa5, 0xfe, 0x91, 0x00, 0x60,
                0xa6, 0x03, 0xa9, 0x00, 0x81, 0x10, 0xa2, 0x00, 0xa9, 0x01, 0x81, 0x10, 0x60, 0xa2, 0x00, 0xea,
                0xea, 0xca, 0xd0, 0xfb, 0x60
            };

            byte[] romFile = File.ReadAllBytes("../nestest.nes");
            Rom rom = new Rom(romFile);
            Bus bus = new Bus(rom);
            CPU cpu = new CPU(bus);
            // cpu.load(game_code);
            cpu.reset();
            cpu.PC = 0xc000;

            if (!initSDL()) {
                Console.WriteLine("Failed to initialize!");
                return;
            }
           
            uint[] screenState = new uint[32*32];
            GCHandle pinnedArray = GCHandle.Alloc(screenState, GCHandleType.Pinned);
            IntPtr screenStatePtr = pinnedArray.AddrOfPinnedObject();

            stopWatch.Start();

            bool quit = false;
            while(!quit) {
                stopWatch.Start();
                if (stopWatch.Elapsed.TotalMilliseconds > 0.12)
                {
                stopWatch.Reset();
                
                // Get user input
                SDL.SDL_Event e;
                while( SDL.SDL_PollEvent(out e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        quit = true;
                    }
                    else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN) {
                        switch (e.key.keysym.sym) {
                            case SDL.SDL_Keycode.SDLK_w:
                                bus.memoryWrite(0xff, 0x77);
                                break;
                            case SDL.SDL_Keycode.SDLK_s:
                                bus.memoryWrite(0xff, 0x73);
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                bus.memoryWrite(0xff, 0x61);
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                bus.memoryWrite(0xff, 0x64);
                                break;
                        }
                    }
                }

                //Random apple color
                // bus.memoryWrite(0xfe, (byte) rnd.Next(1, 16));

                // if (readScreenState(ref bus, ref screenState)) {
                //     // for (int i = 0; i < 32*32; i++)
                //     // {
                //     //     Console.Write(screenState[i] + " ");
                //     // }
                //     //Console.WriteLine();
                //     SDL.SDL_UpdateTexture(gTexture, IntPtr.Zero, screenStatePtr, 32*4);
                //     SDL.SDL_RenderClear(gRenderer);
                //     SDL.SDL_RenderCopy(gRenderer, gTexture, IntPtr.Zero, IntPtr.Zero);
                //     SDL.SDL_RenderPresent(gRenderer);
                // }

                

                cpu.clock();

                stopWatch.Start();
                }
            }

            Marshal.FreeHGlobal(screenStatePtr);
            close();
        }

        static bool initSDL() {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0) {
                Console.WriteLine("SDL could not initialize! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }

            gWindow = SDL.SDL_CreateWindow("Snake", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (gWindow == IntPtr.Zero) {
                Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }

            gRenderer = SDL.SDL_CreateRenderer(gWindow, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (gRenderer == IntPtr.Zero) {
                Console.WriteLine("Renderer could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }
            SDL.SDL_RenderSetScale(gRenderer, 10, 10);

            gTexture = SDL.SDL_CreateTexture(gRenderer, SDL.SDL_PIXELFORMAT_RGB888, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 32, 32);
            if (gTexture == IntPtr.Zero) {
                Console.WriteLine("Texture could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }

            uint format;
            SDL.SDL_QueryTexture(gTexture, out format, out _, out _, out _);
            // Console.WriteLine(format + ",  " + SDL.SDL_PIXELFORMAT_RGB24);
            gPixelFormat = SDL.SDL_AllocFormat(SDL.SDL_PIXELFORMAT_RGB888);

            return true;
        }

        static void close() {
            SDL.SDL_FreeFormat(gPixelFormat);
            gPixelFormat = IntPtr.Zero;
            
            SDL.SDL_DestroyTexture(gTexture);
            gTexture = IntPtr.Zero;

            SDL.SDL_DestroyRenderer(gRenderer);
            gRenderer = IntPtr.Zero;

            SDL.SDL_DestroyWindow(gWindow);
            gWindow = IntPtr.Zero;

            SDL.SDL_Quit();
        }

        static uint color(byte b) {
            uint color;
            switch (b) {
                case 0:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0x00, 0x00, 0x00);
                    break;
                case 1:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0xff, 0xff, 0xff);
                    break;
                case 2:
                case 9:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0x80, 0x80, 0x80);
                    break;
                case 3:
                case 10:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0xff, 0x00, 0x00);
                    break;
                case 4:
                case 11:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0x00, 0xff, 0x00);
                    break;
                case 5:
                case 12:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0x00, 0x00, 0xff);
                    break;
                case 6:
                case 13:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0xff, 0x00, 0xff);
                    break;
                case 7:
                case 14:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0xff, 0xff, 0x00);
                    break;
                default:
                    color = SDL.SDL_MapRGB(gPixelFormat, 0x00, 0xff, 0xff);
                    break;
            }
            return color;
        }

        static bool readScreenState(ref Bus bus, ref uint[] frame) {
            int frame_idx = 0;
            bool update = false;
            for (ushort i = 0x0200; i < 0x600; i++)
            {
                byte color_idx = bus.memoryRead(i);
                uint convertedColor = color(color_idx);
                if (frame[frame_idx] != convertedColor) {
                    frame[frame_idx] = convertedColor;
                    update = true;
                }
                frame_idx += 1;
            }
            return update;
        }
    }
}
