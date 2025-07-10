using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using NAudio.Wave;
using Soundboard;
using Soundboard.Commands;
using Soundboard.Services;

namespace Soundboard.ViewModels;

//TODO: move this file a level up.
public class MainViewModel : INotifyPropertyChanged
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
        set
        {
            _soundFilePath = value;
            OnPropertyChanged();
        }
    }

    public MainViewModel(IAudioService audioService)
    {
        _audioService = audioService;

        // TODO: Hardcoded path for now - probably won't change unless I want people to be able to pick out files specifically...
        // Testing with MP3 for now, but NAudio does support wav and others
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

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}


