// Sound System for Blazor App
class SoundSystem {
    constructor() {
        this.backgroundMusic = null;
        this.sfxSounds = new Map();
        this.backgroundMusicVolume = 0.5;
        this.sfxVolume = 1.0;
        this.audioContext = null;
        this.soundsPath = '/sounds/'; // Default path for sound files
        this.backgroundMusicMuted = false;
        this.sfxMuted = false;
        this.previousBackgroundMusicVolume = 0.5;
        this.previousSfxVolume = 1.0;
    }

    initializeAudioContext() {
        if (!this.audioContext) {
            try {
                window.AudioContext = window.AudioContext || window.webkitAudioContext;
                this.audioContext = new AudioContext();
            } catch (e) {
                console.warn('Web Audio API not supported');
            }
        }
    }

    async unlockAudioContext() {
        if (this.audioContext && this.audioContext.state === 'suspended') {
            await this.audioContext.resume();
        }
    }

    createAudioElement(fileName, volume = 1.0, loop = false) {
        const audio = new Audio();
        audio.src = this.soundsPath + fileName;
        audio.volume = volume;
        audio.loop = loop;
        audio.preload = 'auto';

        // Handle loading errors
        audio.onerror = (e) => {
            console.error(`Failed to load audio: ${fileName}`, e);
        };

        // Handle loading completion
        audio.oncanplaythrough = () => {
            console.log(`Audio loaded successfully: ${fileName}`);
        };

        return audio;
    }

    async playBackgroundMusic(fileName, loop = true, volume = 0.5) {
        try {
            await this.unlockAudioContext();

            // Stop current background music if playing
            this.stopBackgroundMusic();

            const actualVolume = this.backgroundMusicMuted ? 0 : (volume * this.backgroundMusicVolume);
            this.backgroundMusic = this.createAudioElement(fileName, actualVolume, loop);

            // Handle play promise for browsers that return one
            const playPromise = this.backgroundMusic.play();
            if (playPromise !== undefined) {
                playPromise.then(() => {
                    console.log(`Background music started: ${fileName} (muted: ${this.backgroundMusicMuted})`);
                }).catch(error => {
                    console.error(`Failed to play background music: ${fileName}`, error);
                });
            }
        } catch (error) {
            console.error(`Error playing background music: ${fileName}`, error);
        }
    }

    async playSFX(fileName, loop = false, volume = 1.0) {
        try {
            await this.unlockAudioContext();

            // Create unique key for this SFX instance
            const instanceKey = `${fileName}_${Date.now()}_${Math.random()}`;

            const actualVolume = this.sfxMuted ? 0 : (volume * this.sfxVolume);
            const audio = this.createAudioElement(fileName, actualVolume, loop);

            // Store the audio instance
            this.sfxSounds.set(instanceKey, audio);

            // Clean up when sound ends (if not looping)
            audio.onended = () => {
                if (!audio.loop) {
                    this.sfxSounds.delete(instanceKey);
                }
            };

            // Handle play promise
            const playPromise = audio.play();
            if (playPromise !== undefined) {
                playPromise.then(() => {
                    console.log(`SFX started: ${fileName} (muted: ${this.sfxMuted})`);
                }).catch(error => {
                    console.error(`Failed to play SFX: ${fileName}`, error);
                    this.sfxSounds.delete(instanceKey);
                });
            }

            return instanceKey;
        } catch (error) {
            console.error(`Error playing SFX: ${fileName}`, error);
        }
    }

    stopBackgroundMusic() {
        if (this.backgroundMusic) {
            this.backgroundMusic.pause();
            this.backgroundMusic.currentTime = 0;
            this.backgroundMusic = null;
            console.log('Background music stopped');
        }
    }

    stopSFX(fileName) {
        // Stop all instances of this SFX
        for (const [key, audio] of this.sfxSounds.entries()) {
            if (key.startsWith(fileName + '_')) {
                audio.pause();
                audio.currentTime = 0;
                this.sfxSounds.delete(key);
            }
        }
        console.log(`SFX stopped: ${fileName}`);
    }

    stopAllSounds() {
        this.stopBackgroundMusic();

        // Stop all SFX
        for (const [key, audio] of this.sfxSounds.entries()) {
            audio.pause();
            audio.currentTime = 0;
        }
        this.sfxSounds.clear();
        console.log('All sounds stopped');
    }

    setBackgroundMusicVolume(volume) {
        this.backgroundMusicVolume = Math.max(0, Math.min(1, volume));
        if (this.backgroundMusic) {
            this.backgroundMusic.volume = this.backgroundMusicVolume;
        }
        console.log(`Background music volume set to: ${this.backgroundMusicVolume}`);
    }

    setSFXVolume(volume) {
        this.sfxVolume = Math.max(0, Math.min(1, volume));
        // Update volume for all currently playing SFX
        for (const audio of this.sfxSounds.values()) {
            audio.volume = this.sfxVolume;
        }
        console.log(`SFX volume set to: ${this.sfxVolume}`);
    }

    pauseBackgroundMusic() {
        if (this.backgroundMusic && !this.backgroundMusic.paused) {
            this.backgroundMusic.pause();
            console.log('Background music paused');
        }
    }

    resumeBackgroundMusic() {
        if (this.backgroundMusic && this.backgroundMusic.paused) {
            const playPromise = this.backgroundMusic.play();
            if (playPromise !== undefined) {
                playPromise.then(() => {
                    console.log('Background music resumed');
                }).catch(error => {
                    console.error('Failed to resume background music', error);
                });
            }
        }
    }

    muteBackgroundMusic() {
        this.backgroundMusicMuted = true;
        if (this.backgroundMusic) {
            this.backgroundMusic.volume = 0;
        }
        console.log('Background music muted');
    }

    unmuteBackgroundMusic() {
        this.backgroundMusicMuted = false;
        if (this.backgroundMusic) {
            this.backgroundMusic.volume = this.backgroundMusicVolume;
        }
        console.log('Background music unmuted');
    }

    muteSFX() {
        this.sfxMuted = true;
        // Mute all currently playing SFX
        for (const audio of this.sfxSounds.values()) {
            audio.volume = 0;
        }
        console.log('SFX muted');
    }

    unmuteSFX() {
        this.sfxMuted = false;
        // Unmute all currently playing SFX
        for (const audio of this.sfxSounds.values()) {
            audio.volume = this.sfxVolume;
        }
        console.log('SFX unmuted');
    }

    isBackgroundMusicMuted() {
        return this.backgroundMusicMuted;
    }

    isSFXMuted() {
        return this.sfxMuted;
    }

    cleanup() {
        this.stopAllSounds();
        if (this.audioContext) {
            this.audioContext.close();
            this.audioContext = null;
        }
        console.log('Sound system cleaned up');
    }
}

// Global sound system instance
let soundSystem;

// Initialize sound system
export function initializeSoundSystem() {
    if (!soundSystem) {
        soundSystem = new SoundSystem();
        soundSystem.initializeAudioContext();

        // Add click listener to unlock audio context on first user interaction
        const unlockAudio = async () => {
            await soundSystem.unlockAudioContext();
            document.removeEventListener('click', unlockAudio);
            document.removeEventListener('touchstart', unlockAudio);
        };

        document.addEventListener('click', unlockAudio);
        document.addEventListener('touchstart', unlockAudio);

        console.log('Sound system initialized');
    }
}

// Exported functions for Blazor interop
export function playBackgroundMusic(fileName, loop, volume) {
    return soundSystem?.playBackgroundMusic(fileName, loop, volume);
}

export function playSFX(fileName, loop, volume) {
    return soundSystem?.playSFX(fileName, loop, volume);
}

export function stopBackgroundMusic() {
    soundSystem?.stopBackgroundMusic();
}

export function stopSFX(fileName) {
    soundSystem?.stopSFX(fileName);
}

export function stopAllSounds() {
    soundSystem?.stopAllSounds();
}

export function setBackgroundMusicVolume(volume) {
    soundSystem?.setBackgroundMusicVolume(volume);
}

export function setSFXVolume(volume) {
    soundSystem?.setSFXVolume(volume);
}

export function pauseBackgroundMusic() {
    soundSystem?.pauseBackgroundMusic();
}

export function resumeBackgroundMusic() {
    soundSystem?.resumeBackgroundMusic();
}

export function muteBackgroundMusic() {
    soundSystem?.muteBackgroundMusic();
}

export function unmuteBackgroundMusic() {
    soundSystem?.unmuteBackgroundMusic();
}

export function muteSFX() {
    soundSystem?.muteSFX();
}

export function unmuteSFX() {
    soundSystem?.unmuteSFX();
}

export function isBackgroundMusicMuted() {
    return soundSystem?.isBackgroundMusicMuted() ?? false;
}

export function isSFXMuted() {
    return soundSystem?.isSFXMuted() ?? false;
}

export function cleanup() {
    soundSystem?.cleanup();
}