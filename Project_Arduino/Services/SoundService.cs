using Microsoft.JSInterop;

namespace Project_Arduino.Services
{
    public interface ISoundService
    {
        Task PlayBackgroundMusic(string fileName, bool loop = true, float volume = 0.5f);
        Task PlaySFX(string fileName, bool loop = false, float volume = 1.0f);
        Task StopBackgroundMusic();
        Task StopSFX(string fileName);
        Task StopAllSounds();
        Task SetBackgroundMusicVolume(float volume);
        Task SetSFXVolume(float volume);
        Task PauseBackgroundMusic();
        Task ResumeBackgroundMusic();
        Task MuteBackgroundMusic();
        Task UnmuteBackgroundMusic();
        Task MuteSFX();
        Task UnmuteSFX();
        Task<bool> IsBackgroundMusicMuted();
        Task<bool> IsSFXMuted();
    }

    public class SoundService : ISoundService, IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _soundModule;
        private bool _isInitialized = false;

        public SoundService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task EnsureInitializedAsync()
        {
            if (!_isInitialized)
            {
                try
                {
                    _soundModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/soundSystem.js");
                    await _soundModule.InvokeVoidAsync("initializeSoundSystem");
                    _isInitialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to initialize sound system: {ex.Message}");
                    _isInitialized = false;
                }
            }
        }

        public async Task PlayBackgroundMusic(string fileName, bool loop = true, float volume = 0.5f)
        {
            try
            {
                await EnsureInitializedAsync();
                if (_soundModule != null && _isInitialized)
                {
                    await _soundModule.InvokeVoidAsync("playBackgroundMusic", fileName, loop, volume);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing background music: {ex.Message}");
            }
        }

        public async Task PlaySFX(string fileName, bool loop = false, float volume = 1.0f)
        {
            try
            {
                await EnsureInitializedAsync();
                if (_soundModule != null && _isInitialized)
                {
                    await _soundModule.InvokeVoidAsync("playSFX", fileName, loop, volume);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing SFX: {ex.Message}");
            }
        }

        public async Task StopBackgroundMusic()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("stopBackgroundMusic");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping background music: {ex.Message}");
                }
            }
        }

        public async Task StopSFX(string fileName)
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("stopSFX", fileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping SFX: {ex.Message}");
                }
            }
        }

        public async Task StopAllSounds()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("stopAllSounds");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error stopping all sounds: {ex.Message}");
                }
            }
        }

        public async Task SetBackgroundMusicVolume(float volume)
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("setBackgroundMusicVolume", volume);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting background music volume: {ex.Message}");
                }
            }
        }

        public async Task SetSFXVolume(float volume)
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("setSFXVolume", volume);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error setting SFX volume: {ex.Message}");
                }
            }
        }

        public async Task PauseBackgroundMusic()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("pauseBackgroundMusic");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error pausing background music: {ex.Message}");
                }
            }
        }

        public async Task ResumeBackgroundMusic()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("resumeBackgroundMusic");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error resuming background music: {ex.Message}");
                }
            }
        }

        public async Task MuteBackgroundMusic()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("muteBackgroundMusic");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error muting background music: {ex.Message}");
                }
            }
        }

        public async Task UnmuteBackgroundMusic()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("unmuteBackgroundMusic");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error unmuting background music: {ex.Message}");
                }
            }
        }

        public async Task MuteSFX()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("muteSFX");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error muting SFX: {ex.Message}");
                }
            }
        }

        public async Task UnmuteSFX()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("unmuteSFX");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error unmuting SFX: {ex.Message}");
                }
            }
        }

        public async Task<bool> IsBackgroundMusicMuted()
        {
            if (_soundModule != null)
            {
                try
                {
                    return await _soundModule.InvokeAsync<bool>("isBackgroundMusicMuted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking background music mute status: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

        public async Task<bool> IsSFXMuted()
        {
            if (_soundModule != null)
            {
                try
                {
                    return await _soundModule.InvokeAsync<bool>("isSFXMuted");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking SFX mute status: {ex.Message}");
                    return false;
                }
            }
            return false;
        }

        public async ValueTask DisposeAsync()
        {
            if (_soundModule != null)
            {
                try
                {
                    await _soundModule.InvokeVoidAsync("cleanup");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during sound system cleanup: {ex.Message}");
                }

                try
                {
                    await _soundModule.DisposeAsync();
                }
                catch
                {
                    // Ignore disposal errors
                }
            }
        }
    }
}