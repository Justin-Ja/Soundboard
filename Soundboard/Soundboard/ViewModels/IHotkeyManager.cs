using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Soundboard.Common.Components;
using Soundboard.Services;
using Soundboard.Views;
using MessageBox = System.Windows.MessageBox;

namespace Soundboard.ViewModels;

public interface IHotkeyManager
{
    void SetKeyBinding(SoundButtonModel button, IEnumerable<SoundButtonModel> soundButtons);

    bool CanSetKeyBinding(SoundButtonModel button);

    void RegisterHotkey(SoundButtonModel button, Action onHotkeyPressed);

    void ClearKeyBinding(SoundButtonModel button);

    void ClearAllHotkeys();
}

public class HotkeyManager : IHotkeyManager
{
    private readonly IGlobalHotkeyService _hotkeyService;
    private readonly Dictionary<int, SoundButtonModel> _hotkeyToButton = new();
    private int _nextHotkeyId = 1;

    public HotkeyManager(IGlobalHotkeyService hotkeyService)
    {
        _hotkeyService = hotkeyService;
    }

    public bool CanSetKeyBinding(SoundButtonModel button)
    {
        return button != null && !button.IsAddButton;
    }

    public void SetKeyBinding(SoundButtonModel button, IEnumerable<SoundButtonModel> soundButtons)
    {
        if (button == null || button.IsAddButton) return;

        var dialog = new KeyBindingDialog(button.DisplayText, button.BoundKey, button.ModifierKeys)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        var result = dialog.ShowDialog();
        if (result == true)
        {
            ClearKeyBinding(button);

            if (dialog.IsCleared)
            {
                button.BoundKey = null;
                button.ModifierKeys = ModifierKeys.None;
            }
            else if (dialog.SelectedKey.HasValue)
            {
                //Check if key combination is already in use
                var existingButton = soundButtons.FirstOrDefault(b =>
                    !b.IsAddButton &&
                    b != button &&
                    b.BoundKey == dialog.SelectedKey &&
                    b.ModifierKeys == dialog.SelectedModifiers);

                if (existingButton != null)
                {
                    var confirmResult = MessageBox.Show(
                        $"Key combination is already bound to '{existingButton.DisplayText}'.\n\nDo you want to reassign it to '{button.DisplayText}'?",
                        "Key Already Bound",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (confirmResult == MessageBoxResult.Yes)
                    {
                        ClearKeyBinding(existingButton);
                        existingButton.BoundKey = null;
                        existingButton.ModifierKeys = ModifierKeys.None;
                    }
                    else
                    {
                        return;
                    }
                }

                button.BoundKey = dialog.SelectedKey;
                button.ModifierKeys = dialog.SelectedModifiers;
            }
        }
    }

    public void RegisterHotkey(SoundButtonModel button, Action onHotkeyPressed)
    {
        if (!button.BoundKey.HasValue) return;

        var hotkeyId = _nextHotkeyId++;
        var winFormsKey = ConvertToWinFormsKey(button.BoundKey.Value);

        var success = _hotkeyService.RegisterHotkey(
            hotkeyId,
            button.ModifierKeys,
            winFormsKey,
            onHotkeyPressed);

        if (success)
        {
            _hotkeyToButton[hotkeyId] = button;
        }
        else
        {
            MessageBox.Show(
                $"Failed to register hotkey '{button.KeyBindingDisplay}'. It may already be in use by another application.",
                "Hotkey Registration Failed",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            button.BoundKey = null;
            button.ModifierKeys = ModifierKeys.None;
        }
    }

    public void ClearKeyBinding(SoundButtonModel button)
    {
        var hotkeyId = _hotkeyToButton.FirstOrDefault(kvp => kvp.Value == button).Key;
        if (hotkeyId != 0)
        {
            _hotkeyService.UnregisterHotkey(hotkeyId);
            _hotkeyToButton.Remove(hotkeyId);
        }
    }

    public void ClearAllHotkeys()
    {
        foreach (var kvp in _hotkeyToButton.ToList())
        {
            _hotkeyService.UnregisterHotkey(kvp.Key);
        }
        _hotkeyToButton.Clear();
    }

    private Keys ConvertToWinFormsKey(Key wpfKey)
    {
        return wpfKey switch
        {
            Key.A => Keys.A,
            Key.B => Keys.B,
            Key.C => Keys.C,
            Key.D => Keys.D,
            Key.E => Keys.E,
            Key.F => Keys.F,
            Key.G => Keys.G,
            Key.H => Keys.H,
            Key.I => Keys.I,
            Key.J => Keys.J,
            Key.K => Keys.K,
            Key.L => Keys.L,
            Key.M => Keys.M,
            Key.N => Keys.N,
            Key.O => Keys.O,
            Key.P => Keys.P,
            Key.Q => Keys.Q,
            Key.R => Keys.R,
            Key.S => Keys.S,
            Key.T => Keys.T,
            Key.U => Keys.U,
            Key.V => Keys.V,
            Key.W => Keys.W,
            Key.X => Keys.X,
            Key.Y => Keys.Y,
            Key.Z => Keys.Z,
            Key.D0 => Keys.D0,
            Key.D1 => Keys.D1,
            Key.D2 => Keys.D2,
            Key.D3 => Keys.D3,
            Key.D4 => Keys.D4,
            Key.D5 => Keys.D5,
            Key.D6 => Keys.D6,
            Key.D7 => Keys.D7,
            Key.D8 => Keys.D8,
            Key.D9 => Keys.D9,
            Key.NumPad0 => Keys.NumPad0,
            Key.NumPad1 => Keys.NumPad1,
            Key.NumPad2 => Keys.NumPad2,
            Key.NumPad3 => Keys.NumPad3,
            Key.NumPad4 => Keys.NumPad4,
            Key.NumPad5 => Keys.NumPad5,
            Key.NumPad6 => Keys.NumPad6,
            Key.NumPad7 => Keys.NumPad7,
            Key.NumPad8 => Keys.NumPad8,
            Key.NumPad9 => Keys.NumPad9,
            Key.Decimal => Keys.Decimal,
            Key.Add => Keys.Add,
            Key.Subtract => Keys.Subtract,
            Key.Multiply => Keys.Multiply,
            Key.Divide => Keys.Divide,
            Key.F1 => Keys.F1,
            Key.F2 => Keys.F2,
            Key.F3 => Keys.F3,
            Key.F4 => Keys.F4,
            Key.F5 => Keys.F5,
            Key.F6 => Keys.F6,
            Key.F7 => Keys.F7,
            Key.F8 => Keys.F8,
            Key.F9 => Keys.F9,
            Key.F10 => Keys.F10,
            Key.F11 => Keys.F11,
            Key.F12 => Keys.F12,
            Key.Space => Keys.Space,
            Key.Enter => Keys.Enter,
            Key.Escape => Keys.Escape,
            _ => Keys.None
        };
    }
}
