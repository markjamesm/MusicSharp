﻿// <copyright file="Tui.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using MusicSharp.Enums;
using MusicSharp.SoundEngines;
using System;
using System.Collections.Generic;
using System.IO;
using MusicSharp.Models;
using Terminal.Gui;

namespace MusicSharp.View;

/// <summary>
/// The Gui class houses the CLI elements of MusicSharp.
/// </summary>
public class Tui
{
    private static List<string> _playlistTracks;
    private static ListView _playlistView;
    private static FrameView _playlistPane;
    private static FrameView _playbackControls;
    private static FrameView _nowPlaying;
    private static StatusBar _statusBar;
    private static Label _trackName;

    /// <summary>
    /// Create a new instance of the audio player engine.
    /// </summary>
    private readonly IPlayer _player;

    private object _mainLoopTimeout = null;

    private List<string> _playlist = new List<string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="Tui"/> class.
    /// </summary>
    /// <param name="player">The player to be injected.</param>
    public Tui(IPlayer player)
    {
        _player = player;
    }

    /// <summary>
    ///  Gets and sets the current audio file play progress.
    /// </summary>
    internal ProgressBar AudioProgressBar { get; private set; }

    /// <summary>
    /// Start the UI.
    /// </summary>
    public void Start()
    {
        // Creates a instance of MainLoop to process input events, handle timers and other sources of data.
        Application.Init();

        var top = Application.Top;
        var tframe = top.Frame;

        // Create the menubar.
        var menu = new MenuBar(new MenuBarItem[]
        {
            new MenuBarItem("_File", new MenuItem[]
            {
                new MenuItem("_Open", "Open a music file", () => OpenFile()),

                new MenuItem("Open S_tream", "Open a music stream", () => OpenStream()),

                new MenuItem("Open Pla_ylist", "Load a playlist", () => LoadPlaylist()),

                new MenuItem("_Quit", "Exit MusicSharp", () => Application.RequestStop()),
            }),

            new MenuBarItem("_Help", new MenuItem[]
            {
                new MenuItem("_About MusicSharp", string.Empty, () =>
                {
                    MessageBox.Query("Music Sharp 0.7.5", "\nMusic Sharp is a lightweight CLI\n music player written in C#.\n\nDeveloped by Mark-James McDougall\nand licensed under the GPL v3.\n ", "Close");
                }),
            }),
        });

        _statusBar = new StatusBar(new StatusItem[]
        {
                new StatusItem(Key.F1, "~F1~ Open file", () => OpenFile()),
                new StatusItem(Key.F2, "~F2~ Open stream", () => OpenStream()),
                new StatusItem(Key.F3, "~F3~ Load playlist", () => LoadPlaylist()),
                new StatusItem(Key.F4, "~F4~ Quit", () => Application.RequestStop()),
                new StatusItem(Key.Space, "~Space~ Play/Pause", () => PlayPause()),
        });

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

            if (_player.PlayerStatus != ePlayerStatus.Stopped)
            {
                UpdateProgressBar();
            }
        };

        var stopButton = new Button(16, 1, "Stop");
        stopButton.Clicked += () =>
        {
            _player.Stop();
            AudioProgressBar.Fraction = 0F;
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
            _player.LastFileOpened = a.Value.ToString();
            _player.PlayFromPlaylist(_player.LastFileOpened);
            NowPlaying(_player.LastFileOpened);
            UpdateProgressBar();
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

        AudioProgressBar = new ProgressBar()
        {
            X = 0,
            Y = 2,
            Width = Dim.Fill() - 15,
            Height = 1,
            ColorScheme = Colors.Base,
        };

        _nowPlaying.Add(AudioProgressBar);

        // Add the layout elements and run the app.
        top.Add(menu, _playlistPane, _playbackControls, _nowPlaying, _statusBar);

        Application.Run();
    }

    private void PlayPause()
    {
        try
        {
            _player.PlayPause();

            if (_player.PlayerStatus == ePlayerStatus.Playing)
            {
                UpdateProgressBar();
            }
        }
        catch (Exception)
        {
            MessageBox.Query("Warning", "Select a file or stream first.", "Close");
        }
    }

    // Display a file open dialog and return the path of the user selected file.
    private void OpenFile()
    {
        var d = new OpenDialog("Open", "Open an audio file") { AllowsMultipleSelection = false };

        d.DirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        // This will filter the dialog on basis of the allowed file types in the array.
        d.AllowedFileTypes = new string[] { ".mp3", ".wav", ".flac" };
        Application.Run(d);

        if (!d.Canceled)
        {
            if (File.Exists(d.FilePath.ToString()))
            {
                _player.LastFileOpened = d.FilePath.ToString();
                _player.OpenFile(_player.LastFileOpened);
                NowPlaying(_player.LastFileOpened);
                AudioProgressBar.Fraction = 0F;
                UpdateProgressBar();
                TimePlayedLabel();
            }
            else
            {
                // This is a good spot for an error message, should one be wanted/needed
            }
        }
    }

    // Open and play an audio stream.
    private void OpenStream()
    {
        var d = new Dialog("Open Stream", 50, 15);

        var editLabel = new Label("Enter the url of the audio stream to load:\n(.mp3 only)")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
        };

        var streamURL = new TextField(string.Empty)
        {
            X = 3,
            Y = 4,
            Width = 42,
        };

        var loadStream = new Button(12, 7, "Load Stream");
        loadStream.Clicked += () =>
        {
            _player.OpenStream(streamURL.Text.ToString());
            Application.RequestStop();
        };

        var cancelStream = new Button(29, 7, "Cancel");
        cancelStream.Clicked += () =>
        {
            Application.RequestStop();
        };

        d.AddButton(loadStream);
        d.AddButton(cancelStream);
        d.Add(editLabel, streamURL);
        Application.Run(d);
    }

    // Load a playlist file. Currently, only M3U is supported.
    private void LoadPlaylist()
    {
        var d = new OpenDialog("Open", "Open a playlist") { AllowsMultipleSelection = false };

        // This will filter the dialog on basis of the allowed file types in the array.
        d.AllowedFileTypes = new string[] { ".m3u" };
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

    private void NowPlaying(string track)
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
        if (_player.PlayerStatus != ePlayerStatus.Stopped)
        {
            var timePlayed = _player.CurrentTime().ToString(@"mm\:ss");
            var trackLength = _player.TrackLength().ToString(@"mm\:ss");
            _trackName = new Label($"{timePlayed} / {trackLength}")
            {
                X = Pos.Right(AudioProgressBar),
                Y = 2,
            };
        }
        else
        {
            _trackName = new Label($"00:00 / 00:00")
            {
                X = Pos.Right(AudioProgressBar),
                Y = 2,
            };
        }

        _nowPlaying.Add(_trackName);
    }

    private void UpdateProgressBar()
    {
        _mainLoopTimeout = Application.MainLoop.AddTimeout(TimeSpan.FromSeconds(1), (updateTimer) =>
        {
            while (_player.CurrentTime().Seconds < _player.TrackLength().TotalSeconds && _player.PlayerStatus is not ePlayerStatus.Stopped)
            {
                AudioProgressBar.Fraction = (float)(_player.CurrentTime().Seconds / _player.TrackLength().TotalSeconds);
                TimePlayedLabel();

                return true;
            }

            return false;
        });
    }
}