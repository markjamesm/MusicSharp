// <copyright file="WinPlayerTests.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>
namespace MusicSharp.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class WinPlayerTests
    {
        [TestMethod()]
        public void Play_NullFile()
        {
            // arrange
            var player = new WinPlayer();

            // act and assert
            Assert.ThrowsException<System.NullReferenceException>(() => player.OpenFile(null));
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
            // arange
            var player = new WinPlayer();

            // act and assert
            Assert.ThrowsException<System.NullReferenceException>(() => player.PlayFromPlaylist(null));
        }
    }
}