# Unity Audio Mixing System Documentation

## Overview
This document describes the improved audio mixing system for the Unity project, designed to solve BGM and SFX audio conflicts and provide clear audio channel separation.

## Problem Statement
The previous implementation used a single `AudioSource` for all audio playback, causing:
- BGM and SFX mixing issues
- Audio volume chaos
- Poor audio experience when multiple audio types play simultaneously
- Lack of priority management between different audio types

## Solution Architecture

### 1. Separate Audio Sources

The new system uses **two dedicated AudioSources**:

#### BGM AudioSource
- **Purpose**: Plays background music
- **Properties**: Usually looped, continuous playback
- **Assignment**: `bgmAudioSource` field
- **Mixer Output**: Connected to "BGMVolume" mixer group

#### SFX AudioSource
- **Purpose**: Plays sound effects and UI sounds
- **Properties**: Non-looped, short-duration audio
- **Assignment**: `sfxAudioSource` field
- **Mixer Output**: Connected to "EffectVolume" mixer group

### 2. Audio Priority System

The system implements a priority mechanism:
- **High Priority**: BGM audio
- **Low Priority**: SFX audio
- When BGM plays, SFX is automatically paused
- When BGM stops, SFX can resume from where it was paused

### 3. Core Methods

#### PlayBGM(AudioClip clip, bool loop = true)
Plays background music with automatic SFX management:
```csharp
public void PlayBGM(AudioClip clip, bool loop = true)
```
- Stops currently playing BGM
- Pauses SFX if playing
- Sets `isAudioPriority` flag to true
- Plays the provided audio clip in loop (by default)

#### StopBGM()
Stops BGM and restores SFX:
```csharp
public void StopBGM()
```
- Stops BGM playback
- Unpauses SFX if it was paused
- Sets `isAudioPriority` flag to false

#### PlayAudio(BuildType buildType)
Plays SFX (kept for backward compatibility):
```csharp
public void PlayAudio(BuildType buildType)
```
- Loads audio clip from "建筑/" resource folder
- Stops currently playing SFX
- Plays the new SFX clip
- Does not affect BGM playback

#### PauseBGM()
Pauses BGM while preserving its state:
```csharp
public void PauseBGM()
```
- Saves current BGM volume
- Sets BGM volume to 0
- Pauses BGM audio playback
- Used when entering model preview mode

#### ResumeBGM()
Resumes previously paused BGM:
```csharp
public void ResumeBGM()
```
- Restores saved BGM volume
- Unpauses BGM audio playback
- Updates UI slider to reflect current volume
- Used when exiting model preview mode

## Setup Instructions

### Required AudioSources
1. Create two new GameObject children under the AudioManager:
   - `BGMSource` - for background music
   - `SFXSource` - for sound effects

2. Add `AudioSource` component to each GameObject

3. Configure the AudioSource components:
   - **BGM AudioSource**:
     - Uncheck "Play On Awake"
     - Leave "Spatial Blend" at 0 (2D)
     - Adjust volume to taste (or use mixer)
   
   - **SFX AudioSource**:
     - Uncheck "Play On Awake"
     - Leave "Spatial Blend" at 0 (2D)
     - Adjust volume to taste (or use mixer)

4. Assign in the Inspector:
   - Drag BGMSource AudioSource to `bgmAudioSource` field
   - Drag SFXSource AudioSource to `sfxAudioSource` field

### Audio Mixer Configuration
Ensure your AudioMixer has the following groups:
- **Master** (root)
  - **BGMVolume** - for background music (connects to BGM AudioSource)
  - **EffectVolume** - for sound effects (connects to SFX AudioSource)
  - **MasterVolume** - for global volume control

Connect AudioSources to their respective mixer groups via the Inspector.

## Usage Examples

### Playing BGM
```csharp
AudioClip bgmClip = Resources.Load<AudioClip>("Music/background_music");
AudioManager.Instance.PlayBGM(bgmClip, true);
```

### Stopping BGM
```csharp
AudioManager.Instance.StopBGM();
```

### Playing SFX
```csharp
AudioManager.Instance.PlayAudio(BuildType.Tower);
```

### Model Preview Mode
```csharp
// When starting preview
AudioManager.Instance.PauseBGM();

// When ending preview
AudioManager.Instance.ResumeBGM();
```

## Behavioral Specifications

### BGM Playback
1. BGM always takes priority
2. When BGM starts, SFX is paused
3. When BGM stops, SFX automatically resumes
4. BGM volume can be controlled independently via the slider
5. BGM respects the global volume control

### SFX Playback
1. SFX plays when BGM is not active
2. If BGM starts while SFX is playing, SFX is paused
3. If SFX is interrupted by BGM, it resumes after BGM stops
4. Multiple SFX calls will replace the previous one
5. SFX respects the global volume control

### Model Preview Mode
1. BGM is paused (not muted, but paused)
2. BGM volume is preserved
3. After preview ends, BGM resumes with exact previous volume
4. No manual BGM control needed during preview

## Mixer Configuration Example

For best results, configure your AudioMixer like this:

```
AudioMixer
├── MasterVolume (parameter: MasterVolume, default: 0 dB)
├── BGMVolume (parameter: BGMVolume, default: -10 dB)
└── EffectVolume (parameter: EffectVolume, default: -10 dB)
```

Each mixer group can have:
- Attenuation effect for volume control
- Optional reverb or other effects

## Technical Details

### Volume Conversion
- Linear volume (0-1) is converted to decibels (-80dB to 0dB)
- Formula: `dB = log₁₀(volume) × 20`
- Prevents log(0) errors by clamping to -80dB minimum

### State Management
- `isAudioPriority`: Tracks whether high-priority audio is playing
- `isBGMPaused`: Tracks whether BGM is explicitly paused
- `savedBGMVolume`: Preserves volume when pausing

### Memory Management
- Event listeners are properly removed in `OnDestroy()`
- No circular references or memory leaks
- Proper null checking for all AudioSource operations

## Future Enhancements

Possible improvements for future versions:
1. Fade in/fade out transitions
2. Cross-fade between different BGM tracks
3. Support for multiple SFX playing simultaneously
4. Audio ducking (automatic volume reduction)
5. 3D audio positioning support
6. Audio events system for better decoupling

## Troubleshooting

### BGM not playing
- Check if `bgmAudioSource` is assigned in the Inspector
- Verify the AudioClip is loaded correctly
- Check if global volume is set to 0
- Verify AudioMixer and mixer groups are properly connected

### SFX not playing
- Check if `sfxAudioSource` is assigned in the Inspector
- Verify the resource path matches the loading code
- Check if SFX is being paused by BGM priority
- Verify EffectVolume mixer group is not muted

### Audio mixing issues
- Ensure each AudioSource is connected to the correct mixer group
- Check mixer output levels are not clipping
- Verify slider values are in proper range (0-1)
- Confirm AudioListener is present in the scene

### Volume slider not working
- Verify mixer parameter names match exactly
- Check if AudioMixer reference is assigned
- Ensure slider is connected to `onValueChanged` events
