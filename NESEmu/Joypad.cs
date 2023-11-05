using SDL2;

namespace NESEmu
{
    public abstract class Joypad
    {
        protected enum JoypadButton {
            RIGHT = (1 << 7),   
            LEFT = (1 << 6),  
            DOWN = (1 << 5),  
            UP = (1 << 4), 
            START = (1 << 3),
            SELECT = (1 << 2),  
            BUTTON_A = (1 << 1),
            BUTTON_B = (1 << 0), 
        };

        byte button_status;
        bool strobe;
        byte button_index;

        public abstract void keyUp(SDL.SDL_Keycode code);
        public abstract void keyDown(SDL.SDL_Keycode code);

        void setFlag(JoypadButton flag, bool v) {
            if (v)
                button_status |= (byte) flag;
            else
                button_status &= unchecked((byte) ~flag);
        }

        byte getFlag(JoypadButton flag) {
            return (byte) ((button_status & (byte) flag) == 0 ? 0 : 1);
        }

        public Joypad() {
            button_status = 0;
            strobe = false;
            button_index = 0;
        }

        public void write(byte data) {
            strobe = (data & 1) == 1;
            if (strobe) {
                button_index = 0;
            }
        }

        public byte read() {
            if (button_index > 7) {
                return 1;
            }
            byte response = (byte)((button_status & (1 << button_index)) >> button_index);
            if (!strobe && button_index <= 7) {
                button_index += 1;
            }
            return response;
        }

        protected void setButtonPressedStatus(JoypadButton button, bool pressed) {
            setFlag(button, pressed);
        }
    }

    class SDLKeyboard: Joypad {
        Dictionary<SDL.SDL_Keycode, JoypadButton> key_map;

        public SDLKeyboard() {
            key_map = new Dictionary<SDL.SDL_Keycode, JoypadButton>
            {
                { SDL.SDL_Keycode.SDLK_DOWN, JoypadButton.DOWN },
                { SDL.SDL_Keycode.SDLK_UP, JoypadButton.UP },
                { SDL.SDL_Keycode.SDLK_RIGHT, JoypadButton.RIGHT },
                { SDL.SDL_Keycode.SDLK_LEFT, JoypadButton.LEFT },
                { SDL.SDL_Keycode.SDLK_SPACE, JoypadButton.SELECT },
                { SDL.SDL_Keycode.SDLK_RETURN, JoypadButton.START },
                { SDL.SDL_Keycode.SDLK_a, JoypadButton.BUTTON_A },
                { SDL.SDL_Keycode.SDLK_s, JoypadButton.BUTTON_B }
            };
        }

        public override void keyUp(SDL.SDL_Keycode code)
        {
            JoypadButton key;
            if (key_map.TryGetValue(code, out key)) {
                setButtonPressedStatus(key, true);
            }
        }

        public override void keyDown(SDL.SDL_Keycode code)
        {
            JoypadButton key;
            if (key_map.TryGetValue(code, out key)) {
                setButtonPressedStatus(key, false);
            }
        }
    }
}