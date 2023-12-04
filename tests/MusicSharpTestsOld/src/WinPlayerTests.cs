// <copyright file="WinPlayerTests.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using MusicSharp.SoundEngines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace MusicSharpTests;

[TestClass()]
public class WinPlayerTests
{
    [TestMethod()]
    public void Play_NullFile()
    {
        // arrange
        var player = new WinPlayer();
        var isFileValid = File.Exists("thisisafail.exe");

        // act and assert
        Assert.IsFalse(isFileValid);
    }

    [TestMethod()]
    public void PlayFromPlaylist_NullFile()
    {
        // arange
        var player = new WinPlayer();

        // act and assert
        Assert.ThrowsException<System.NullReferenceException>(() => player.PlayFromPlaylist(null));
    }

    [TestMethod()]
    public void OpenStream_NullFile()
    {
        // arrange
        var player = new WinPlayer();

        // act and assert
        Assert.ThrowsException<System.NullReferenceException>(() => player.PlayFromPlaylist(null));
    }
}