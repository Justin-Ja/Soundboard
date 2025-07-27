using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Soundboard.Services;

public interface IGlobalHotkeyService : IDisposable
{
    bool RegisterHotkey(int id, ModifierKeys modifiers, Keys key, Action callback);
    void UnregisterHotkey(int id);
    void UnregisterAllHotkeys();
}

public class GlobalHotkeyService : IGlobalHotkeyService
{
    private const int WM_HOTKEY = 0x0312;
    private readonly Dictionary<int, Action> hotkeys = new();
    private readonly HotkeyMessageWindow messageWindow;

    //Native windows API functions
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public GlobalHotkeyService()
    {
        messageWindow = new HotkeyMessageWindow(OnHotkeyPressed);
    }

    public bool RegisterHotkey(int id, ModifierKeys modifiers, Keys key, Action callback)
    {
        var mod = ConvertModifierKeys(modifiers);
        var success = RegisterHotKey(messageWindow.Handle, id, mod, (uint)key);

        if (success)
        {
            hotkeys[id] = callback;
        }

        return success;
    }

    public void UnregisterHotkey(int id)
    {
        UnregisterHotKey(messageWindow.Handle, id);
        hotkeys.Remove(id);
    }

    public void UnregisterAllHotkeys()
    {
        foreach (var id in hotkeys.Keys)
        {
            UnregisterHotKey(messageWindow.Handle, id);
        }
        hotkeys.Clear();
    }

    private void OnHotkeyPressed(int id)
    {
        if (hotkeys.TryGetValue(id, out var callback))
        {
            callback?.Invoke();
        }
    }

    private uint ConvertModifierKeys(ModifierKeys modifiers)
    {
        uint mod = 0;
        if (modifiers.HasFlag(ModifierKeys.Alt)) mod |= 0x0001;
        if (modifiers.HasFlag(ModifierKeys.Control)) mod |= 0x0002;
        if (modifiers.HasFlag(ModifierKeys.Shift)) mod |= 0x0004;
        if (modifiers.HasFlag(ModifierKeys.Windows)) mod |= 0x0008;
        return mod;
    }

    public void Dispose()
    {
        UnregisterAllHotkeys();
        messageWindow?.Dispose();
    }

    private class HotkeyMessageWindow : NativeWindow, IDisposable
    {
        private readonly Action<int> _onHotkeyPressed;

        public HotkeyMessageWindow(Action<int> onHotkeyPressed)
        {
            _onHotkeyPressed = onHotkeyPressed;
            CreateHandle(new CreateParams());
        }

        //WM_HOTKEY is a msg ID in windows for hotkey events - whenever one of our registered keys is hit, Windows captures and passes it to the application
        //This method gets that call and passes the hotkey to the Action (onHotkeyPressed), which is pointing to OnHotkeyPressed in GlobalHotkeyService
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WM_HOTKEY)
            {
                _onHotkeyPressed?.Invoke(message.WParam.ToInt32());
            }
            base.WndProc(ref message);
        }

        public void Dispose()
        {
            DestroyHandle();
        }
    }
}

