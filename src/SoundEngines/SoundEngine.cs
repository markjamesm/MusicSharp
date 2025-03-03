using System;
using System.IO;
using MusicSharp.Enums;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Providers;

namespace MusicSharp.SoundEngines;


// Cross-platform sound engine that works for all devices which
//  the .NET platform runs on.
public class SoundEngine: ISoundEngine, IDisposable
{
    private readonly MiniAudioEngine _soundEngine;
    private SoundPlayer _player;
    
    public ePlayerStatus PlayerStatus { get; set; }
    public string LastFileOpened { get; set; }


    public SoundEngine()
    {
     _soundEngine = new MiniAudioEngine(44100, Capability.Playback);
    }
    
    public void OpenFile(string path)
    {
        if (File.Exists(path))
        {
            _player = new SoundPlayer(new StreamDataProvider(File.OpenRead(path)));
            
            // Add the player to the master mixer. This connects the player's output to the audio engine's output.
            Mixer.Master.AddComponent(_player);
            
            _player.Play();
            PlayerStatus = ePlayerStatus.Playing;
        }
    }

    public void OpenStream(string streamUrl)
    {
        throw new NotImplementedException();
    }

    public void PlayPause()
    {
        switch (PlayerStatus)
        {
            case ePlayerStatus.Playing:
                _player.Pause();
                PlayerStatus = ePlayerStatus.Paused;
                break;
            case ePlayerStatus.Paused:
                _player.Play();
                PlayerStatus = ePlayerStatus.Playing;
                break;
            case ePlayerStatus.Stopped:
                _player.Play();
                PlayerStatus = ePlayerStatus.Playing;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Stop()
    {
        if (PlayerStatus != ePlayerStatus.Stopped)
        {
            _player.Stop();
            PlayerStatus = ePlayerStatus.Stopped;
        }
    }

    public void IncreaseVolume()
    {
        // Need to verify what SoundFlow's max volume level is
        // For now this should be enough based on testing
        if (_player.Volume < 2.0f)
        {
            _player.Volume += .1f;
        }
    }

    public void DecreaseVolume()
    {
        // Ensure that the volume isn't negative
        // otherwise the player will crash
        if (_player.Volume > .1f)
        {
            _player.Volume -= .1f;
        }

        if (_player.Volume <= .1f)
        {
            _player.Volume = 0f;
        }
    }

    public void PlayFromPlaylist(string path)
    {
        throw new NotImplementedException();
    }
    
   public float CurrentTime()
   {
       return _player.Time;
   }

    public float TrackLength()
    {
        return _player.Duration;
    }

    public void SeekForward()
    {
        throw new NotImplementedException();
    }

    public void SeekBackwards()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _soundEngine.Dispose();
    }
}