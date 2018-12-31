using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    public class Ship
    {
        public enum ShipType
        {
            AircraftCarrier,
            Battleship,
            Cruiser,
            Destroyer,
            Submarine
        }

        public enum ShipDirection
        {
           Horizontal,
           Vertical
        }

        public ShipType Type { get; private set; }
        public string ShipName { get; set; }
        public int Size { get; set; }
        public ShipDirection Direction { get; set; }
        public Coordinate BowCoordinate { get; set; }
        public List<Coordinate> CoordinatesList { get; set; }

        public Ship(ShipType shiptype, string shipName)
        {
            this.Direction = ShipDirection.Horizontal;
            this.Type = shiptype;
            this.ShipName = shipName;
            this.CoordinatesList = new List<Coordinate>();

            switch (shiptype)
            {           
                case ShipType.AircraftCarrier:
                    {
                        
                        this.Size = 5;
                        return;
                    }
                case ShipType.Battleship:
                    {
                        this.Size = 4;
                        return;
                    }
                case ShipType.Cruiser:
                    {
                        this.Size = 3;
                        return;
                    }
                case ShipType.Submarine:
                    {
                        this.Size = 3;
                        return;
                    }
                case ShipType.Destroyer:
                    {
                        this.Size = 2;
                        return;
                    }
                default:
                    this.Size = 0;
                    return;
            }

        }

        public void RotateShip(Ship ship)
        {
            ShipDirection currentDirection = ship.Direction;

            if (currentDirection == ShipDirection.Horizontal)
                ship.Direction = ShipDirection.Vertical;
            else
                ship.Direction = ShipDirection.Horizontal;
        }

        public List<Coordinate> ShipCoordinatesList(Ship ship)
        {
            if (ship.BowCoordinate == null)
                throw new Exception("Ship bow coordinate cannot be null");

            var retval = new List<Coordinate>();
            if (ship.Direction == ShipDirection.Horizontal)
            {
                var startingXValue = ship.BowCoordinate.X;
                for (int i = startingXValue; i < (startingXValue + ship.Size); i++)
                {
                    retval.Add(new Coordinate(i, ship.BowCoordinate.Y, new HitState()));
                }
            }
            else
            {
                var startingYValue = ship.BowCoordinate.Y;
                for (int i = startingYValue; i < (startingYValue + ship.Size); i++)
                {
                    retval.Add(new Coordinate(ship.BowCoordinate.X, i, new HitState()));
                }
            }
            return retval;
        }

        public void InitializeCoordinateList(Ship ship, List<Coordinate> coords)
        {
            foreach (Coordinate c in coords)
            {
                ship.CoordinatesList.Add(c);
            }
        }
    }
}
