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
                _soundModule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/soundSystem.js");
                await _soundModule.InvokeVoidAsync("initializeSoundSystem");
                _isInitialized = true;
            }
        }

        public async Task PlayBackgroundMusic(string fileName, bool loop = true, float volume = 0.5f)
        {
            await EnsureInitializedAsync();
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("playBackgroundMusic", fileName, loop, volume);
            }
        }

        public async Task PlaySFX(string fileName, bool loop = false, float volume = 1.0f)
        {
            await EnsureInitializedAsync();
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("playSFX", fileName, loop, volume);
            }
        }

        public async Task StopBackgroundMusic()
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("stopBackgroundMusic");
            }
        }

        public async Task StopSFX(string fileName)
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("stopSFX", fileName);
            }
        }

        public async Task StopAllSounds()
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("stopAllSounds");
            }
        }

        public async Task SetBackgroundMusicVolume(float volume)
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("setBackgroundMusicVolume", volume);
            }
        }

        public async Task SetSFXVolume(float volume)
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("setSFXVolume", volume);
            }
        }

        public async Task PauseBackgroundMusic()
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("pauseBackgroundMusic");
            }
        }

        public async Task ResumeBackgroundMusic()
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("resumeBackgroundMusic");
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_soundModule != null)
            {
                await _soundModule.InvokeVoidAsync("cleanup");
                await _soundModule.DisposeAsync();
            }
        }
    }
}