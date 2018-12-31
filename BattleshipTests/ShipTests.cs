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
    public class ShipTests
    {
        [TestMethod()]
        public void AircraftCarrierCoordinatesListTest()
        {
            Ship ship = new Ship(Ship.ShipType.AircraftCarrier, "Aircraft Carrier");
            var player = new Player("TestPlayer");
            ship.BowCoordinate = new Coordinate(5, 4, new HitState());
            var coordsList = ship.ShipCoordinatesList(ship);

            Assert.AreEqual(coordsList.Last().X, 9);
            Assert.AreEqual(coordsList.Last().Y, 4);

            ship.RotateShip(ship);
            coordsList = ship.ShipCoordinatesList(ship);

            Assert.AreEqual(coordsList.Last().X, 5);
            Assert.AreEqual(coordsList.Last().Y, 8);
        }
    }
}