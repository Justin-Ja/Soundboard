using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NAudio.Wave;
using NAudio;

namespace Soundboard.Services
{
    public class AudioService : IAudioService
    {
        private WaveOutEvent _waveOut;
        private AudioFileReader _audioFile;

        public async Task PlaySoundAsync(string filePath)
        {
            try
            {
                await Task.Run(() =>
                {
                    //TODO: For now no overlapping sounds, may want to make it an option in the future
                    StopCurrentSound();

                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException($"Audio file not found: {filePath}");
                    }

                    _audioFile = new AudioFileReader(filePath);

                    _waveOut = new WaveOutEvent();
                    _waveOut.Init(_audioFile);

                    var completionSource = new TaskCompletionSource<bool>();
                    _waveOut.PlaybackStopped += (sender, e) =>
                    {
                        completionSource.SetResult(true);
                    };

                    _waveOut.Play();

                    completionSource.Task.Wait();
                });
            }
            catch (Exception ex)
            {
                StopCurrentSound();

                // Proper logging at some point? This works for testing for now
                System.Diagnostics.Debug.WriteLine($"Error playing sound: {ex.Message}");
                throw;
            }
        }

        private void StopCurrentSound()
        {
            try
            {
                _waveOut?.Stop();
                _waveOut?.Dispose();
                _waveOut = null;

                _audioFile?.Dispose();
                _audioFile = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping sound: {ex.Message}");
            }
        }

        public void Dispose()
        {
            StopCurrentSound();
        }
    }
}

