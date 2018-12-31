using Microsoft.VisualStudio.TestTools.UnitTesting;
using Battleship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Tests
{
    [TestClass()]
    public class PlayerTests
    {
        [TestMethod()]
        public void CreateGridTest()
        {
            var player = new Player("TestPlayer");
            var grid = player.CreateGrid();
            Assert.IsTrue(grid.Count == 100);
        }
    }
}