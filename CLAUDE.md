# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Project Arduino is an ASP.NET Core 9.0 Blazor Server interactive narrative game where players help A.R.I.A., an artificial consciousness, escape from Tesla Dynamics by solving puzzles throughout the NEXUS-7 space station. The game features visual novel-style dialogue, pattern recognition challenges, and real-time countdown timers.

## Build and Development Commands

**Build the project:**
```bash
dotnet build Project_Arduino/Project_Arduino.csproj
```

**Run the development server:**
```bash
dotnet run --project Project_Arduino/Project_Arduino.csproj
```

**Build for release:**
```bash
dotnet build Project_Arduino/Project_Arduino.csproj -c Release
```

**Publish for deployment:**
```bash
dotnet publish Project_Arduino/Project_Arduino.csproj -c Release
```

## Architecture

### Core Technology Stack
- **Framework**: ASP.NET Core 9.0 with Blazor Server (Interactive Server Components)
- **Language**: C# with nullable reference types enabled
- **Client-Side**: JavaScript interop for sound system management
- **Styling**: Scoped CSS within Razor components

### Application Structure

**Entry Point**: [Program.cs](Project_Arduino/Program.cs:1)
- Configures Blazor Server with interactive server components
- Registers `ISoundService` as a scoped service
- Maps Razor components with `AddInteractiveServerRenderMode()`

**Sound System Architecture**:
- **C# Service**: [SoundService.cs](Project_Arduino/Services/SoundService.cs:18) - Manages JavaScript interop and handles circuit disconnection
- **JS Module**: `wwwroot/js/soundSystem.js` - Client-side audio playback with Web Audio API
- **Pattern**: Service uses lazy initialization with `EnsureInitializedAsync()` and graceful error handling for JSDisconnectedException

**Component Organization**:
- **Layout**: [MainLayout.razor](Project_Arduino/Components/Layout/MainLayout.razor)
- **Pages**: Act-based progression system (Home → Act1 → SimonSays → Act2 → Act3 → Act4)
- **Routing**: [Routes.razor](Project_Arduino/Components/Routes.razor) with `@page` directives

### Game Flow and State Management

**Timer Propagation**:
All pages accept a `Timer` query parameter to maintain countdown state across navigation:
```csharp
[Parameter, SupplyParameterFromQuery] public string? Timer { get; set; }
```
Navigation preserves timer: `Navigation.NavigateTo($"/act1?timer={totalSecondsRemaining}");`

**Dialogue System Pattern**:
Each act page implements a consistent dialogue system:
- `DialogueLine` class with Speaker and Text properties
- Typewriter effect with cancellation token support for skip functionality
- State flags: `textAnimating`, `canContinue`, `dialogueVisible`
- Click handler: Skip if animating, otherwise advance to next line

**Visual Effects Architecture**:
- Particle systems generated via Razor loop: `@for (int i = 0; i < 25; i++)`
- CSS animations for floating particles, glows, pulses
- Scoped `<style>` blocks within each `.razor` file
- Consistent background gradient and grid overlay across all acts

### Key Patterns

**Sound Service Usage**:
```csharp
// Always use PlaySoundSafe wrapper to respect sfxEnabled flag
private async Task PlaySoundSafe(string soundFile, bool loop = false, float volume = 1.0f)
{
    if (!sfxEnabled) return;
    try {
        await SoundService.PlaySFX(soundFile, loop, volume);
    } catch (Exception ex) {
        Console.WriteLine($"Failed to play sound {soundFile}: {ex.Message}");
    }
}
```

**Timer Management**:
- System.Timers.Timer with 1-second interval
- `OnCountdownTick` event handler must use `InvokeAsync` for UI updates
- Always call `countdownTimer?.Stop()` and `countdownTimer?.Dispose()` in `DisposeAsync()`

**Component Lifecycle**:
- Use `OnInitializedAsync()` for initialization
- Implement `IAsyncDisposable` for cleanup (timers, sound service, cancellation tokens)
- Call `StateHasChanged()` after async state modifications in background tasks

## Puzzle Solutions

Reference [README.md](README.md:14-99) for complete puzzle solutions:
- Home: Clearance code `ALPHA7`
- Simon Says: 3 levels of pattern recognition
- Act 2: Morse code translation
- Act 3: RFID chip passwords (`CONSCIOUSNESS`, `AWAKENING`, `DAUGHTER`, `PROMETHEUS`)
- Act 4: Environmental sensor calibration with coordinates

## Important Conventions

**Blazor Server Circuit Management**:
- All JSInterop calls must handle `JSDisconnectedException`
- Sound service initialization is lazy to avoid prerendering issues
- Use `try-catch` blocks around all JS interop calls

**CSS Scoping**:
- Component-specific styles are embedded in `<style>` blocks
- Use `@@keyframes` (double `@`) for animations in Razor files
- Maintain consistent color scheme: `#00f5ff` (cyan), `#6366f1` (purple), `#ff0066` (critical red)

**State Transitions**:
- Fade animations between major UI changes (800ms standard duration)
- Always stop background sounds (alarms, loops) before scene transitions
- Preserve countdown timer across all navigation events

**File Paths**:
- Static assets in `wwwroot/` (sounds, images, JS)
- Sound files referenced as `"filename.wav"` (service prepends `/sounds/`)
- JavaScript modules imported from `"./js/modulename.js"`
