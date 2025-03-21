using System;
using MusicSharp.AudioPlayer;
using MusicSharp.Enums;
using Terminal.Gui;

namespace MusicSharp.UI;

public class Tui : Toplevel
{
    private readonly IPlayer _player;
    private ProgressBar _progressBar;
    private object? _mainLoopTimeout;
    private readonly uint _mainLoopTimeoutTick = 100; // ms
    private Window? _nowPlayingWindow;

    public Tui(IPlayer player)
    {
        _player = player;
        
        var menuBar = new MenuBar()
        {
            Title = "MusicSharp",
            MenusBorderStyle = LineStyle.Rounded,
            Menus =
            [
                new MenuBarItem(
                    "_File",
                    new MenuItem[]
                    {
                        new(
                            "_Open file",
                            "Open a local audio file",
                            OpenFile
                        ),
                        new(
                            "Open _stream",
                            "Open a web stream",
                            OpenStream
                        ),
                        new(
                            "_Quit",
                            "Quit MusicSharp",
                            RequestStop
                        )
                    }
                )
            ]
        };
        
        var libraryWindow = new Window()
        {
            Title = "Library",
            Y = Pos.Bottom(menuBar),
            Width = Dim.Fill(),
            Height = Dim.Auto(),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded
        };

        #region PlayBackControls
        
        var playbackControls = new Window()
        {
            Title = "Playback",
            X = 0,
            Y = Pos.Bottom(libraryWindow),
            Width = Dim.Auto(),
            Height = Dim.Auto(),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded
        };

        var playPauseButton = new Button
        {
            X = 0, 
            Y = 0, 
            IsDefault = false, 
            CanFocus = true, 
            Text = "Play/Pause"
        };
        
        var stopButton = new Button
        {
            X = 0, 
            Y = Pos.Bottom(playPauseButton), 
            IsDefault = false, 
            CanFocus = true, 
            Text = "Stop"
        };
        
        var volumeIncreaseButton = new Button
        {
            X = Pos.Right(playPauseButton), 
            Y = 0, 
            IsDefault = false, 
            CanFocus = true, 
            Text = "Volume +"
        };
        
        var volumeDecreaseButton = new Button
        {
            X = Pos.Right(playPauseButton), 
            Y = Pos.Bottom(volumeIncreaseButton), 
            CanFocus = true, 
            IsDefault = false, 
            Text = "Volume -"
        };
        
        var seekForwardButton = new Button
        {
            X = Pos.Right(volumeIncreaseButton), 
            Y = 0, 
            IsDefault = false, 
            CanFocus = true, 
            Text = "Seek 5s"
        };
        
        var seekBackwardButton = new Button
        {
            X = Pos.Right(volumeDecreaseButton), 
            Y = Pos.Bottom(seekForwardButton), 
            IsDefault = false, 
            CanFocus = true, 
            Text = "Seek -5s"
        };
        
        playPauseButton.Accepting += (s, args) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.PlayPause();
            }
        };
        stopButton.Accepting += (s, args)  =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.Stop();
            }
        };
        volumeIncreaseButton.Accepting += (s, args) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.IncreaseVolume();
            }
        }; 
        volumeDecreaseButton.Accepting += (s, args) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.DecreaseVolume();
            }
        };
        seekForwardButton.Accepting += (s, args) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.SeekForward();
            }
        };
        seekBackwardButton.Accepting += (s, a) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.SeekBackward();
            }
        };
        
        playbackControls.Add (playPauseButton, stopButton, volumeIncreaseButton, volumeDecreaseButton, seekForwardButton, seekBackwardButton);
        
        #endregion
        
        #region PlaybackInfo
        
        var playbackInfo = new Window()
        {
            X = Pos.Right(playbackControls),
            Y = Pos.Bottom(libraryWindow),
            Width = Dim.Fill(),
            Height = Dim.Auto(),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded
        };
        
        _nowPlayingWindow = new Window
        {
            Title = "Status",
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 4,
            BorderStyle = LineStyle.Rounded,
        };

        _progressBar = new ProgressBar()
        {
            X = 0,
            Y = Pos.Bottom(_nowPlayingWindow),
            Width = Dim.Fill(),
            Height = 3,
            CanFocus = false,
            BorderStyle = LineStyle.Rounded,
            Fraction = 0f,
            ColorScheme = Colors.ColorSchemes ["Error"]
        };
        
        playbackInfo.Add (_progressBar, _nowPlayingWindow);
        
        // Add the views to the main window
        Add(menuBar, libraryWindow, playbackControls, playbackInfo);
    }
    
    #endregion
    
    // Action Methods
    private void OpenFile()
    {
        var d = new OpenDialog()
        {
            AllowsMultipleSelection = false,
            Title = "Open an audio file",
            AllowedTypes = [new AllowedType("Allowed filetypes", ".mp3")]
        };
        
        Application.Run(d);

        if (!d.Canceled)
        {
            _player.Play(d.FilePaths[0]);
            UpdateProgressBar();
            NowPlaying("Test Track");
        }
    }
    
    private void OpenStream()
    {
        var streamDialog = new Dialog
        {
            Title = "Open an audio stream",
        };

        var uriLabel = new Label
        {
            Text = "Enter the stream URI",
            X = Pos.Center(),
            Y = 0,
            Width = Dim.Auto(),
        };

        var streamUrl = new TextField
        {
            X = Pos.Center(),
            Y = Pos.Bottom(uriLabel),
            Width = Dim.Fill(),
            BorderStyle = LineStyle.Rounded,
        };

        var loadStreamButton = new Button
        {
            Text = "Load stream",
            X = Pos.Center(),
            Y = Pos.Bottom(streamUrl),
        };

        var cancelButton = new Button
        {
            Text = "Cancel",
            X = Pos.Right(loadStreamButton),
            Y = Pos.Bottom(streamUrl)
        };

        loadStreamButton.Accepting += (s, args) =>
        {
            _player.Play(streamUrl.Text);
        };

        cancelButton.Accepting += (s, args) =>
        {
            RequestStop();
        };
        
        streamDialog.Add(uriLabel, streamUrl, loadStreamButton, cancelButton);
        
        Application.Run(streamDialog);
    }

    private void UpdateProgressBar()
    {
        _mainLoopTimeout = Application.AddTimeout (
            TimeSpan.FromMilliseconds (_mainLoopTimeoutTick),
            () =>
            {
                while (_player.CurrentTime < _player.TrackLength && _player.PlayerState != EPlayerStatus.Stopped)
                {
                    _progressBar.Fraction = _player.CurrentTime / _player.TrackLength;
                  //  TimePlayedLabel();

                    return true;
                }

                return true;
            }
        );
    }
    
    private void NowPlaying(string trackName)
    {
        var nowPlayingLabel = new Label
        {
            Text = trackName,
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
        };

        _nowPlayingWindow?.Add(nowPlayingLabel);
    }
}