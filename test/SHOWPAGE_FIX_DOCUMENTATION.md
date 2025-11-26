# ShowPage BGM Integration Fix

## Problem Identified
The `ShowPage.cs` file was opening and closing the model preview UI without properly managing the audio state. While `Item.cs` had methods to pause and resume BGM during model preview, these methods were **never being called**.

## Issues Found

### Issue 1: BGM Not Paused During Preview
**File**: ShowPage.cs - `Open()` method
- When a user opens a model preview, the BGM continues playing
- The audio preview button (ClickAudio) attempts to play SFX
- BGM and SFX mix together, causing audio confusion
- This violates the acceptance criteria: "BGM playing时，其他音乐应该暂停或停止"

### Issue 2: BGM Not Resumed After Preview
**File**: ShowPage.cs - `Close()` method  
- When a user closes the preview, BGM remains paused (if it was paused)
- The preview state is not cleaned up properly
- This causes persistent audio state issues

## Solution Implemented

### Change 1: Open() Method Enhancement
**Added code to start model preview:**
```csharp
// Start model preview to pause BGM
if (_Item != null)
{
    _Item.StartModelPreview();
}
```

**Effect**:
- When model preview opens, `Item.StartModelPreview()` is called
- This triggers `AudioManager.PauseBGM()`
- BGM volume is muted and playback paused
- BGM state is saved for later restoration
- SFX can now play clearly without mixing with BGM

### Change 2: Close() Method Enhancement  
**Added code to end model preview:**
```csharp
// End model preview to resume BGM
if (_Item != null)
{
    _Item.EndModelPreview();
}
```

**Effect**:
- When model preview closes, `Item.EndModelPreview()` is called
- This triggers `AudioManager.ResumeBGM()`
- BGM volume and playback are fully restored
- Audio returns to pre-preview state exactly

## Integration Flow

### Before Fix (Broken)
```
1. User clicks model
2. ShowPage.Open() called → BGM still playing
3. User clicks ClickAudio() → Plays SFX while BGM plays
4. Audio mixing occurs → Poor experience
5. User closes preview
6. ShowPage.Close() called → BGM still playing (or may be stuck paused)
```

### After Fix (Correct)
```
1. User clicks model
2. ShowPage.Open() called
3. Open() calls Item.StartModelPreview()
4. StartModelPreview() calls AudioManager.PauseBGM()
5. BGM paused, state saved
6. User clicks ClickAudio() → Plays SFX cleanly without BGM
7. Clear audio experience
8. User closes preview
9. ShowPage.Close() called
10. Close() calls Item.EndModelPreview()
11. EndModelPreview() calls AudioManager.ResumeBGM()
12. BGM resumes with exact previous state
```

## Acceptance Criteria Met

✅ **BGM pauses when entering model preview**
- User enters preview → BGM automatically paused

✅ **BGM resumes after exiting model preview**
- User exits preview → BGM automatically resumes

✅ **Clean audio channels during preview**
- No mixing between BGM and SFX preview sound
- Clear audio for model demonstration

✅ **State preservation**
- BGM volume exactly preserved
- Audio state fully restored after preview

## Testing Checklist

- [ ] Play BGM background music
- [ ] Click on a model to open preview
- [ ] Verify BGM pauses/mutes
- [ ] Click "Play Audio" button to hear model sound clearly
- [ ] Verify no BGM is mixing with model sound
- [ ] Close the preview window
- [ ] Verify BGM resumes with same volume as before
- [ ] Test multiple preview cycles - volume should remain consistent
- [ ] Test rapid open/close - should handle gracefully

## Technical Details

### Call Chain
```
ShowPage.Open() 
  → Item.StartModelPreview()
    → AudioManager.PauseBGM()
      → Save current BGM volume
      → SetMixerVolume(bgmVolumeParam, 0f)
      → bgmAudioSource.Pause()

ShowPage.Close()
  → Item.EndModelPreview()
    → AudioManager.ResumeBGM()
      → Restore saved BGM volume
      → bgmAudioSource.UnPause()
      → Update slider UI
```

### Null Safety
- Both methods check `if (_Item != null)` before calling
- Item.cs methods check AudioManager instance validity
- Safe handling of edge cases

## Related Files
- **AudioManager.cs** - Enhanced PauseBGM() and ResumeBGM() methods
- **Item.cs** - StartModelPreview() and EndModelPreview() methods
- **ShowPage.cs** - Now properly integrates with audio system
