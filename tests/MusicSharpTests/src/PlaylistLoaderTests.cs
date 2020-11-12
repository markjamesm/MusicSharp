// <copyright file="PlaylistLoaderTests.cs" company="Mark-James McDougall">
// Licensed under the GNU GPL v3 License. See LICENSE in the project root for license information.
// </copyright>
namespace MusicSharp.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass()]
    public class PlaylistLoaderTests
    {
        [TestMethod()]
        public void Load_NullPlaylist()
        {
            // Arrange
            var playlist = new PlaylistLoader();

            // Act and assert
            Assert.ThrowsException<System.NullReferenceException>(() => playlist.LoadPlaylist(null));
        }
    }
}