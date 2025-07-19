using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Soundboard.Commands;
using Soundboard.Common;
using Soundboard.Services;

namespace Soundboard.ViewModels;

//TODO: move this file a level up
public class MainViewModel : BaseViewModel
{
    private readonly IAudioService _audioService;
    private readonly string baseDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.Parent.FullName;
    private string _soundFilePath;

    public ICommand PlaySoundCommand { get; }
    public event PropertyChangedEventHandler PropertyChanged;

    public string SoundButtonText => "Play Sound";

    public string SoundFilePath
    {
        get => _soundFilePath;
        set => SetProperty(ref _soundFilePath, value);
    }

    //TODO: A lot of this is old code to test service, and is outdated. See if its still used and remove
    public MainViewModel(IAudioService audioService)
    {
        _audioService = audioService;


        _soundFilePath = Path.Combine(baseDir, "Sounds", "sample");

        if (!File.Exists(_soundFilePath))
        {
            _soundFilePath = Path.Combine(baseDir, "Sounds", "sample.mp3");
        }

        PlaySoundCommand = new RelayCommand(async () => await PlaySound(), CanPlaySound);
    }

    private async Task PlaySound()
    {
        try
        {
            if (File.Exists(SoundFilePath))
            {
                await _audioService.PlaySoundAsync(SoundFilePath);
            }
            else
            {
                MessageBox.Show($"Sound file not found: {SoundFilePath}\n\nSupported formats: MP3, WAV, AIFF");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error playing sound: {ex.Message}");
        }
    }

    private bool CanPlaySound()
    {
        return !string.IsNullOrEmpty(SoundFilePath);
    }
}


