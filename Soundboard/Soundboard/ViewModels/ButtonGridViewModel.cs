using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Soundboard.Commands;
using Soundboard.Common.Components;
using Soundboard.Services;

namespace Soundboard.ViewModels;
public class ButtonGridViewModel : INotifyPropertyChanged
{
    private readonly IAudioService _audioService;
    private ObservableCollection<SoundButtonModel> _soundButtons;
    private const int _gridColumns = 5; // TODO: Is there a better way in C# for global consts? gotta be

    public ObservableCollection<SoundButtonModel> SoundButtons
    {
        get => _soundButtons;
        set
        {
            _soundButtons = value;
            OnPropertyChanged();
        }
    }

    public int GridColumns
    {
        get => _gridColumns;
        //set
        //{
        //    _gridColumns = value;
        //    OnPropertyChanged();
        //}
    }

    //public ButtonGridViewModel()
    //{
    //    // For now, we'll use a mock audio service if none is provided
    //    _audioService = new AudioService();

    //    SoundButtons = new ObservableCollection<SoundButtonModel>();
    //    InitializeGrid();
    //}

    public ButtonGridViewModel(IAudioService audioService)
    {
        _audioService = audioService;
        _soundButtons = new ObservableCollection<SoundButtonModel>();
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Start with just the "Add Sound" button
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

            // Remove the current add button
            var addButton = SoundButtons.FirstOrDefault(b => b.IsAddButton);
            if (addButton != null)
            {
                SoundButtons.Remove(addButton);
            }

            // Create a new sound button
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

            // Add a new "Add Sound" button at the end
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


    //Is it possible to move this property thing to a super class for VMs? probably :/
    public event PropertyChangedEventHandler PropertyChanged;

    
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}