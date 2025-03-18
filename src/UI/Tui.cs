// <copyright file="Tui.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using MusicSharp.Enums;
using MusicSharp.PlaylistHandlers;
using MusicSharp.AudioPlayer;
using Terminal.Gui;

namespace MusicSharp.UI;

public class Tui
{
    // TUI Components
    private static List<string> _playlistTracks;
    private static ListView _playlistView;
    private static FrameView _playlistPane;
    private static FrameView _playbackControls;
    private static FrameView _nowPlaying;
    private static StatusBar _statusBar;
    private static Label _trackName;
    private ProgressBar _audioProgressBar;
    private object? _mainLoopTimeout = null;
    private List<string> _playlist = new List<string>();
    
    private readonly IPlayer _player;
    private readonly IStreamConverter _streamConverter;
    
    public Tui(IPlayer player, IStreamConverter streamConverter)
    {
        _player = player;
        _streamConverter = streamConverter;
    }
    
    public void Start()
    {
        // Creates an instance of MainLoop to process input events, handle timers and other sources of data.
        Application.Init();

        var top = Application.Top;

        // Create the menubar.
        var menu = new MenuBar([
            new MenuBarItem("_File", [
                new MenuItem("_Open", "Open a music file", () => OpenFile()),

                new MenuItem("Open _Stream", "Open a music stream", () => OpenStream()),

                new MenuItem("Open _Playlist", "Load a playlist", () => LoadPlaylist()),

                new MenuItem("_Quit", "Exit MusicSharp", () => Application.RequestStop())
            ]),

            new MenuBarItem("_Help", [
                new MenuItem("_About MusicSharp", string.Empty, () =>
                {
                    MessageBox.Query("Music Sharp 1.0.0", "\nMusic Sharp is a lightweight CLI\n music player written in C#.\n\nDeveloped by Mark-James McDougall\nand licensed under the GPL v3.\n ", "Close");
                })
            ])
        ]);

        _statusBar = new StatusBar([
            new StatusItem(Key.F1, "~F1~ Open file", () => OpenFile()),
                new StatusItem(Key.F2, "~F2~ Open stream", () => OpenStream()),
                new StatusItem(Key.F3, "~F3~ Load playlist", () => LoadPlaylist()),
                new StatusItem(Key.F4, "~F4~ Quit", () => Application.RequestStop()),
                new StatusItem(Key.Space, "~Space~ Play/Pause", () => PlayPause())
        ]);

        // Create the playback controls frame.
        _playbackControls = new FrameView("Playback")
        {
            X = 0,
            Y = 24,
            Width = 55,
            Height = 5,
            CanFocus = true,
        };

        var playPauseButton = new Button(1, 1, "Play/Pause");
        playPauseButton.Clicked += () =>
        {
            PlayPause();

            if (_player.PlayerStatus != EPlayerStatus.Stopped)
            {
                UpdateProgressBar();
            }
        };

        var stopButton = new Button(16, 1, "Stop");
        stopButton.Clicked += () =>
        {
            _player.Stop();
            _audioProgressBar.Fraction = 0F;
            TimePlayedLabel();
        };

        var seekForward = new Button(26, 0, "Seek  5s");
        seekForward.Clicked += () =>
        {
            _player.SeekForward();
        };

        var seekBackward = new Button(26, 2, "Seek -5s");
        seekBackward.Clicked += () =>
        {
            _player.SeekBackwards();
        };

        var increaseVolumeButton = new Button(39, 0, "+ Volume");
        increaseVolumeButton.Clicked += () =>
        {
            _player.IncreaseVolume();
        };

        var decreaseVolumeButton = new Button(39, 2, "- Volume");
        decreaseVolumeButton.Clicked += () =>
        {
            _player.DecreaseVolume();
        };

        _playbackControls.Add(playPauseButton, stopButton, increaseVolumeButton, decreaseVolumeButton, seekBackward, seekForward);

        // Create the left-hand playlists view.
        _playlistPane = new FrameView("Playlist Tracks")
        {
            X = 0,
            Y = 1, // for menu
            Width = Dim.Fill(),
            Height = 23,
            CanFocus = false,
        };

        // The list of tracks in the playlist.
        _playlistTracks = new List<string>();

        _playlistView = new ListView(_playlistTracks)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 23,
            AllowsMarking = false,
            CanFocus = true,
        };

        // Play the selection when a playlist path is clicked.
        _playlistView.OpenSelectedItem += (a) =>
        {
            try
            {
                _player.LastFileOpened = a.Value.ToString();
                _player.Play(_streamConverter.ConvertFileToStream(a.Value.ToString()));
                NowPlaying(_player.LastFileOpened);
                UpdateProgressBar();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Query("Warning", "Invalid file path.", "Close");
            }
        };

        _playlistPane.Add(_playlistView);

        // Create the audio progress bar frame.
        _nowPlaying = new FrameView("Now Playing")
        {
            X = 55,
            Y = 24,
            Width = Dim.Fill(),
            Height = 5,
            CanFocus = false,
        };

        _audioProgressBar = new ProgressBar()
        {
            X = 0,
            Y = 2,
            Width = Dim.Fill() - 15,
            Height = 1,
            ColorScheme = Colors.Base,
        };

        _nowPlaying.Add(_audioProgressBar);

        // Add the layout elements and run the app.
        top.Add(menu, _playlistPane, _playbackControls, _nowPlaying, _statusBar);

        Application.Run();
    }

    private void PlayPause()
    {
        try
        {
            _player.PlayPause();

            if (_player.PlayerStatus == EPlayerStatus.Playing)
            {
                UpdateProgressBar();
            }
        }
        catch (Exception)
        {
            MessageBox.Query("Warning", "Select a file or stream first.", "Close");
        }
    }
    
    private void OpenFile()
    {
        var d = new OpenDialog("Open", "Open an audio file") { AllowsMultipleSelection = false };
        d.DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        d.AllowedFileTypes = [".mp3", ".wav", ".flac"];
        Application.Run(d);

        if (!d.Canceled)
        {
            if (File.Exists(d.FilePath.ToString()))
            {
                try
                {
                    _player.LastFileOpened = d.FilePath.ToString();
                    var stream = _streamConverter.ConvertFileToStream(d.FilePath.ToString());
                    _player.Play(stream);
                    NowPlaying(_player.LastFileOpened);
                    _audioProgressBar.Fraction = 0F;
                    UpdateProgressBar();
                    TimePlayedLabel();
                }
                catch (FileNotFoundException ex)
                {
                    MessageBox.Query("Warning", "Invalid file path.", "Close");
                }
            }
        }
    }

    // Open and play an audio stream.
    private void OpenStream()
    {
        var d = new Dialog("Open Stream", 50, 15);

        var editLabel = new Label("Enter the url of the audio stream to load\n (.mp3 streams only):")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
        };

        var streamUrl = new TextField(string.Empty)
        {
            X = 3,
            Y = 4,
            Width = 42,
        };

        var loadStream = new Button(12, 7, "Load Stream");
        loadStream.Clicked += async () =>
        {
            try
            {
                var stream = await _streamConverter.ConvertUrlToStream(streamUrl.Text.ToString());
                _player.Play(stream);
            }
            catch (Exception ex)
            {
                MessageBox.Query("Warning", "Invalid URL.", "Close");
            }
        };

        var cancelStream = new Button(29, 7, "Close");
        cancelStream.Clicked += () =>
        {
            Application.RequestStop();
        };

        d.AddButton(loadStream);
        d.AddButton(cancelStream);
        d.Add(editLabel, streamUrl);
        Application.Run(d);
    }
    
    private void LoadPlaylist()
    {
        var d = new OpenDialog("Open", "Open a playlist") { AllowsMultipleSelection = false };

        // This will filter the dialog on basis of the allowed file types in the array.
        d.AllowedFileTypes = [".m3u"];
        Application.Run(d);

        if (!d.Canceled)
        {
            _playlist = PlaylistLoader.LoadPlaylist(d.FilePath.ToString());

            if (_playlist == null)
            {
                Application.RequestStop();
            }
            else
            {
                foreach (var track in _playlist)
                {
                    _playlistTracks.Add(track);
                }

                Application.Run();
            }
        }
    }

    private static void NowPlaying(string track)
    {
        _trackName = new Label(track)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
        };

        _nowPlaying.Add(_trackName);
    }

    private void TimePlayedLabel()
    {
        if (_player.PlayerStatus != EPlayerStatus.Stopped)
        {
            var timePlayed = TimeSpan.FromSeconds((double)(new decimal(_player.CurrentTime()))).ToString(@"hh\:mm\:ss");
            var trackLength = TimeSpan.FromSeconds((double)(new decimal(_player.TrackLength()))).ToString(@"hh\:mm\:ss");
            
            _trackName = new Label($"{timePlayed} / {trackLength}")
            {
                X = Pos.Right(_audioProgressBar),
                Y = 2,
            };
        }
        else
        {
            _trackName = new Label($"00:00 / 00:00")
            {
                X = Pos.Right(_audioProgressBar),
                Y = 2,
            };
        }

        _nowPlaying.Add(_trackName);
    }

    private void UpdateProgressBar()
    {
        _mainLoopTimeout = Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), (updateTimer) =>
        {
            while (_player.CurrentTime() < _player.TrackLength() && _player.PlayerStatus is not EPlayerStatus.Stopped)
            {
                _audioProgressBar.Fraction = _player.CurrentTime() / _player.TrackLength();
                TimePlayedLabel();

                return true;
            }

            return false;
        });
    }
}