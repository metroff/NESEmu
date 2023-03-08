using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SDL2;

namespace NESEmu
{
    class Program
    {
        const int SCREEN_WIDTH = 256 * 3;
        const int SCREEN_HEIGHT = 240 * 3;

        static IntPtr gWindow = IntPtr.Zero;

        static IntPtr gRenderer = IntPtr.Zero;

        static IntPtr gTexture = IntPtr.Zero;

        static IntPtr gPixelFormat = IntPtr.Zero;
        static IntPtr droppedFile = IntPtr.Zero;

        Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args) {
            Random rnd = new Random();
            string filename = "";

            Stopwatch stopWatch = new Stopwatch();

            if (!initSDL()) {
                Console.WriteLine("Failed to initialize!");
                return;
            }

            while(filename == "") {
                SDL.SDL_Event e;
                while( SDL.SDL_PollEvent(out e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        System.Environment.Exit(1);
                    }
                    else if (e.type == SDL.SDL_EventType.SDL_DROPFILE) {
                        filename = SDL.UTF8_ToManaged(e.drop.file, true);
                    }
                }
            }

            Console.WriteLine("{0}", filename);

            byte[] romFile = File.ReadAllBytes(filename);
            Rom rom = new Rom(romFile);

            Frame frame = new Frame();

            GCHandle pinnedArray = GCHandle.Alloc(frame.data, GCHandleType.Pinned);
            IntPtr frame_ptr = pinnedArray.AddrOfPinnedObject();

            Dictionary<SDL.SDL_Keycode, Joypad.JoypadButton> key_map = new Dictionary<SDL.SDL_Keycode, Joypad.JoypadButton>();
            key_map.Add(SDL.SDL_Keycode.SDLK_DOWN, Joypad.JoypadButton.DOWN);
            key_map.Add(SDL.SDL_Keycode.SDLK_UP, Joypad.JoypadButton.UP);
            key_map.Add(SDL.SDL_Keycode.SDLK_RIGHT, Joypad.JoypadButton.RIGHT);
            key_map.Add(SDL.SDL_Keycode.SDLK_LEFT, Joypad.JoypadButton.LEFT);
            key_map.Add(SDL.SDL_Keycode.SDLK_SPACE, Joypad.JoypadButton.SELECT);
            key_map.Add(SDL.SDL_Keycode.SDLK_RETURN, Joypad.JoypadButton.START);
            key_map.Add(SDL.SDL_Keycode.SDLK_a, Joypad.JoypadButton.BUTTON_A);
            key_map.Add(SDL.SDL_Keycode.SDLK_s, Joypad.JoypadButton.BUTTON_B);


            bool exit = false;

            Bus.gameloopDel callback = delegate(ref PPU ppu, ref Joypad joypad) {
                Renderer.render(ref ppu, ref frame);

                SDL.SDL_UpdateTexture(gTexture, IntPtr.Zero, frame_ptr, 256*3);

                SDL.SDL_RenderClear(gRenderer);
                SDL.SDL_RenderCopy(gRenderer, gTexture, IntPtr.Zero, IntPtr.Zero);
                SDL.SDL_RenderPresent(gRenderer);

                SDL.SDL_Event e;
                while( SDL.SDL_PollEvent(out e) != 0) {
                    if (e.type == SDL.SDL_EventType.SDL_QUIT) {
                        exit = true;
                    }
                    else if (e.type == SDL.SDL_EventType.SDL_KEYDOWN) {
                        Joypad.JoypadButton key;
                        if (key_map.TryGetValue(e.key.keysym.sym, out key)) {
                            joypad.setButtonPressedStatus(key, true);
                        }
                    }
                    else if (e.type == SDL.SDL_EventType.SDL_KEYUP) {
                        Joypad.JoypadButton key;
                        if (key_map.TryGetValue(e.key.keysym.sym, out key)) {
                            joypad.setButtonPressedStatus(key, false);
                        }
                    }
                }
            };

            Bus bus = new Bus(rom, callback);

            CPU cpu = new CPU(bus);
            cpu.reset();

            int cyclesLeft = 0;

            stopWatch.Start();

            while (!exit) {
                if (stopWatch.Elapsed.TotalMilliseconds >= 4) { // 16: 28830, 8: 14415, 4: 7207
                    stopWatch.Reset();
                    stopWatch.Start();
                    cyclesLeft += 7207;
                }
                while(cyclesLeft > 0)
                {
                    cyclesLeft -= (int) cpu.clock();
                }

            } 

            Marshal.FreeHGlobal(frame_ptr);
            close();
        }

        static bool initSDL() {
            if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) < 0) {
                Console.WriteLine("SDL could not initialize! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }

            gWindow = SDL.SDL_CreateWindow("NESEmu", SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL.SDL_WindowFlags.SDL_WINDOW_SHOWN);
            if (gWindow == IntPtr.Zero) {
                Console.WriteLine("Window could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }

            gRenderer = SDL.SDL_CreateRenderer(gWindow, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
            if (gRenderer == IntPtr.Zero) {
                Console.WriteLine("Renderer could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }

            gTexture = SDL.SDL_CreateTexture(gRenderer, SDL.SDL_PIXELFORMAT_RGB24, (int)SDL.SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, 256, 240);
            if (gTexture == IntPtr.Zero) {
                Console.WriteLine("Texture could not be created! SDL_Error: {0}", SDL.SDL_GetError());
                return false;
            }

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
    }
}
