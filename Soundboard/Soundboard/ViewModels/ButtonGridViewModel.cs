using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Soundboard.Commands;
using Soundboard.Common;
using Soundboard.Common.Components;
using Soundboard.Services;

namespace Soundboard.ViewModels;
public class ButtonGridViewModel : BaseViewModel
{
    private readonly IAudioService _audioService;
    private ObservableCollection<SoundButtonModel> _soundButtons;
    private const int _gridColumns = 5; 

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
    }

    public ButtonGridViewModel(IAudioService audioService)
    {
        _audioService = audioService;
        _soundButtons = new ObservableCollection<SoundButtonModel>();
        InitializeGrid();
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