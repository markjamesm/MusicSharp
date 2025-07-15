using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using MusicSharp.AudioPlayer;
using SoundFlow.Enums;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace MusicSharp.UI;

public class Tui : Toplevel
{
    private readonly IPlayer _player;
    private readonly ProgressBar _progressBar;
    private object? _mainLoopTimeout;
    private readonly Label _nowPlayingLabel;
    private readonly Label _timePlayedLabel;
    private readonly ListView? _libraryListView;
    private readonly ObservableCollection<string> _playlistTracks = [];
    
    private const uint MainLoopTimeoutTick = 100; // ms

    public Tui(IPlayer player)
    {
        _player = player;

        var menuBar = new MenuBarv2()
        {
            Title = "MusicSharp",
            BorderStyle = LineStyle.Rounded,
            Menus = 
                [ 
                    new MenuBarItemv2(
                                        Title = "_File",
                                        new MenuItemv2[]
                                        {
                                            new("_Open file", "Open audio file", OpenFile),
                                            new("Open _stream", "Open stream", OpenStream),
                                            new("_Quit", "Quit MusicSharp", RequestStop)
                                        }
                    ),
                    new MenuBarItemv2(
                                        Title = "Playlist",
                                        new MenuItemv2[]
                                        {
                                            new("Open _playlist", "Open a playlist", OpenPlaylist),
                                            new("_Save playlist", "Save files to playlist", SavePlaylist)
                                        }
                    ),
                    new MenuBarItemv2(
                        Title = "About",
                        new MenuItemv2[]
                        {
                            new("_About", "About MusicSharp", AboutDialog)
                        }
                    ),
                ]
        };

        var statusBar = new StatusBar([
            new Shortcut
            {
                Text = "Quit",
                Key = Key.Esc,
                Action = RequestStop
            },
            new Shortcut
            {
                Text = "Open file",
                Key = Key.F1,
                Action = OpenFile
            },
            new Shortcut
            {
                Text = "Open stream",
                Key = Key.F2,
                Action = OpenStream
            },
            new Shortcut
            {
                Text = "Open playlist",
                Key = Key.F3,
                Action = OpenPlaylist
            }
        ]);

        _libraryListView = new ListView
        {
            Title = "Library",
            X = 0,
            Y = Pos.Bottom(menuBar),
            Width = Dim.Fill(),
            Height = 12,
            CanFocus = false,
            BorderStyle = LineStyle.Rounded,
            Source = new ListWrapper<string>(_playlistTracks)
        };

        _libraryListView.OpenSelectedItem += (sender, args) =>
        {
            if (args.Value != null)
            {
                PlayHandler(args.Value.ToString()!);
            }
        };

        _progressBar = new ProgressBar()
        {
            Title = "Progress",
            X = 0,
            Y = Pos.Bottom(_libraryListView),
            Width = Dim.Fill(),
            Height = 3,
            CanFocus = false,
            BorderStyle = LineStyle.Rounded,
            Fraction = 0f,
        };

        #region PlayBackControls

        var playbackControls = new View()
        {
            Title = "Playback",
            X = 0,
            Y = Pos.Bottom(_progressBar),
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

        playPauseButton.Accepting += (s, e) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.PlayPause();
            }
            
            e.Handled = true;
        };
        stopButton.Accepting += (s, e) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.Stop();
            }
            
            e.Handled = true;
        };
        volumeIncreaseButton.Accepting += (s, e) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.IncreaseVolume();
            }
            
            e.Handled = true;
        };
        volumeDecreaseButton.Accepting += (s, e) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.DecreaseVolume();
            }
            
            e.Handled = true;
        };
        seekForwardButton.Accepting += (s, e) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.SeekForward();
            }
            
            e.Handled = true;
        };
        seekBackwardButton.Accepting += (s, e) =>
        {
            if (_player.IsStreamLoaded)
            {
                _player.SeekBackward();
            }
            
            e.Handled = true;
        };

        playbackControls.Add(playPauseButton, stopButton, volumeIncreaseButton, volumeDecreaseButton, seekForwardButton,
            seekBackwardButton);

        #endregion

        #region PlaybackInfo

        var nowPlayingView = new View
        {
            Title = "Now playing",
            X = Pos.Right(playbackControls),
            Y = Pos.Bottom(_progressBar),
            Width = Dim.Fill(),
            Height = Dim.Height(playbackControls),
            BorderStyle = LineStyle.Rounded,
        };

        _nowPlayingLabel = new Label
        {
            Text = string.Empty,
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
        };

        nowPlayingView?.Add(_nowPlayingLabel);

        _timePlayedLabel = new Label
        {
            Text = $"00:00 / 00:00",
            X = 0,
            Y = Pos.Bottom(_nowPlayingLabel) + 1,
        };

        nowPlayingView?.Add(_nowPlayingLabel, _timePlayedLabel);

        // Add the views to the main window
        Add(menuBar, _libraryListView, _progressBar, playbackControls, nowPlayingView, statusBar);
    }

    #endregion

    #region ActionMethods

    private void PlayHandler(string filePath)
    {
        _player.Play(filePath);
        RunMainLoop();
        NowPlaying(FileData.TrackData.GetTrackAndArtistName(filePath));
    }

    private void OpenFile()
    {
        var d = new OpenDialog()
        {
            AllowsMultipleSelection = false,
            Title = "Open an audio file",
            AllowedTypes = [new AllowedType("Allowed filetypes", ".mp3", ".flac", ".wav")]
        };

        Application.Run(d);

        if (!d.Canceled)
        {
            _player.Play(d.FilePaths[0]);
            PlayHandler(d.FilePaths[0]);
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

        loadStreamButton.Accepting += (s, e) =>
        {
            if (streamUrl.Text != string.Empty)
            {
                _player.Play(streamUrl.Text); 
            }
            
            e.Handled = true;
        };

        cancelButton.Accepting += (s, e) =>
        {
            RequestStop();
            e.Handled = true;
        };

        streamDialog.Add(uriLabel, streamUrl, loadStreamButton, cancelButton);

        Application.Run(streamDialog);
    }

    private void RunMainLoop()
    {
        _mainLoopTimeout = Application.AddTimeout(
            TimeSpan.FromMilliseconds(MainLoopTimeoutTick),
            () =>
            {
                while (_player.CurrentTime < _player.TrackLength && _player.State != PlaybackState.Stopped)
                {
                    _progressBar.Fraction = _player.CurrentTime / _player.TrackLength;
                    TimePlayedLabel();

                    return true;
                }

                return true;
            }
        );
    }

    private void NowPlaying(string trackName)
    {
        _nowPlayingLabel.Text = trackName;
    }

    private void OpenPlaylist()
    {
        var d = new OpenDialog()
        {
            AllowsMultipleSelection = false,
            Title = "Open a playlist",
            AllowedTypes = [new AllowedType("Allowed filetypes", ".m3u")]
        };

        Application.Run(d);

        if (!d.Canceled)
        {
            var playlist = Playlist.LoadPlaylist(d.FilePaths[0]);

            foreach (var track in playlist)
            {
                _playlistTracks.Add(track);
            }
        }
    }

    private void SavePlaylist()
    {
        var d = new SaveDialog
        {
            AllowsMultipleSelection = false,
            AllowedTypes = [new AllowedType("Allowed filetypes", ".m3u")],
            Title = "Save playlist in M3U format"
        };

        Application.Run(d);

        if (!d.Canceled)
        {
            var currentTracks = _playlistTracks.ToList();
            Playlist.SavePlaylistToFile(d.FileName, currentTracks);
        }
    }

    private void TimePlayedLabel()
    {
        if (_player.State != PlaybackState.Stopped)
        {
            if (_player.TrackLength > 3599)
            {
                var timePlayed = TimeSpan.FromSeconds((double)new decimal(_player.CurrentTime)).ToString(@"hh\:mm\:ss");
                var trackLength = TimeSpan.FromSeconds((double)new decimal(_player.TrackLength))
                    .ToString(@"hh\:mm\:ss");

                _timePlayedLabel.Text = $"{timePlayed} / {trackLength}";
            }

            else
            {
                var timePlayed = TimeSpan.FromSeconds((double)new decimal(_player.CurrentTime)).ToString(@"mm\:ss");
                var trackLength = TimeSpan.FromSeconds((double)new decimal(_player.TrackLength)).ToString(@"mm\:ss");

                _timePlayedLabel.Text = $"{timePlayed} / {trackLength}";
            }
        }
        else
        {
            _timePlayedLabel.Text = $"00:00 / 00:00";
        }
    }

    private void AboutDialog()
    {
        var aboutDialog = new Dialog
        {
            X = Pos.Center(),
            Y = Pos.Center(),
        };

        var sb = new StringBuilder();
        sb.Append("""
                      __  ___           _      _____ __                   
                     /  |/  /_  _______(_)____/ ___// /_  ____ __________ 
                    / /|_/ / / / / ___/ / ___/\__ \/ __ \/ __ `/ ___/ __ \
                   / /  / / /_/ (__  ) / /__ ___/ / / / / /_/ / /  / /_/ /
                  /_/  /_/\__,_/____/_/\___//____/_/ /_/\__,_/_/  / .___/ 
                                                                 /_/      
                  """);

        var asciiLabel = new Label
        {
            Text = sb.ToString(),
            X = Pos.Center(),
            Y = 0,
        };

        var infoLabel = new Label
        {
            Text = "MusicSharp v2.0.0\nCreated by Mark-James M.",
            X = Pos.Center(),
            Y = Pos.Bottom(asciiLabel),
            Width = Dim.Auto(),
            Height = Dim.Auto()
        };

        var closeButton = new Button
        {
            Text = "Close",
            X = Pos.Center(),
        };

        closeButton.Accepting += (s, e) =>
        {
            RequestStop();
            e.Handled = true;
        };

        aboutDialog.Add(asciiLabel, infoLabel);
        aboutDialog.AddButton(closeButton);

        Application.Run(aboutDialog);
    }

    #endregion
}