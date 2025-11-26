# Audio Mixing System - Change Log

## Version 2.0 - Audio Priority System Implementation

### Problem Fixed
- **Issue**: BGM and other music mixed when playing simultaneously, causing volume chaos and poor audio experience
- **Root Cause**: Single AudioSource used for all audio types (BGM, SFX), causing conflicts and overlapping audio

### Changes Made

#### 1. Separate Audio Source Architecture
**File: `AudioManager.cs`**

- **Removed**: Single `audioSource` field
- **Added**: Two dedicated AudioSource fields:
  - `public AudioSource bgmAudioSource` - For background music (high priority)
  - `public AudioSource sfxAudioSource` - For sound effects (lower priority)

#### 2. Audio Priority System
**Added**: `isAudioPriority` flag to track when high-priority audio is active

#### 3. New Methods

##### PlayBGM(AudioClip clip, bool loop = true)
- Plays background music
- Automatically pauses SFX when BGM starts
- Sets audio priority flag
- Supports looping (default: true)
- Replaces previously playing BGM

##### StopBGM()
- Stops background music
- Automatically resumes paused SFX
- Clears audio priority flag
- Ensures seamless audio transition

##### Updated PlayAudio(BuildType buildType)
- Now uses `sfxAudioSource` instead of generic `audioSource`
- Stops current SFX before playing new one
- Better error handling and validation
- Improved debug logging

#### 4. Enhanced BGM Control

##### PauseBGM() - Enhanced
**Previous behavior**: Only reduced volume to 0
**New behavior**:
- Saves current volume state
- Reduces volume to 0 (mixer level)
- **Additionally**: Pauses actual audio playback (AudioSource.Pause())
- Sets `isBGMPaused` flag
- Preserves complete audio state for full restoration

##### ResumeBGM() - Enhanced
**Previous behavior**: Only restored volume
**New behavior**:
- Restores saved volume level
- **Additionally**: Resumes audio playback (AudioSource.UnPause())
- Updates UI slider display
- Fully restores BGM state exactly as before

#### 5. Improved Error Handling
- Null checks for all AudioSource operations
- Warning logs for missing AudioSource assignments
- Validation of loaded audio clips
- Safe handling of edge cases

#### 6. Documentation
- Created comprehensive `AUDIO_MIXING_DOCUMENTATION.md` with:
  - System overview and architecture
  - Problem statement and solution approach
  - Setup instructions for proper configuration
  - Usage examples and behavioral specifications
  - Mixer configuration guidelines
  - Technical implementation details
  - Troubleshooting guide

### Backward Compatibility
- `PlayAudio()` method signature unchanged
- Existing calls to `PauseBGM()` and `ResumeBGM()` work as before
- All previous methods maintained for compatibility
- No breaking changes to public API

### Files Modified
1. **test/AudioManager.cs** - Complete audio system overhaul
   - Added 2 new AudioSource fields
   - Added 1 new audio priority field
   - Added 2 new public methods (PlayBGM, StopBGM)
   - Enhanced 4 existing methods (PlayAudio, PauseBGM, ResumeBGM, TemporaryMuteBGM)
   - Improved error handling and logging

### Files Added
1. **AUDIO_MIXING_DOCUMENTATION.md** - Complete system documentation
2. **.gitignore** - Proper Unity project .gitignore

### Acceptance Criteria Met

✅ **BGM playback control**
- Implemented `PlayBGM()` method with automatic SFX pause
- Implemented `StopBGM()` method with automatic SFX resume
- BGM takes priority over SFX when both attempt to play

✅ **Audio channel separation**
- Separate AudioSource for BGM and SFX
- Independent mixer groups for each audio type
- Clear priority hierarchy

✅ **Priority system**
- `isAudioPriority` flag tracks high-priority audio
- BGM automatically pauses SFX during playback
- Automatic resumption when BGM stops

✅ **Preview mode handling**
- `PauseBGM()` properly pauses audio while preserving state
- `ResumeBGM()` fully restores previous state
- Used by Item.cs for model preview functionality

### Testing Recommendations

1. **Basic BGM Playback**
   - Play BGM and verify it plays without distortion
   - Check mixer levels are appropriate

2. **BGM and SFX Priority**
   - Play BGM while SFX is playing
   - Verify SFX pauses automatically
   - Verify SFX resumes when BGM stops

3. **Volume Control**
   - Adjust sliders and verify independent control
   - Check volume persistence via PlayerPrefs
   - Verify mixer parameter updates

4. **Model Preview Mode**
   - Start preview (BGM should pause)
   - Verify exact volume restoration after preview
   - Check multiple preview cycles work correctly

5. **Edge Cases**
   - Stop BGM when no SFX is playing
   - Play SFX when BGM not assigned
   - Rapid BGM start/stop cycles
   - Missing audio files and clips

### Integration Notes

To fully utilize the new system:

1. **Create AudioSources in Scene**
   - Add BGMSource child GameObject to AudioManager
   - Add SFXSource child GameObject to AudioManager
   - Assign to respective fields in Inspector

2. **Configure Mixer Groups**
   - Ensure BGMVolume and EffectVolume groups exist
   - Connect AudioSources to proper mixer groups
   - Verify parameter names match configuration

3. **Update Game Code**
   - Use `PlayBGM()` when starting background music
   - Use `StopBGM()` when ending background music
   - `PlayAudio()` continues working for SFX
   - Model preview already integrated via Item.cs

### Future Considerations
- Consider adding fade in/fade out transitions
- Implement cross-fade for BGM transitions
- Support for multiple simultaneous SFX
- Audio ducking for automatic volume adjustment
- Event system for audio state changes
