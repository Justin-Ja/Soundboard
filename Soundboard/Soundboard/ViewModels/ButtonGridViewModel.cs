using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Soundboard.Commands;
using Soundboard.Common;
using Soundboard.Common.Components;
using Soundboard.Domain.DataAccess.Implementations;
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
    private SoundButtonGridLayout _currentGrid;

    public SoundButtonGridLayout CurrentGrid
    {
        get => _currentGrid;
        private set => SetProperty(ref _currentGrid, value);
    }

//TODO: With the addition of SoundButtonGridLayout, this OC may be redundant
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
                filePath : filePath,
                isAdd : false,
                buttonBrush : Brushes.LightBlue,
                buttonBorderBrush : Brushes.DarkBlue,
                command : new RelayCommand(async () => await PlaySoundAsync(filePath))
            );
            soundButton.SetKeyBindingCommand = new RelayCommand(() => SetKeyBinding(soundButton));

            SoundButtons.Add(soundButton);

            CreateAddSoundButton();

            //If we have a current grid, add the button to it (This is for DB purposes, above is for UI purposes)
            if (CurrentGrid != null)
            {
                var domainButton = new SoundButton
                {
                    Guid = Guid.NewGuid(),
                    Name = fileName,
                    FilePath = filePath,
                    GridId = CurrentGrid.Guid
                };

                CurrentGrid.SoundButtons.Add(domainButton);
            }
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

    public void HandleGridChanged(SoundButtonGridLayout grid, string operation)
    {
        CurrentGrid = grid;
        LoadGridButtons(grid);
    }

    private void LoadGridButtons(SoundButtonGridLayout grid)
    {
        _hotkeyManager.ClearAllHotkeys(); //Loading a new grid will register the needed keys later in this function so clear everything for now
        SoundButtons.Clear();

        if (grid?.SoundButtons != null)
        {
            foreach (var soundButton in grid.SoundButtons)
            {
                var buttonModel = new SoundButtonModel(
                    displayText: soundButton.Name,
                    filePath: soundButton.FilePath,
                    isAdd: false,
                    buttonBrush: Brushes.LightBlue,
                    buttonBorderBrush: Brushes.DarkBlue,
                    command: new RelayCommand(async () => await PlaySoundAsync(soundButton.FilePath))
                );

                //Restore hotkey if it exists - convert back from int to WPF types
                if (soundButton.BoundKeyCode.HasValue && soundButton.BoundKeyModifiers.HasValue)
                {
                    buttonModel.BoundKey = (Key)soundButton.BoundKeyCode.Value;
                    buttonModel.ModifierKeys = (ModifierKeys)soundButton.BoundKeyModifiers.Value;

                    //Re-register the hotkey with the hotkey manager
                    _hotkeyManager.RegisterHotkey(buttonModel, () => _ = PlaySoundAsync(soundButton.FilePath));
                }

                buttonModel.SetKeyBindingCommand = new RelayCommand(() => SetKeyBinding(buttonModel));
                SoundButtons.Add(buttonModel);
            }
        }

        //Always add the "Add" button at the end
        CreateAddSoundButton();
    }

    public List<SoundButton> GetCurrentGridData()
    {
        var soundButtons = new List<SoundButton>();

        foreach (var buttonModel in SoundButtons.Where(b => !b.IsAddButton))
        {
            var soundButton = new SoundButton
            {
                Guid = Guid.NewGuid(),
                Name = buttonModel.DisplayText,
                FilePath = buttonModel.FilePath,
                GridId = CurrentGrid?.Guid ?? Guid.Empty
            };

            //Save hotkey information - convert WPF types to int
            if (buttonModel.BoundKey.HasValue)
            {
                soundButton.BoundKeyCode = (int)buttonModel.BoundKey.Value;
                soundButton.BoundKeyModifiers = (int)buttonModel.ModifierKeys;
            }

            soundButtons.Add(soundButton);
        }

        return soundButtons;
    }

    //Called in the DI Container
    public void SubscribeToCrudEvents(CrudToolbarViewModel crudViewModel)
    {
        crudViewModel.GridChanged += (sender, e) => HandleGridChanged(e.Grid, e.Operation);
        crudViewModel.GridDataRequested += (sender, e) => e.GridData = GetCurrentGridData();
    }
}