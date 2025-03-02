using System;
using MusicSharp.Enums;
using SoundFlow.Abstracts;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;

namespace MusicSharp.SoundEngines;


// Cross-platform sound engine that works for all devices which
//  the .NET platform runs on.
public class SoundEngine: ISoundEngine
{
    public ePlayerStatus PlayerStatus { get; set; }
    public string LastFileOpened { get; set; }
    
    
    public void OpenFile(string path)
    {
        throw new NotImplementedException();
    }

    public void OpenStream(string streamUrl)
    {
        throw new NotImplementedException();
    }

    public void PlayPause()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public void IncreaseVolume()
    {
        throw new NotImplementedException();
    }

    public void DecreaseVolume()
    {
        throw new NotImplementedException();
    }

    public void PlayFromPlaylist(string path)
    {
        throw new NotImplementedException();
    }

    public TimeSpan CurrentTime()
    {
        throw new NotImplementedException();
    }

    public TimeSpan TrackLength()
    {
        throw new NotImplementedException();
    }

    public void SeekForward()
    {
        throw new NotImplementedException();
    }

    public void SeekBackwards()
    {
        throw new NotImplementedException();
    }
}