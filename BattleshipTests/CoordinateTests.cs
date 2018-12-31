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
    public class CoordinateTests
    {
        [TestMethod()]
        public void IsOffGridTest()
        {
            var coordinate = new Coordinate(-1, 0, new HitState());
            var isOffGrid = coordinate.IsOffGrid(coordinate.X);
            Assert.IsTrue(isOffGrid);
            isOffGrid = coordinate.IsOffGrid(coordinate.Y);
            Assert.IsFalse(isOffGrid);
        }
    }
}