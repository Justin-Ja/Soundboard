using System.IO;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Soundboard.Services;

public interface IAudioService : IDisposable
{
    Task PlaySoundAsync(string filePath);
}

public class AudioService : IAudioService
{
    private WasapiOut _waveOutSpeaker;
    private AudioFileReader _audioFileSpeaker;
    private WasapiOut _waveOutMic;
    private AudioFileReader _audioFileMic;
    private readonly int latencyInMs = 50;

    public async Task PlaySoundAsync(string filePath)
    {
        try
        {
            await Task.Run(() =>
            {
                StopCurrentSound();

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Audio file not found: {filePath}");

                //Check if VoiceMeeter is running and find its devices
                var voiceMeeterAuxDevice = FindVoiceMeeterDevice("Aux"); //For speakers/headphones
                var voiceMeeterVaioDevice = FindVoiceMeeterDevice("VAIO"); //For microphone

                if (voiceMeeterAuxDevice != null || voiceMeeterVaioDevice != null)
                {
                    //VoiceMeeter is available - use it exclusively
                    System.Diagnostics.Debug.WriteLine("VoiceMeeter detected - using VoiceMeeter routing");

                    if (voiceMeeterAuxDevice != null)
                    {
                        _audioFileSpeaker = new AudioFileReader(filePath);
                        _waveOutSpeaker = new WasapiOut(voiceMeeterAuxDevice, AudioClientShareMode.Shared, false, latencyInMs);
                        _waveOutSpeaker.Init(_audioFileSpeaker);
                    }

                    if (voiceMeeterVaioDevice != null)
                    {
                        _audioFileMic = new AudioFileReader(filePath);
                        _waveOutMic = new WasapiOut(voiceMeeterVaioDevice, AudioClientShareMode.Shared, false, latencyInMs);
                        _waveOutMic.Init(_audioFileMic);
                    }
                }
                else
                {
                    //No VoiceMeeter - use default device directly
                    System.Diagnostics.Debug.WriteLine("VoiceMeeter not detected - using default audio device");
                    _audioFileSpeaker = new AudioFileReader(filePath);
                    _waveOutSpeaker = new WasapiOut(AudioClientShareMode.Shared, 10);
                    _waveOutSpeaker.Init(_audioFileSpeaker);
                }

                var tcs = new TaskCompletionSource<bool>();
                int finishedCount = 0;
                int expectedCount = (_waveOutSpeaker != null ? 1 : 0) + (_waveOutMic != null ? 1 : 0);

                if (expectedCount == 0)
                    throw new InvalidOperationException("No audio output devices available");

                EventHandler<StoppedEventArgs> handler = (s, e) =>
                {
                    if (Interlocked.Increment(ref finishedCount) == expectedCount)
                        tcs.SetResult(true);
                };

                _waveOutSpeaker.PlaybackStopped += handler;
                _waveOutMic.PlaybackStopped += handler;

                _waveOutSpeaker?.Play();
                _waveOutMic?.Play();

                tcs.Task.Wait();
            });
        }
        catch (Exception ex)
        {
            StopCurrentSound();
            System.Diagnostics.Debug.WriteLine($"Error playing sound: {ex.Message}");
            throw;
        }
    }

    private MMDevice FindVoiceMeeterDevice(string type)
    {
        try
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            foreach (var device in devices)
            {
                System.Diagnostics.Debug.WriteLine($"Checking device: {device.FriendlyName}");

                if (type.Equals("Aux", StringComparison.OrdinalIgnoreCase))
                {
                    //For speakers/headphones look for VoiceMeeter Aux Input
                    if (device.FriendlyName.Contains("VoiceMeeter Aux Input", StringComparison.OrdinalIgnoreCase) ||
                        device.FriendlyName.Contains("VoiceMeeter Input (VB-Audio VoiceMeeter AUX VAIO)", StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine($"Found VoiceMeeter Aux device: {device.FriendlyName}");
                        return device;
                    }
                }
                else if (type.Equals("VAIO", StringComparison.OrdinalIgnoreCase))
                {
                    //For microphone look for VoiceMeeter VAIO
                    if (device.FriendlyName.Contains("VoiceMeeter Input (VB-Audio VoiceMeeter VAIO)", StringComparison.OrdinalIgnoreCase) ||
                        device.FriendlyName.Contains("CABLE Input", StringComparison.OrdinalIgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine($"Found VoiceMeeter VAIO device: {device.FriendlyName}");
                        return device;
                    }
                }
            }

            //Backup in case either of the above fail to find a device
            foreach (var device in devices)
            {
                if (device.FriendlyName.Contains("VB-Audio", StringComparison.OrdinalIgnoreCase) &&
                    device.FriendlyName.Contains(type, StringComparison.OrdinalIgnoreCase))
                {
                    System.Diagnostics.Debug.WriteLine($"Found VB-Audio device (fallback): {device.FriendlyName}");
                    return device;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error finding VoiceMeeter {type} device: {ex.Message}");
        }

        return null;
    }

    private void StopCurrentSound()
    {
        try
        {
            _waveOutSpeaker?.Stop();
            _waveOutSpeaker?.Dispose();
            _waveOutSpeaker = null;
            _audioFileSpeaker?.Dispose();
            _audioFileSpeaker = null;

            _waveOutMic?.Stop();
            _waveOutMic?.Dispose();
            _waveOutMic = null;
            _audioFileMic?.Dispose();
            _audioFileMic = null;
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

