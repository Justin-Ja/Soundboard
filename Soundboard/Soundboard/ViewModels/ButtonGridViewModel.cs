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
    private readonly IHotkeyManager _hotkeyManager;
    private ObservableCollection<SoundButtonModel> _soundButtons;
    private const int _gridColumns = 5;

    public ObservableCollection<SoundButtonModel> SoundButtons
    {
        get => _soundButtons;
        set => SetProperty(ref _soundButtons, value);
    }

    public int GridColumns => _gridColumns;
    public ICommand SetKeyBindingCommand { get; }

    public ButtonGridViewModel(IAudioService audioService, IHotkeyManager hotkeyManagerService)
    {
        _audioService = audioService;
        _hotkeyManager = hotkeyManagerService;
        _soundButtons = new ObservableCollection<SoundButtonModel>();

        SetKeyBindingCommand = new RelayCommand(param => SetKeyBinding(param as SoundButtonModel),
                                               param => CanSetKeyBinding(param as SoundButtonModel));
        InitializeGrid();
    }

    private bool CanSetKeyBinding(SoundButtonModel button)
    {
        return _hotkeyManager.CanSetKeyBinding(button);
    }

    private void SetKeyBinding(SoundButtonModel button)
    {
        _hotkeyManager.SetKeyBinding(button, SoundButtons);

        // If a key was successfully bound, register the hotkey
        if (button.BoundKey.HasValue)
        {
            _hotkeyManager.RegisterHotkey(button, () => _ = PlaySoundAsync(button.FilePath));
        }
    }
    private void InitializeGrid()
    {
        CreateAddSoundButton();
    }

    private void CreateAddSoundButton()
    {
        //TODO: RESX Files for UI strings.
        var addButton = new SoundButtonModel(
            displayText: "+ Add Sound",
            isAdd: true,
            buttonBrush: Brushes.LightGreen,
            buttonBorderBrush: Brushes.DarkGreen,
            command: new RelayCommand(async () => await AddSoundAsync())
        );

        SoundButtons.Add(addButton);
        OnPropertyChanged(nameof(SoundButtons));
    }

    //TODO: Check if we need async, not for this function but for the above relayCommands
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
            (
                displayText: fileName,
                FilePath : filePath,
                isAdd : false,
                buttonBrush : Brushes.LightBlue,
                buttonBorderBrush : Brushes.DarkBlue,
                command : new RelayCommand(async () => await PlaySoundAsync(filePath))
            );
            soundButton.SetKeyBindingCommand = new RelayCommand(() => SetKeyBinding(soundButton));

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