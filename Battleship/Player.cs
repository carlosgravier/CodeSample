using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Player
    {
        public string PlayerName { get; private set; }
        public List<Ship> Fleet = new List<Ship>();
        public List<Coordinate> TargetGrid { get; private set; }
        public List<Coordinate> FleetGrid { get; private set; }

        public List<Coordinate> PlayerOccupiedCoordinates { get; private set; }

        public Player(string playerName)
        {
            this.PlayerName = playerName;
            PopulateShips();
            //TargetGrid = CreateGrid();
            //FleetGrid = CreateGrid();
            PlayerOccupiedCoordinates = new List<Coordinate>();
        }

        private void PopulateShips()
        {
            Fleet.Add(new Ship(Ship.ShipType.AircraftCarrier, "Aircraft Carrier"));
            Fleet.Add(new Ship(Ship.ShipType.Battleship, "Battleship"));
            Fleet.Add(new Ship(Ship.ShipType.Cruiser, "Cruiser"));
            Fleet.Add(new Ship(Ship.ShipType.Destroyer, "Destroyer 1"));
            Fleet.Add(new Ship(Ship.ShipType.Destroyer, "Destroyer 2"));
            Fleet.Add(new Ship(Ship.ShipType.Submarine, "Submarine 1"));
            Fleet.Add(new Ship(Ship.ShipType.Submarine, "Submarine 2"));
        }

        public List<Coordinate> CreateGrid()
        {
            var retval = new List<Coordinate>();


            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var coordinate = new Coordinate(i, j, new HitState());
                    retval.Add(coordinate);
                }
            }
            return retval;
        }
    }

}

