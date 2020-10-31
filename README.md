# MusicSharp
[![Build status](https://github.com/markjamesm/MusicSharp/workflows/.NET%20Core/badge.svg?branch=main)](https://github.com/markjamesm/MusicSharp/actions) [![Platforms](https://img.shields.io/badge/Platforms-Windows-blue)]() [![C#](https://img.shields.io/badge/Language-CSharp-darkgreen.svg)](https://en.wikipedia.org/wiki/C_Sharp_(programming_language)) [![License](https://img.shields.io/badge/License-GPL-orange.svg)](https://www.gnu.org/licenses/gpl-3.0.en.html)

MusicSharp is a Terminal User Interface (TUI) music player for Windows written in C# with the goal of being minimalistic and light on resources.

Currently in an early alpha beta stage, MusicSharp makes use of the [NAudio](https://github.com/naudio/NAudio) and [Terminal.Gui](https://github.com/migueldeicaza/gui.cs) libraries.

## Features

- Load and play audio files.

## Planned

- Save/Load playlists.
- Cross platform support.

## Want to Contribute?

To see how you can submit PRs, be sure to check out our [contributing page](https://github.com/markjamesm/MusicSharp/blob/main/CONTRIBUTING.md).

NAudio currently isn't cross-platform, but the player class is loosely coupled to the GUI by using Dependency Injection (DI). Currently, the GUI injects [Winplayer](https://github.com/markjamesm/MusicSharp/blob/main/src/model/WinPlayer.cs) to handle audio processing. Conceivably, one could write a Linux or MacOS player class which conforms to IPlayer using a cross-platform C# audio library such as [Bassoon](https://gitlab.com/define-private-public/Bassoon), and this would be a great contribution to the project. 

There's also several [issues](https://github.com/markjamesm/MusicSharp/issues) which could use work on.

## Screenshot

<img src="https://user-images.githubusercontent.com/20845425/97771186-c8f72980-1b10-11eb-9082-cf053eb8fd5a.png" alt="Screenshot of MusicSharp 0.6.1.">