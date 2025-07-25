using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Soundboard.Commands;
using Soundboard.Common;
using Soundboard.Common.Components;
using Soundboard.Services;
using Soundboard.Views;
using MessageBox = System.Windows.MessageBox;

namespace Soundboard.ViewModels;
public class ButtonGridViewModel : BaseViewModel
{
    private readonly IAudioService _audioService;
    private readonly IGlobalHotkeyService _hotkeyService;
    private ObservableCollection<SoundButtonModel> _soundButtons;
    private const int _gridColumns = 5;
    private int _nextHotkeyId = 1;
    private readonly Dictionary<int, SoundButtonModel> _hotkeyToButton = new();

    public ObservableCollection<SoundButtonModel> SoundButtons
    {
        get => _soundButtons;
        set
        {
            _soundButtons = value;
            OnPropertyChanged();
        }
    }

    public int GridColumns => _gridColumns;
    public ICommand SetKeyBindingCommand { get; }

    public ButtonGridViewModel(IAudioService audioService, IGlobalHotkeyService hotkeyService)
    {
        _audioService = audioService;
        _hotkeyService = hotkeyService;
        _soundButtons = new ObservableCollection<SoundButtonModel>();

        SetKeyBindingCommand = new RelayCommand(param => SetKeyBinding(param as SoundButtonModel),
                                               param => CanSetKeyBinding(param as SoundButtonModel));
        InitializeGrid();
    }

    //All this needs to be moved to another VM
    private bool CanSetKeyBinding(SoundButtonModel button)
    {
        return button != null && !button.IsAddButton;
    }

    private void SetKeyBinding(SoundButtonModel button)
    {
        if (button == null || button.IsAddButton) return;

        var dialog = new KeyBindingDialog(button.DisplayText, button.BoundKey, button.ModifierKeys)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };

        if (dialog.ShowDialog() == true)
        {
            // Clear existing binding if any
            ClearKeyBinding(button);

            if (dialog.IsCleared)
            {
                // User chose to clear the binding
                button.BoundKey = null;
                button.ModifierKeys = ModifierKeys.None;
            }
            else if (dialog.SelectedKey.HasValue)
            {
                // Check if key combination is already in use
                var existingButton = SoundButtons.FirstOrDefault(b =>
                    !b.IsAddButton &&
                    b != button &&
                    b.BoundKey == dialog.SelectedKey &&
                    b.ModifierKeys == dialog.SelectedModifiers);

                if (existingButton != null)
                {
                    var result = MessageBox.Show(
                        $"Key combination is already bound to '{existingButton.DisplayText}'.\n\nDo you want to reassign it to '{button.DisplayText}'?",
                        "Key Already Bound",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Clear the existing button's binding
                        ClearKeyBinding(existingButton);
                        existingButton.BoundKey = null;
                        existingButton.ModifierKeys = ModifierKeys.None;
                    }
                    else
                    {
                        return;
                    }
                }

                // Set new binding
                button.BoundKey = dialog.SelectedKey;
                button.ModifierKeys = dialog.SelectedModifiers;

                // Register the hotkey
                RegisterHotkey(button);
            }
        }
    }

    private void RegisterHotkey(SoundButtonModel button)
    {
        if (!button.BoundKey.HasValue) return;

        var hotkeyId = _nextHotkeyId++;
        var winFormsKey = ConvertToWinFormsKey(button.BoundKey.Value);

        var success = _hotkeyService.RegisterHotkey(
            hotkeyId,
            button.ModifierKeys,
            winFormsKey,
            () => _ = PlaySoundAsync(button.FilePath));

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

    private void ClearKeyBinding(SoundButtonModel button)
    {
        var hotkeyId = _hotkeyToButton.FirstOrDefault(kvp => kvp.Value == button).Key;
        if (hotkeyId != 0)
        {
            _hotkeyService.UnregisterHotkey(hotkeyId);
            _hotkeyToButton.Remove(hotkeyId);
        }
    }

    private Keys ConvertToWinFormsKey(Key wpfKey)
    {
//add numpad keys
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

    private void InitializeGrid()
    {
        CreateAddSoundButton();
    }

    private void CreateAddSoundButton()
    {
        //TODO: RESX Files for UI strings.
        var addButton = new SoundButtonModel
        {
            DisplayText = "+ Add Sound",
            IsAddButton = true,
            ButtonBrush = Brushes.LightGreen,
            BorderBrush = Brushes.DarkGreen,
            Command = new RelayCommand(async () => await AddSoundAsync())
        };

        SoundButtons.Add(addButton);
        OnPropertyChanged(nameof(SoundButtons));
    }

    private async Task AddSoundAsync()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select Sound File",
            Filter = "Audio Files (*.mp3;*.wav;*.aiff)|*.mp3;*.wav;*.aiff|All Files (*.*)|*.*",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
        };

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            var filePath = openFileDialog.FileName;
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            var addButton = SoundButtons.FirstOrDefault(b => b.IsAddButton);
            if (addButton != null)
            {
                SoundButtons.Remove(addButton);
            }

            var soundButton = new SoundButtonModel
            {
                DisplayText = fileName,
                FilePath = filePath,
                IsAddButton = false,
                ButtonBrush = Brushes.LightBlue,
                BorderBrush = Brushes.DarkBlue,
                Command = new RelayCommand(async () => await PlaySoundAsync(filePath))
            };

            SoundButtons.Add(soundButton);

            CreateAddSoundButton();
        }
    }

    private async Task PlaySoundAsync(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                await _audioService.PlaySoundAsync(filePath);
            }
            else
            {
                MessageBox.Show($"Sound file not found: {filePath}\n\nSupported formats: MP3, WAV, AIFF");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error playing sound: {ex.Message}");
        }
    }
}