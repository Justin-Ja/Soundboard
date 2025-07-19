namespace Soundboard.Services;

public interface IAudioService : IDisposable
{
    Task PlaySoundAsync(string filePath);
}


