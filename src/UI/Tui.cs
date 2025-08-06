using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using MusicSharp.AudioPlayer;
using MusicSharp.Data;
using MusicSharp.Enums;
using MusicSharp.Playlist;
using SoundFlow.Enums;
using Terminal.Gui.App;
using Terminal.Gui.Drawing;
using Terminal.Gui.Input;
using Terminal.Gui.Text;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace MusicSharp.UI;

public class Tui : Toplevel
{
    private readonly IPlayer _player;
    private readonly ProgressBar _progressBar;
    private readonly Label _nowPlayingLabel;
    private readonly Label _timePlayedLabel;
    private readonly Button _playPauseButton;
    private readonly ListView _playlistView;
    private readonly ObservableCollection<AudioFile>? _loadedPlaylist = [];
    private int _playlistIndex;
    private object? _mainLoopTimeout;

    private const uint MainLoopTimeoutTick = 100; // ms

    public Tui(IPlayer player)
    {
        _player = player;

        #region Menus

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
                        new("Open _stream", "Open a stream URL", OpenStream),
                        new("_Quit", "Quit MusicSharp", RequestStop)
                    }
                ),
                new MenuBarItemv2(
                    Title = "Playlist",
                    new MenuItemv2[]
                    {
                        new("_Add to playlist", "Add track(s) to playlist", AddToPlaylist),
                        new("_Remove from playlist", "Remove selected track from playlist", RemoveFromPlaylist),
                        new("Open _playlist", "Open a playlist", OpenPlaylist),
                        new("_Save playlist", "Save to playlist", SavePlaylist)
                    }
                ),
                new MenuBarItemv2(
                    Title = "Help",
                    new MenuItemv2[]
                    {
                        new("_About...", "About MusicSharp", () => MessageBox.Query(
                                "",
                                GetAboutMessage(),
                                wrapMessage: false,
                                buttons: "_Ok"
                            ),
                            Key.A.WithCtrl
                        )
                    }
                ),
            ]
        };

        var statusBar = new StatusBar([
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
                Text = "Load playlist",
                Key = Key.F3,
                Action = OpenPlaylist
            },
            new Shortcut
            {
                Text = "Quit",
                Key = Key.Esc,
                Action = RequestStop
            },
        ]);

        #endregion Menus

        _playlistView = new ListView
        {
            Title = "Playlist",
            X = 0,
            Y = Pos.Bottom(menuBar),
            Width = Dim.Fill(),
            Height = Dim.Fill(11),
            CanFocus = true,
            BorderStyle = LineStyle.Rounded,
            Source = new TrackListDataSource(_loadedPlaylist),
            AllowsMarking = false,
            AllowsMultipleSelection = false
        };
        _playlistView.RowRender += PlaylistView_RowRender;
        _playlistView.VerticalScrollBar.AutoShow = true;
        _playlistView.HorizontalScrollBar.AutoShow = true;
        _playlistView.OpenSelectedItem += (sender, args) =>
        {
            if (args.Value != null)
            {
                _playlistIndex = args.Item;
                PlayHandler((AudioFile)args.Value);
            }
        };

        _progressBar = new ProgressBar()
        {
            Title = "Progress",
            X = 0,
            Y = Pos.Bottom(_playlistView),
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

        var seekBackwardButton = new Button
        {
            X = 0,
            Y = 0,
            CanFocus = true,
            Text = "<<"
        };
        seekBackwardButton.Accepting += (s, e) =>
        {
            _player.SeekBackward();
            e.Handled = true;
        };

        _playPauseButton = new Button
        {
            X = Pos.Right(seekBackwardButton),
            Y = 0,
            CanFocus = true,
            Text = "▶"
        };
        _playPauseButton.Accepting += (s, e) =>
        {
            var selected = _playlistView.SelectedItem;
            _playlistIndex = selected;
            var selectedTrack = _loadedPlaylist?.ElementAtOrDefault(selected);

            if (selectedTrack != null)
            {
                PlayHandler(selectedTrack);
            }

            if (selectedTrack == null && _player.NowPlaying != null)
            {
                PlayHandler(_player.NowPlaying);
            }

            e.Handled = true;
        };

        var seekForwardButton = new Button
        {
            X = Pos.Right(_playPauseButton),
            Y = 0,
            CanFocus = true,
            Text = ">>"
        };
        seekForwardButton.Accepting += (s, e) =>
        {
            _player.SeekForward();
            e.Handled = true;
        };

        var stopButton = new Button
        {
            X = Pos.Right(seekForwardButton),
            Y = 0,
            CanFocus = true,
            Text = "⏹︎",
        };
        stopButton.Accepting += (s, e) =>
        {
            _player.Stop();
            _progressBar.Fraction = 0;
            TimePlayedLabel();
            _nowPlayingLabel!.Text = string.Empty;
            _playPauseButton.Text = "▶";
            e.Handled = true;
        };

        // Verify what SoundFlow's max volume level is
        // For now this should be enough based on testing
        List<object> volumeOptions =
        [
            0f, .1f, .2f, .3f, .4f, .5f, .6f, .7f, .8f, 1.0f, 1.2f, 1.4f
        ];

        var volumeSlider = new Slider(volumeOptions)
        {
            Title = "Volume",
            X = 0,
            Y = Pos.Bottom(_playPauseButton),
            Width = Dim.Fill(),
            Height = Dim.Auto(),
            Type = SliderType.LeftRange,
            AllowEmpty = false,
            ShowLegends = false,
            BorderStyle = LineStyle.Rounded,
        };
        volumeSlider.SetOption(6);
        volumeSlider.OptionsChanged += (s, e) =>
        {
            var value = e.Options.FirstOrDefault().Value;
            var volumeLevel = (float)value.Data;
            _player.ChangeVolume(volumeLevel);
        };

        playbackControls.Add(_playPauseButton, stopButton, seekForwardButton, seekBackwardButton,
            volumeSlider);
        
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
            Height = Dim.Fill(),
            TextDirection = TextDirection.LeftRight_TopBottom
        };
        _nowPlayingLabel.TextFormatter.WordWrap = true;

        _timePlayedLabel = new Label
        {
            Text = "00:00 / 00:00",
            X = Pos.Align(Alignment.End),
            Y = Pos.Align(Alignment.End),
        };

        nowPlayingView.Add(_nowPlayingLabel, _timePlayedLabel);

        // Add the views to the main window
        Add(menuBar, _playlistView, _progressBar, playbackControls, nowPlayingView, statusBar);
    }

    #endregion

    private void PlayHandler(AudioFile audioFile)
    {
        _player.PlayPause(audioFile);

        _playPauseButton.Text = _player.State switch
        {
            PlaybackState.Stopped => "▶",
            PlaybackState.Playing => "⏸︎",
            PlaybackState.Paused => "▶",
            _ => _playPauseButton.Text
        };

        if (_player.State == PlaybackState.Playing)
        {
            _nowPlayingLabel.Text = audioFile.Type switch
            {
                EFileType.File =>
                    $"{(string.IsNullOrWhiteSpace(audioFile.TrackInfo.Title) ? "Unknown" : audioFile.TrackInfo.Title)} - " +
                    $"{(string.IsNullOrWhiteSpace(audioFile.TrackInfo.Artist) ? "Unknown" : audioFile.TrackInfo.Artist)} - " +
                    $"{(string.IsNullOrWhiteSpace(audioFile.TrackInfo.Album) ? "Unknown" : audioFile.TrackInfo.Album)}",
                EFileType.Stream => $"Web stream: {audioFile.Path}",
                _ => _nowPlayingLabel.Text
            };

            RunMainLoop();
        }
    }

    private void AutoPlayNextTrack()
    {
        if (Math.Abs(_player.TrackLength - _player.CurrentTime) < 0.5f)
        {
            if (_playlistIndex + 1 < _loadedPlaylist?.Count)
            {
                var nextTrack = _loadedPlaylist.ElementAtOrDefault(_playlistIndex + 1);

                if (nextTrack != null)
                {
                    PlayHandler(nextTrack);
                    _playlistIndex++;
                }
            }
        }
    }

    #region OpenMethods

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
            var audioFile = new AudioFile(d.FilePaths[0], EFileType.File);
            PlayHandler(audioFile);
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
            Text = "Open stream",
            X = Pos.Center(),
            Y = Pos.Bottom(streamUrl),
        };

        loadStreamButton.Accepting += (s, e) =>
        {
            if (streamUrl.Text != string.Empty)
            {
                var audioFile = new AudioFile(streamUrl.Text, EFileType.Stream);
                PlayHandler(audioFile);
            }

            e.Handled = true;
            RequestStop();
        };

        var closeButton = new Button
        {
            Text = "Close",
            X = Pos.Right(loadStreamButton),
            Y = Pos.Bottom(streamUrl)
        };

        closeButton.Accepting += (s, e) =>
        {
            e.Handled = true;
            RequestStop();
        };

        streamDialog.Add(uriLabel, streamUrl, loadStreamButton, closeButton);
        Application.Run(streamDialog);
    }

    #endregion

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

                AutoPlayNextTrack();
                return false;
            }
        );
    }

    #region PlaylistMethods

    private void AddToPlaylist()
    {
        var d = new OpenDialog()
        {
            AllowsMultipleSelection = true,
            Title = "Add tracks to playlist",
            AllowedTypes = [new AllowedType("Allowed filetypes", ".mp3", ".flac", ".wav")]
        };

        Application.Run(d);

        if (!d.Canceled)
        {
            foreach (var filepath in d.FilePaths)
            {
                var track = new AudioFile(filepath, EFileType.File);
                _loadedPlaylist?.Add(track);
            }
        }
    }

    private void RemoveFromPlaylist()
    {
        var s = _playlistView.SelectedItem;
        _loadedPlaylist?.RemoveAt(s);
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
            var playlist = PlaylistHelpers.LoadPlaylist(d.FilePaths[0]);

            foreach (var track in playlist.Select(filepath => new AudioFile(filepath, EFileType.File)))
            {
                _loadedPlaylist?.Add(track);
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
            if (_loadedPlaylist != null)
            {
                var currentTracks = _loadedPlaylist.ToList();
                PlaylistHelpers.SavePlaylistToFile(d.FileName, currentTracks);
            }
        }
    }

    private void PlaylistView_RowRender(object? sender, ListViewRowEventArgs obj)
    {
        if (obj.Row == _playlistView.SelectedItem)
        {
            obj.RowAttribute = new Attribute(Color.White, Color.Blue);

            return;
        }

        if (_playlistView.AllowsMarking && _playlistView.Source.IsMarked(obj.Row))
        {
            obj.RowAttribute = new Attribute(Color.Black, Color.White);

            return;
        }

        if (obj.Row % 2 == 0)
        {
            obj.RowAttribute = new Attribute(Color.Green, Color.Black);
        }
        else
        {
            obj.RowAttribute = new Attribute(Color.Black, Color.Green);
        }
    }

    #endregion

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
            _timePlayedLabel.Text = "00:00 / 00:00";
        }
    }

    private static string GetAboutMessage()
    {
        var sb = new StringBuilder();
        sb.AppendLine("""
                          __  ___           _      _____ __                   
                         /  |/  /_  _______(_)____/ ___// /_  ____ __________ 
                        / /|_/ / / / / ___/ / ___/\__ \/ __ \/ __ `/ ___/ __ \
                       / /  / / /_/ (__  ) / /__ ___/ / / / / /_/ / /  / /_/ /
                      /_/  /_/\__,_/____/_/\___//____/_/ /_/\__,_/_/  / .___/ 
                                                                     /_/      
                      """);
        sb.AppendLine();
        sb.AppendLine("MusicSharp v2.0.0");
        sb.AppendLine("Created by Mark-James M.");

        return sb.ToString();
    }

    #region IListDataSource

    private class TrackListDataSource : IListDataSource
    {
        private const int TitleColumnWidth = 40;
        private const int ArtistColumnWidth = 30;
        private const int AlbumColumnWidth = 40;
        private int _count;
        private BitArray _marks;
        private ObservableCollection<AudioFile>? _loadedPlaylist;

        public TrackListDataSource(ObservableCollection<AudioFile>? audioFiles)
        {
            AudioFiles = audioFiles;
        }

        private ObservableCollection<AudioFile>? AudioFiles
        {
            get => _loadedPlaylist;
            set
            {
                if (value != null)
                {
                    _count = value.Count;
                    _marks = new BitArray(_count);
                    _loadedPlaylist = value;
                    Length = GetMaxLengthItem();
                }
            }
        }

        public bool IsMarked(int item)
        {
            if (item >= 0 && item < _count)
            {
                return _marks[item];
            }

            return false;
        }
#pragma warning disable CS0067
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#pragma warning restore CS0067

        public int Count => AudioFiles?.Count ?? 0;
        public int Length { get; private set; }

        public bool SuspendCollectionChangedEvent
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public void Render(
            ListView container,
            bool selected,
            int item,
            int col,
            int line,
            int width,
            int start = 0
        )
        {
            container.Move(col, line);

            // Equivalent to an interpolated string like $"{AudioFiles[item].Name, -widtestname}"; if it were possible
            var trackTitle = string.Format(
                string.Format("{{0,{0}}}", -TitleColumnWidth),
                AudioFiles?[item].TrackInfo.Title
            );

            var artist = string.Format(
                string.Format("{{0,{0}}}", -ArtistColumnWidth),
                AudioFiles?[item].TrackInfo.Artist
            );

            var album = string.Format(
                string.Format("{{0,{0}}}", -AlbumColumnWidth),
                AudioFiles?[item].TrackInfo.Album
            );

            RenderUstr(container, $"{trackTitle} {artist} {album}", col, line, width, start);
        }

        public void SetMark(int item, bool value)
        {
            if (item >= 0 && item < _count)
            {
                _marks[item] = value;
            }
        }

        public IList ToList()
        {
            return AudioFiles;
        }

        private int GetMaxLengthItem()
        {
            if (_loadedPlaylist?.Count == 0)
            {
                return 0;
            }

            var maxLength = 0;

            for (var i = 0; i < _loadedPlaylist.Count; i++)
            {
                var trackTitle = string.Format(
                    $"{{0,{-TitleColumnWidth}}}",
                    AudioFiles?[i].TrackInfo.Title
                );

                var artist = string.Format(
                    $"{{0,{-ArtistColumnWidth}}}",
                    AudioFiles?[i].TrackInfo.Artist
                );

                var album = string.Format(
                    $"{{0,{-AlbumColumnWidth}}}",
                    AudioFiles?[i].TrackInfo.Album
                );

                var sc = $"{trackTitle} {artist} {album}";
                var l = sc.Length;

                if (l > maxLength)
                {
                    maxLength = l;
                }
            }

            return maxLength;
        }

        // A slightly adapted method from: https://github.com/gui-cs/Terminal.Gui/blob/fc1faba7452ccbdf49028ac49f0c9f0f42bbae91/Terminal.Gui/Views/ListView.cs#L433-L461
        private static void RenderUstr(View view, string ustr, int col, int line, int width, int start = 0)
        {
            var used = 0;
            var index = start;

            while (index < ustr.Length)
            {
                var (rune, size) = ustr.DecodeRune(index, index - ustr.Length);
                var count = rune.GetColumns();

                if (used + count >= width)
                {
                    break;
                }

                view.AddRune(rune);
                used += count;
                index += size;
            }

            while (used < width)
            {
                view.AddRune((Rune)' ');
                used++;
            }
        }

        public void Dispose()
        {
            _loadedPlaylist = null;
        }
    }

    #endregion
}