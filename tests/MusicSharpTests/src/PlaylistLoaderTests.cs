// <copyright file="PlaylistLoaderTests.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>

using MusicSharp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicSharpTests;

    [TestClass()]
    public class PlaylistLoaderTests
    {
        [TestMethod()]
        public void Load_NullPlaylist()
        {
            // Act and assert
            Assert.ThrowsException<System.NullReferenceException>(() => PlaylistLoader.LoadPlaylist(null));
        }
    }