# MusicSharp
[![.NET](https://github.com/markjamesm/Baseball-Sharp/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/markjamesm/MusicSharp/actions) [![C#](https://img.shields.io/badge/Language-CSharp-darkgreen.svg)](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)) [![License](https://img.shields.io/badge/License-GPL-orange.svg)](https://www.gnu.org/licenses/gpl-3.0.en.html)

MusicSharp is a cross-platform Terminal User Interface (TUI) music player written in C# (.NET 8) with the goal of being minimalistic and light on resources.

Currently in beta, MusicSharp makes use of the [NAudio](https://github.com/naudio/NAudio) and [Terminal.Gui](https://github.com/migueldeicaza/gui.cs) libraries. A project build log can be [found here](https://markjames.dev/blog/developing-a-cli-music-player-csharp/)

## Screenshot

<img src="https://user-images.githubusercontent.com/20845425/99861949-06763200-2b66-11eb-9d5a-9bf2ea5151ee.png" alt="Screenshot of MusicSharp">

## Features

- Play audio files.
- Load music playlists (M3U)
- Audio streaming.
- Lightweight

## Planned

- Save playlists.
- Cross-platform support.

## Installation

Download the [latest release of MusicSharp](https://github.com/markjamesm/MusicSharp/releases) and follow the installation instructions.

### Additional step for Linux

Install GStreamer:
```bash
# Ubuntu/Debian:
sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly
```

```bash 
# Fedora
sudo dnf install gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free
```

## Want to Contribute?

To see how you can submit PRs, be sure to check out our [contributing page](https://github.com/markjamesm/MusicSharp/blob/main/CONTRIBUTING.md).

A list of open tasks can be found on our [issues page](https://github.com/markjamesm/MusicSharp/issues).