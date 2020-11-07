namespace MusicSharp.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

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
    }
}